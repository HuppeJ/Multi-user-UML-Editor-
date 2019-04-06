using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using PolyPaint.CustomInk.Strokes;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System;
using System.Windows.Ink;
using PolyPaint.Services;
using PolyPaint.Vues;
using PolyPaint.Templates;
using PolyPaint.CustomInk.Adorners;

namespace PolyPaint.CustomInk
{
    class ResizeAdorner : CustomAdorner
    {
        List<Thumb> anchors;
        Thumb moveThumb;

        VisualCollection visualChildren;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public CustomStroke customStroke;

        public CustomInkCanvas canvas;

        RotateTransform rotationPreview;
        RotateTransform rotation;

        private Path resizePreview;
        private Path outerBoundPath;

        Vector unitX = new Vector(1, 0);
        Vector unitY = new Vector(0, 1);

        double horizontalChange = 0;
        double verticalChange = 0;

        const int MARGIN = 10;

        RectangleGeometry NewRectangle = new RectangleGeometry();
        RectangleGeometry OldRectangle = new RectangleGeometry();

        double WIDTH_LEGER = 60;

        public ResizeAdorner(UIElement adornedElement, CustomStroke customStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            if(customStroke is LinkStroke)
            {
                WIDTH_LEGER = 1;
            }
            adornedStroke = customStroke;

            visualChildren = new VisualCollection(this);

            if (customStroke is ShapeStroke)
                strokeBounds = customStroke.GetCustomBound();
            else
                strokeBounds = (customStroke as LinkStroke).GetStraightBounds();

            resizePreview = new Path();
            resizePreview.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBBBBBB"));
            resizePreview.StrokeThickness = 1;
            visualChildren.Add(resizePreview);

            this.customStroke = customStroke;
            canvas = actualCanvas;

            Point center = customStroke.GetCenter();
            if(customStroke is ShapeStroke)
                rotation = new RotateTransform((customStroke as ShapeStroke).shapeStyle.rotation, center.X, center.Y);
            else
                rotation = new RotateTransform(0, center.X, center.Y);
            while (rotation.Angle < 0)
            {
                rotation.Angle += 360;
            }

            outerBoundPath = new Path();
            outerBoundPath.Stroke = Brushes.Black;
            outerBoundPath.StrokeDashArray = new DoubleCollection { 5, 2 }; 
            outerBoundPath.StrokeThickness = 1;
            Rect rect = customStroke.GetCustomBound();
            rect.X -= MARGIN;
            rect.Y -= MARGIN;
            rect.Width += MARGIN * 2;
            rect.Height += MARGIN * 2;
            outerBoundPath.Data = new RectangleGeometry(rect, 0, 0, rotation);
            visualChildren.Add(outerBoundPath);

            moveThumb = new Thumb();
            moveThumb.Cursor = Cursors.SizeAll;
            moveThumb.Height = strokeBounds.Height + MARGIN * 2;
            moveThumb.Width = strokeBounds.Width + MARGIN * 2;
            moveThumb.Background = Brushes.Transparent;
            moveThumb.DragDelta += new DragDeltaEventHandler(Move_DragDelta);
            moveThumb.DragCompleted += new DragCompletedEventHandler(Move_DragCompleted);
            moveThumb.DragStarted += new DragStartedEventHandler(All_DragStarted);
            moveThumb.PreviewMouseUp += new MouseButtonEventHandler(LeftMouseUp);
            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(rotation.Angle, moveThumb.Width / 2, moveThumb.Height / 2));
            transform.Children.Add(new TranslateTransform(-canvas.ActualWidth / 2 + strokeBounds.Width / 2 + strokeBounds.X,
                -canvas.ActualHeight / 2 + strokeBounds.Height / 2 + strokeBounds.Y));
            moveThumb.RenderTransform = transform;
            

            visualChildren.Add(moveThumb);

            unitX = rotation.Value.Transform(unitX);
            unitY = rotation.Value.Transform(unitY);
            // RenderTransform = rotation;

            anchors = new List<Thumb>();
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());

