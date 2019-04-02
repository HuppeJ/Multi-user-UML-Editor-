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

namespace PolyPaint.CustomInk
{
    class ResizeAdorner : Adorner
    {
        List<Thumb> anchors;
        List<StrokeResizePointThumb> cheatAnchors;

        VisualCollection visualChildren;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public ShapeStroke shapeStroke;

        public CustomInkCanvas canvas;

        RotateTransform rotation;

        private Path resizePreview;

        Vector unitX = new Vector(1, 0);
        Vector unitY = new Vector(0, 1);

        RectangleGeometry NewRectangle = new RectangleGeometry();
        RectangleGeometry OldRectangle = new RectangleGeometry();

        public ResizeAdorner(UIElement adornedElement, CustomStroke customStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            resizePreview = new Path();
            resizePreview.Stroke = Brushes.Gray;
            resizePreview.StrokeThickness = 2;
            visualChildren.Add(resizePreview);

            shapeStroke = customStroke as ShapeStroke;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = customStroke.GetCustomBound();
            Point center = customStroke.GetCenter();
            rotation = new RotateTransform((customStroke as ShapeStroke).shapeStyle.rotation, center.X, center.Y);
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
                anchor.Cursor = Cursors.SizeNWSE;
                anchor.Width = 5;
                anchor.Height = 5;
                anchor.Background = Brushes.Gray;

                switch (index)
                {
                    case 0:
                        anchor.DragDelta += new DragDeltaEventHandler(Top_DragDelta);
                        break;
                    case 1:
                        anchor.DragDelta += new DragDeltaEventHandler(Right_DragDelta);
                        break;
                    case 2:
                        anchor.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);
                        break;
                    case 3:
                        anchor.DragDelta += new DragDeltaEventHandler(Left_DragDelta);
                        break;
                    case 4:
                        anchor.DragDelta += new DragDeltaEventHandler(TopLeft_DragDelta);
                        break;
                    case 5:
                        anchor.DragDelta += new DragDeltaEventHandler(TopRight_DragDelta);
                        break;
                    case 6:
                        anchor.DragDelta += new DragDeltaEventHandler(BottomLeft_DragDelta);
                        break;
                    case 7:
                        anchor.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
                        break;
                    default:
                        break;
                }
                anchor.DragStarted += new DragStartedEventHandler(All_DragStarted);
                anchor.DragCompleted += new DragCompletedEventHandler(All_DragCompleted);
                visualChildren.Add(anchor);
                index++;
            }

            cheatAnchors = new List<StrokeResizePointThumb>();
            cheatAnchors.Add(new StrokeResizePointThumb(shapeStroke, canvas, 0));
            cheatAnchors.Add(new StrokeResizePointThumb(shapeStroke, canvas, 1));
            cheatAnchors.Add(new StrokeResizePointThumb(shapeStroke, canvas, 2));
            cheatAnchors.Add(new StrokeResizePointThumb(shapeStroke, canvas, 3));
            cheatAnchors.Add(new StrokeResizePointThumb(shapeStroke, canvas, 4));
            cheatAnchors.Add(new StrokeResizePointThumb(shapeStroke, canvas, 5));
            cheatAnchors.Add(new StrokeResizePointThumb(shapeStroke, canvas, 6));
            cheatAnchors.Add(new StrokeResizePointThumb(shapeStroke, canvas, 7));
            foreach (Thumb cheatAnchor in cheatAnchors)
            {
                cheatAnchor.Cursor = Cursors.SizeNWSE;
                cheatAnchor.Width = 1;
                cheatAnchor.Height = 1;

                canvas.Children.Add(cheatAnchor);
            }

            strokeBounds = customStroke.GetCustomBound();

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            // Top
            ArrangeAnchor(0, 0, -(strokeBounds.Height / 2));
            // Right
            ArrangeAnchor(1, strokeBounds.Width / 2, 0);
            // Bottom
            ArrangeAnchor(2, 0, strokeBounds.Height / 2);
            // Left
            ArrangeAnchor(3, -(strokeBounds.Width / 2), 0);
            // TopLeft
            ArrangeAnchor(4, -(strokeBounds.Width / 2), -(strokeBounds.Height / 2));
            // TopRight
            ArrangeAnchor(5, strokeBounds.Width / 2, -(strokeBounds.Height / 2));
            // BottomLeft
            ArrangeAnchor(6, -(strokeBounds.Width / 2), strokeBounds.Height / 2);
            // BottomRight
            ArrangeAnchor(7, strokeBounds.Width / 2, strokeBounds.Height / 2);