            int index = 0;
            foreach (Thumb anchor in anchors)
            {
                anchor.Width = 8;
                anchor.Height = 8;
                anchor.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFc8d4ea"),
                (Color)ColorConverter.ConvertFromString("#FF809dce"), 45);
                anchor.BorderBrush = Brushes.Black;
                if (rotation.Angle % 360 >= 360 - 45 / 2 || rotation.Angle % 360 <= 45 / 2)
                {
                    switch (index)
                    {
                        case 0:
                            anchor.DragDelta += new DragDeltaEventHandler(Top_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 1:
                            anchor.DragDelta += new DragDeltaEventHandler(Right_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 2:
                            anchor.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 3:
                            anchor.DragDelta += new DragDeltaEventHandler(Left_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 4:
                            anchor.DragDelta += new DragDeltaEventHandler(TopLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 5:
                            anchor.DragDelta += new DragDeltaEventHandler(TopRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 6:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 7:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        default:
                            break;
                    }
                } else if (rotation.Angle % 360 > 45 / 2 && rotation.Angle % 360 <= 90 - 45 / 2)
                {
                    switch (index)
                    {
                        case 0:
                            anchor.DragDelta += new DragDeltaEventHandler(Top_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 1:
                            anchor.DragDelta += new DragDeltaEventHandler(Right_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 2:
                            anchor.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 3:
                            anchor.DragDelta += new DragDeltaEventHandler(Left_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 4:
                            anchor.DragDelta += new DragDeltaEventHandler(TopLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 5:
                            anchor.DragDelta += new DragDeltaEventHandler(TopRight_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 6:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 7:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        default:
                            break;
                    }
                } else if (rotation.Angle % 360 > 90 - 45 / 2 && rotation.Angle % 360 <= 90 + 45 / 2)
                {
                    switch (index)
                    {
                        case 0:
                            anchor.DragDelta += new DragDeltaEventHandler(Top_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 1:
                            anchor.DragDelta += new DragDeltaEventHandler(Right_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 2:
                            anchor.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 3:
                            anchor.DragDelta += new DragDeltaEventHandler(Left_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 4:
                            anchor.DragDelta += new DragDeltaEventHandler(TopLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 5:
                            anchor.DragDelta += new DragDeltaEventHandler(TopRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 6:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 7:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        default:
                            break;
                    }
                }
                else if (rotation.Angle % 360 > 90 + 45 / 2 && rotation.Angle % 360 <= 135 + 45 / 2)
                {
                    switch (index)
                    {
                        case 0:
                            anchor.DragDelta += new DragDeltaEventHandler(Top_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 1:
                            anchor.DragDelta += new DragDeltaEventHandler(Right_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 2:
                            anchor.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 3:
                            anchor.DragDelta += new DragDeltaEventHandler(Left_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 4:
                            anchor.DragDelta += new DragDeltaEventHandler(TopLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 5:
                            anchor.DragDelta += new DragDeltaEventHandler(TopRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 6:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 7:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        default:
                            break;
                    }
                }
                else if (rotation.Angle % 360 > 180 - 45 / 2 && rotation.Angle % 360 <= 180 + 45 / 2)
                {
                    switch (index)
                    {
                        case 0:
                            anchor.DragDelta += new DragDeltaEventHandler(Top_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 1:
                            anchor.DragDelta += new DragDeltaEventHandler(Right_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 2:
                            anchor.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 3:
                            anchor.DragDelta += new DragDeltaEventHandler(Left_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 4:
                            anchor.DragDelta += new DragDeltaEventHandler(TopLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 5:
                            anchor.DragDelta += new DragDeltaEventHandler(TopRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 6:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 7:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        default:
                            break;
                    }
                }
                else if (rotation.Angle % 360 > 225 - 45 / 2 && rotation.Angle % 360 <= 225 + 45 / 2)
                {
                    switch (index)
                    {
                        case 0:
                            anchor.DragDelta += new DragDeltaEventHandler(Top_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 1:
                            anchor.DragDelta += new DragDeltaEventHandler(Right_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 2:
                            anchor.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 3:
                            anchor.DragDelta += new DragDeltaEventHandler(Left_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 4:
                            anchor.DragDelta += new DragDeltaEventHandler(TopLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 5:
                            anchor.DragDelta += new DragDeltaEventHandler(TopRight_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 6:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 7:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        default:
                            break;
                    }
                }
                else if (rotation.Angle % 360 > 270 - 45 / 2 && rotation.Angle % 360 <= 270 + 45 / 2)
                {
                    switch (index)
                    {
                        case 0:
                            anchor.DragDelta += new DragDeltaEventHandler(Top_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 1:
                            anchor.DragDelta += new DragDeltaEventHandler(Right_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 2:
                            anchor.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 3:
                            anchor.DragDelta += new DragDeltaEventHandler(Left_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 4:
                            anchor.DragDelta += new DragDeltaEventHandler(TopLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 5:
                            anchor.DragDelta += new DragDeltaEventHandler(TopRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 6:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 7:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (index)
                    {
                        case 0:
                            anchor.DragDelta += new DragDeltaEventHandler(Top_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 1:
                            anchor.DragDelta += new DragDeltaEventHandler(Right_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 2:
                            anchor.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);
                            anchor.Cursor = Cursors.SizeNWSE;
                            break;
                        case 3:
                            anchor.DragDelta += new DragDeltaEventHandler(Left_DragDelta);
                            anchor.Cursor = Cursors.SizeNESW;
                            break;
                        case 4:
                            anchor.DragDelta += new DragDeltaEventHandler(TopLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        case 5:
                            anchor.DragDelta += new DragDeltaEventHandler(TopRight_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 6:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomLeft_DragDelta);
                            anchor.Cursor = Cursors.SizeNS;
                            break;
                        case 7:
                            anchor.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
                            anchor.Cursor = Cursors.SizeWE;
                            break;
                        default:
                            break;
                    }
                }
                anchor.DragStarted += new DragStartedEventHandler(All_DragStarted);
                anchor.DragCompleted += new DragCompletedEventHandler(All_DragCompleted);
                if(!(customStroke is LinkStroke) || !(customStroke as LinkStroke).isAttached())
                    visualChildren.Add(anchor);
                double xOffset = 0;
                double yOffset = 0;
                switch (index)
                {
                    case 0: //Top
                        xOffset = strokeBounds.Width / 2;
                        yOffset = - MARGIN;
                        break;
                    case 1: //Right
                        xOffset = strokeBounds.Width + MARGIN;
                        yOffset = strokeBounds.Height / 2;
                        break;
                    case 2: //Bottom
                        xOffset = strokeBounds.Width / 2;
                        yOffset = strokeBounds.Height + MARGIN;
                        break;
                    case 3: //Left
                        xOffset = -MARGIN;
                        yOffset = strokeBounds.Height / 2;
                        break;
                    case 4: //TopLeft
                        xOffset = -MARGIN;
                        yOffset = -MARGIN;
                        break;
                    case 5: //TopRight
                        xOffset = strokeBounds.Width + MARGIN;
                        yOffset = - MARGIN;
                        break;
                    case 6: //BottomLeft
                        xOffset = - MARGIN;
                        yOffset = strokeBounds.Height + MARGIN;
                        break;
                    case 7: //BottomRight
                        xOffset = strokeBounds.Width + MARGIN;
                        yOffset = strokeBounds.Height + MARGIN;
                        break;
                    default:
                        break;
                }
                ArrangeAnchor(anchor, xOffset, yOffset);
                index++;
            }

            rotationPreview = rotation.Clone();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            for (int i = 0; i < anchors.Count; i++)
            {
                anchors[i].Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
            }

            resizePreview.Arrange(new Rect(finalSize));
            outerBoundPath.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
            
            moveThumb.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));

            return finalSize;
        }

        private void ArrangeAnchor(Thumb anchor, double xOffset, double yOffset)
        {
            
            TransformGroup transformAnchor = new TransformGroup();
            transformAnchor.Children.Add(new RotateTransform(rotation.Angle, 
                -xOffset + strokeBounds.Width / 2 + anchor.Width / 2,
                -yOffset + strokeBounds.Height / 2 + anchor.Height / 2)); 
            transformAnchor.Children.Add(new TranslateTransform(-canvas.ActualWidth / 2 + strokeBounds.X + xOffset,
                -canvas.ActualHeight / 2 + strokeBounds.Y + yOffset));
            anchor.RenderTransform = transformAnchor;
        }

        void All_DragStarted(object sender, DragStartedEventArgs e)
        {
            Rect rectangle = new Rect(customStroke.GetCustomBound().X,
                                      customStroke.GetCustomBound().Y,
                                      customStroke.GetCustomBound().Width,
                                      customStroke.GetCustomBound().Height);
            OldRectangle = new RectangleGeometry(rectangle, 0, 0, rotation);
        }

        void LeftMouseUp(object sender, MouseEventArgs e)
        {
            if (customStroke is LinkStroke)
            {
                canvas.modifyLinkStrokePath(customStroke as LinkStroke, e.GetPosition(canvas));
            }
        }
        
        void All_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Point actualPos = Mouse.GetPosition(this);
            if (e.HorizontalChange == 0 && e.VerticalChange == 0)
            {
                visualChildren.Remove(resizePreview);
                InvalidateArrange();
                return;
            }
            
            visualChildren.Remove(resizePreview);

            canvas.ResizeShape(customStroke, NewRectangle, OldRectangle);

            canvas.RefreshLinks(false);
            canvas.RefreshChildren();

            InvalidateArrange();

            DrawingService.UpdateShapes(new StrokeCollection { customStroke });
        }

        void Move_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (e.HorizontalChange == 0 && e.VerticalChange == 0)
            {
                visualChildren.Remove(resizePreview);
                InvalidateArrange();
                return;
            }


            visualChildren.Remove(resizePreview);

            if (customStroke is LinkStroke && (customStroke as LinkStroke).isAttached())
                canvas.MoveShape(horizontalChange, verticalChange);
            else
                canvas.MoveShape(NewRectangle.Rect.X - OldRectangle.Rect.X, NewRectangle.Rect.Y - OldRectangle.Rect.Y);

            canvas.RefreshLinks(false);
            canvas.RefreshChildren();

            InvalidateArrange();

            DrawingService.UpdateShapes(new StrokeCollection { customStroke });
            DrawingService.UpdateLinks(new StrokeCollection { customStroke });
        }

        #region DragDelta
        private void generatePreview(Rect rectangle)
        {
            NewRectangle = new RectangleGeometry(rectangle, 0, 0, rotationPreview);
            resizePreview.Data = NewRectangle;
            resizePreview.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
        }

        void Move_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector dragVect = new Vector(e.HorizontalChange, e.VerticalChange);
            dragVect = rotation.Value.Transform(dragVect);

            if (dragVect.X != 0 || dragVect.Y != 0)
            {
                rotationPreview.CenterX = customStroke.GetCenter().X + dragVect.X;
                rotationPreview.CenterY = customStroke.GetCenter().Y + dragVect.Y;
                Rect rectangle = new Rect(customStroke.GetCustomBound().X + dragVect.X,
                                          customStroke.GetCustomBound().Y + dragVect.Y,
                                          customStroke.GetCustomBound().Width,
                                          customStroke.GetCustomBound().Height);
                if(customStroke is LinkStroke && (customStroke as LinkStroke).isAttached())
                {
                    horizontalChange = e.HorizontalChange;
                    verticalChange = e.VerticalChange;
                    LinkStroke linkStroke = customStroke.Clone() as LinkStroke;
                    linkStroke.path = new List<Coordinates>();
                    foreach (Coordinates coord in (customStroke as LinkStroke).path)
                    {
                        linkStroke.path.Add(new Coordinates(coord.ToPoint()));
                    }

                    for (int i = 0; i < linkStroke.path.Count; i++)
                    {
                        if (i == 0 && linkStroke.isAttached() && linkStroke.from?.formId != null)
                        {
                            continue;
                        }
                        if (i == linkStroke.path.Count - 1 && linkStroke.isAttached() && linkStroke.to?.formId != null)
                        {
                            continue;
                        }
                        Coordinates coords = linkStroke.path[i];
                        coords.x += e.HorizontalChange;
                        coords.y += e.VerticalChange;
                    }
                    rectangle = linkStroke.GetStraightBounds();
                }
                generatePreview(rectangle);
            }
        }

        void Top_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = new Vector(e.HorizontalChange, e.VerticalChange);
            if (delta.Y > customStroke.GetCustomBound().Height - WIDTH_LEGER)
            {
                delta.Y = customStroke.GetCustomBound().Height - WIDTH_LEGER;
            }
            Rect rectangle = new Rect(customStroke.GetCustomBound().X,
                                      customStroke.GetCustomBound().Y + delta.Y, 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Width), 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Height - delta.Y));
            generatePreview(rectangle);
        }

        void Right_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = new Vector(e.HorizontalChange, e.VerticalChange);
            if (delta.X < -customStroke.GetCustomBound().Width + WIDTH_LEGER)
            {
                delta.X = -customStroke.GetCustomBound().Width + WIDTH_LEGER;
            }
            Rect rectangle = new Rect(customStroke.GetCustomBound().X, 
                                      customStroke.GetCustomBound().Y, 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Width + delta.X), 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Height));
            generatePreview(rectangle);
        }

        void Bottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = new Vector(e.HorizontalChange, e.VerticalChange);
            if (delta.Y < -customStroke.GetCustomBound().Height + WIDTH_LEGER)
            {
                delta.Y = -customStroke.GetCustomBound().Height + WIDTH_LEGER;
            }
            Rect rectangle = new Rect(customStroke.GetCustomBound().X, 
                                      customStroke.GetCustomBound().Y, 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Width), 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Height + delta.Y));
            generatePreview(rectangle);
        }

        void Left_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = new Vector(e.HorizontalChange, e.VerticalChange);
            if (delta.X > customStroke.GetCustomBound().Width - WIDTH_LEGER)
            {
                delta.X = customStroke.GetCustomBound().Width - WIDTH_LEGER;
            }
            Rect rectangle = new Rect(customStroke.GetCustomBound().X + delta.X, 
                                      customStroke.GetCustomBound().Y, 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Width - delta.X), 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Height));
            generatePreview(rectangle);
        }

        void TopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = new Vector(e.HorizontalChange, e.VerticalChange);
            if (delta.Y > customStroke.GetCustomBound().Height - WIDTH_LEGER)
            {
                delta.Y = customStroke.GetCustomBound().Height - WIDTH_LEGER;
            }
            if (delta.X > customStroke.GetCustomBound().Width - WIDTH_LEGER)
            {
                delta.X = customStroke.GetCustomBound().Width - WIDTH_LEGER;
            }
            Rect rectangle = new Rect(customStroke.GetCustomBound().X + delta.X, 
                                      customStroke.GetCustomBound().Y + delta.Y, 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Width - delta.X), 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Height - delta.Y));
            generatePreview(rectangle);
        }

        void TopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = new Vector(e.HorizontalChange, e.VerticalChange);
            if (delta.Y > customStroke.GetCustomBound().Height - WIDTH_LEGER)
            {
                delta.Y = customStroke.GetCustomBound().Height - WIDTH_LEGER;
            }
            if (delta.X < -customStroke.GetCustomBound().Width + WIDTH_LEGER)
            {
                delta.X = -customStroke.GetCustomBound().Width + WIDTH_LEGER;
            }
            Rect rectangle = new Rect(customStroke.GetCustomBound().X, 
                                      customStroke.GetCustomBound().Y + delta.Y, 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Width + delta.X), 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Height - delta.Y));
            generatePreview(rectangle);
        }

        void BottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = new Vector(e.HorizontalChange, e.VerticalChange);
            if (delta.Y < -customStroke.GetCustomBound().Height + WIDTH_LEGER)
            {
                delta.Y = -customStroke.GetCustomBound().Height + WIDTH_LEGER;
            }
            if (delta.X > customStroke.GetCustomBound().Width - WIDTH_LEGER)
            {
                delta.X = customStroke.GetCustomBound().Width - WIDTH_LEGER;
            }
            Rect rectangle = new Rect(customStroke.GetCustomBound().X + delta.X, 
                                      customStroke.GetCustomBound().Y, 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Width - delta.X), 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Height + delta.Y));
            generatePreview(rectangle);
        }

        void BottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = new Vector(e.HorizontalChange, e.VerticalChange);
            if (delta.Y < -customStroke.GetCustomBound().Height + WIDTH_LEGER)
            {
                delta.Y = -customStroke.GetCustomBound().Height + WIDTH_LEGER;
            }
            if (delta.X < -customStroke.GetCustomBound().Width + WIDTH_LEGER)
            {
                delta.X = -customStroke.GetCustomBound().Width + WIDTH_LEGER;
            }
            Rect rectangle = new Rect(customStroke.GetCustomBound().X, 
                                      customStroke.GetCustomBound().Y, 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Width + delta.X), 
                                      Math.Max(WIDTH_LEGER, customStroke.GetCustomBound().Height + delta.Y));
            generatePreview(rectangle);
        }
        #endregion

        // Override the VisualChildrenCount and 
        // GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount
        {
            get { return visualChildren.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }
    }
}