            resizePreview.Arrange(new Rect(finalSize));

            return finalSize;
        }

        private void ArrangeAnchor(int anchorNumber, double xOffset, double yOffset)
        {
            // The rectangle that determines the position of the Thumb.
            Rect handleRect = new Rect(strokeBounds.X + xOffset,
                                  strokeBounds.Y + yOffset,
                                  strokeBounds.Width,
                                  strokeBounds.Height);

            handleRect.Transform(rotation.Value);
            // Draws the thumb and the rectangle around the strokes.
            anchors[anchorNumber].Arrange(handleRect);
            cheatAnchors[anchorNumber].Arrange(handleRect);
        }

        void All_DragStarted(object sender,
                                        DragStartedEventArgs e)
        {
            Rect rectangle = new Rect(shapeStroke.GetCustomBound().X, 
                                      shapeStroke.GetCustomBound().Y, 
                                      shapeStroke.GetCustomBound().Width, 
                                      shapeStroke.GetCustomBound().Height);
            OldRectangle = new RectangleGeometry(rectangle, 0, 0, rotation);
        }
        
        void All_DragCompleted(object sender,
                                        DragCompletedEventArgs e)
        {
            Point actualPos = Mouse.GetPosition(this);
            if (e.HorizontalChange == 0 && e.VerticalChange == 0)
            {
                visualChildren.Remove(resizePreview);
                InvalidateArrange();
                return;
            }


            visualChildren.Remove(resizePreview);

            canvas.ResizeShape(shapeStroke, NewRectangle, OldRectangle);

            canvas.RefreshLinks(false);
            canvas.RefreshChildren();

            InvalidateArrange();

            DrawingService.UpdateShapes(new StrokeCollection { shapeStroke });
        }

        #region DragDelta
        private void generatePreview(Rect rectangle)
        {
            NewRectangle = new RectangleGeometry(rectangle, 0, 0, rotation);
            resizePreview.Data = NewRectangle;
            resizePreview.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
        }

        private Vector calculateDelta(DragDeltaEventArgs e)
        {
            Point center = shapeStroke.GetCenter();
            RotateTransform rotationInverse = new RotateTransform(360 - shapeStroke.shapeStyle.rotation, center.X, center.Y);
            double deltaX = rotationInverse.Value.Transform(new Vector(e.HorizontalChange * unitX.X, e.VerticalChange * unitX.Y)).X;
            double deltaY = rotationInverse.Value.Transform(new Vector(e.HorizontalChange * unitY.X, e.VerticalChange * unitY.Y)).Y;
            
            return new Vector(deltaX, deltaY);
        }

        void Top_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = calculateDelta(e);
            if (delta.Y > shapeStroke.GetCustomBound().Height - 21)
            {
                delta.Y = shapeStroke.GetCustomBound().Height - 21;
            }
            Rect rectangle = new Rect(shapeStroke.GetCustomBound().X, 
                                      shapeStroke.GetCustomBound().Y + delta.Y, 
                                      Math.Max(21, shapeStroke.GetCustomBound().Width), 
                                      Math.Max(21, shapeStroke.GetCustomBound().Height - delta.Y));
            generatePreview(rectangle);
        }

        void Right_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = calculateDelta(e);
            if (delta.X < -shapeStroke.GetCustomBound().Width + 21)
            {
                delta.X = -shapeStroke.GetCustomBound().Width + 21;
            }
            Rect rectangle = new Rect(shapeStroke.GetCustomBound().X, 
                                      shapeStroke.GetCustomBound().Y, 
                                      Math.Max(21, shapeStroke.GetCustomBound().Width + delta.X), 
                                      Math.Max(21, shapeStroke.GetCustomBound().Height));
            generatePreview(rectangle);
        }

        void Bottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = calculateDelta(e);
            if (delta.Y < -shapeStroke.GetCustomBound().Height + 21)
            {
                delta.Y = -shapeStroke.GetCustomBound().Height + 21;
            }
            Rect rectangle = new Rect(shapeStroke.GetCustomBound().X, 
                                      shapeStroke.GetCustomBound().Y, 
                                      Math.Max(21, shapeStroke.GetCustomBound().Width), 
                                      Math.Max(21, shapeStroke.GetCustomBound().Height + delta.Y));
            generatePreview(rectangle);
        }

        void Left_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = calculateDelta(e);
            if (delta.X > shapeStroke.GetCustomBound().Width - 21)
            {
                delta.X = shapeStroke.GetCustomBound().Width - 21;
            }
            Rect rectangle = new Rect(shapeStroke.GetCustomBound().X + delta.X, 
                                      shapeStroke.GetCustomBound().Y, 
                                      Math.Max(21, shapeStroke.GetCustomBound().Width - delta.X), 
                                      Math.Max(21, shapeStroke.GetCustomBound().Height));
            generatePreview(rectangle);
        }

        void TopLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = calculateDelta(e);
            if (delta.Y > shapeStroke.GetCustomBound().Height - 21)
            {
                delta.Y = shapeStroke.GetCustomBound().Height - 21;
            }
            if (delta.X > shapeStroke.GetCustomBound().Width - 21)
            {
                delta.X = shapeStroke.GetCustomBound().Width - 21;
            }
            Rect rectangle = new Rect(shapeStroke.GetCustomBound().X + delta.X, 
                                      shapeStroke.GetCustomBound().Y + delta.Y, 
                                      Math.Max(21, shapeStroke.GetCustomBound().Width - delta.X), 
                                      Math.Max(21, shapeStroke.GetCustomBound().Height - delta.Y));
            generatePreview(rectangle);
        }

        void TopRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = calculateDelta(e);
            if (delta.Y > shapeStroke.GetCustomBound().Height - 21)
            {
                delta.Y = shapeStroke.GetCustomBound().Height - 21;
            }
            if (delta.X < -shapeStroke.GetCustomBound().Width + 21)
            {
                delta.X = -shapeStroke.GetCustomBound().Width + 21;
            }
            Rect rectangle = new Rect(shapeStroke.GetCustomBound().X, 
                                      shapeStroke.GetCustomBound().Y + delta.Y, 
                                      Math.Max(21, shapeStroke.GetCustomBound().Width + delta.X), 
                                      Math.Max(21, shapeStroke.GetCustomBound().Height - delta.Y));
            generatePreview(rectangle);
        }

        void BottomLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = calculateDelta(e);
            if (delta.Y < -shapeStroke.GetCustomBound().Height + 21)
            {
                delta.Y = -shapeStroke.GetCustomBound().Height + 21;
            }
            if (delta.X > shapeStroke.GetCustomBound().Width - 21)
            {
                delta.X = shapeStroke.GetCustomBound().Width - 21;
            }
            Rect rectangle = new Rect(shapeStroke.GetCustomBound().X + delta.X, 
                                      shapeStroke.GetCustomBound().Y, 
                                      Math.Max(21, shapeStroke.GetCustomBound().Width - delta.X), 
                                      Math.Max(21, shapeStroke.GetCustomBound().Height + delta.Y));
            generatePreview(rectangle);
        }

        void BottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Vector delta = calculateDelta(e);
            if (delta.Y < -shapeStroke.GetCustomBound().Height + 21)
            {
                delta.Y = -shapeStroke.GetCustomBound().Height + 21;
            }
            if (delta.X < -shapeStroke.GetCustomBound().Width + 21)
            {
                delta.X = -shapeStroke.GetCustomBound().Width + 21;
            }
            Rect rectangle = new Rect(shapeStroke.GetCustomBound().X, 
                                      shapeStroke.GetCustomBound().Y, 
                                      Math.Max(21, shapeStroke.GetCustomBound().Width + delta.X), 
                                      Math.Max(21, shapeStroke.GetCustomBound().Height + delta.Y));
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