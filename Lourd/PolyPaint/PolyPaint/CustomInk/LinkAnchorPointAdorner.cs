using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using PolyPaint.CustomInk.Strokes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;
using System.Windows.Shapes;

namespace PolyPaint.CustomInk
{
    class LinkAnchorPointAdorner : Adorner
    {
        List<Thumb> anchors;
        VisualCollection visualChildren;

        // The center of the strokes.
        Point center;

        // The linkLine
        Path line;

        const int HANDLEMARGIN = 15;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public LinkStroke stroke;
        public CustomInkCanvas canvas;

        public LinkAnchorPointAdorner(UIElement adornedElement, LinkStroke linkStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            stroke = linkStroke;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = linkStroke.GetBounds();
            center = stroke.GetCenter();

            anchors = new List<Thumb>();
            // Pour une ShapeStroke
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            
            visualChildren = new VisualCollection(this);
            //foreach (Thumb anchor in anchors)
            //{
            //    anchor.Cursor = Cursors.SizeNWSE;
            //    anchor.Width = 10;
            //    anchor.Height = 10;
            //    anchor.Background = Brushes.IndianRed;

            //    anchor.DragDelta += new DragDeltaEventHandler(dragHandle_DragDelta);
            //    anchor.DragCompleted += new DragCompletedEventHandler(dragHandle_DragCompleted);
            //    anchor.DragStarted += new DragStartedEventHandler(dragHandle_DragStarted);

            //    visualChildren.Add(anchor);
            //}

            anchors[0].Cursor = Cursors.SizeNWSE;
            anchors[0].Width = 10;
            anchors[0].Height = 10;
            anchors[0].Background = Brushes.IndianRed;

            anchors[0].DragDelta += new DragDeltaEventHandler(dragHandle_DragDelta);
            anchors[0].DragCompleted += new DragCompletedEventHandler(dragHandle_DragCompleted);
            anchors[0].DragStarted += new DragStartedEventHandler(dragHandle_DragStarted);

            visualChildren.Add(anchors[0]);

            anchors[1].Cursor = Cursors.SizeNWSE;
            anchors[1].Width = 10;
            anchors[1].Height = 10;
            anchors[1].Background = Brushes.Blue;
            
            anchors[1].DragDelta += new DragDeltaEventHandler(dragHandle_DragDelta);
            anchors[1].DragCompleted += new DragCompletedEventHandler(dragHandle_DragCompleted);
            anchors[1].DragStarted += new DragStartedEventHandler(dragHandle_DragStarted);
            visualChildren.Add(anchors[1]);

            strokeBounds = linkStroke.GetBounds();

            line = new Path();
            
            visualChildren.Add(line);

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            center = stroke.GetCenter();

            if ((stroke as LinkStroke).path.Count > 1)
            {

                ArrangeAnchor(0, -center.X + (stroke as LinkStroke).path[0].x, -center.Y + (stroke as LinkStroke).path[0].y);
                ArrangeAnchor(1, -center.X + (stroke as LinkStroke).path[1].x, -center.Y + (stroke as LinkStroke).path[1].y);
            }
            
            line.Arrange(new Rect(finalSize));

            return finalSize;
        }

        private void ArrangeAnchor(int anchorNumber, double xOffset, double yOffset)
        {
            // The rectangle that determines the position of the Thumb.
            Rect handleRect = new Rect(strokeBounds.X + xOffset,
                                  strokeBounds.Y + yOffset,
                                  strokeBounds.Width,
                                  strokeBounds.Height);

            // Draws the thumb and the rectangle around the strokes.
            anchors[anchorNumber].Arrange(handleRect);
        }

        void dragHandle_DragStarted(object sender,
                                        DragStartedEventArgs e)
        {
            canvas.addAnchorPoints();
            canvas.isUpdatingLink = true;

        }

        void dragHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // Find the angle of which to rotate the shape.  Use the right
            // triangle that uses the center and the mouse's position 
            // as vertices for the hypotenuse.
            
            Point pos = Mouse.GetPosition(this);
            int number = 0;
            if (sender as Thumb == anchors[1]) number = 1;

            double deltaX = pos.X - center.X;
            double deltaY = pos.Y - center.Y;

            if (deltaY.Equals(0) && deltaX.Equals(0))
            {
                return;
            }


            // Apply the rotation to the strokes' outline.
            //rotation = new RotateTransform(angle, center.X, center.Y);

            // works!!
            //line.Data = new LineGeometry(new Point(0, 0),
            //                             new Point((stroke as LinkStroke).path[1].x, (stroke as LinkStroke).path[1].y)
            //                            );
            line.Stroke = Brushes.Blue;
            line.StrokeThickness = 1;
            
            //line.RenderTransform = rotation;
        }

        
        void dragHandle_DragCompleted(object sender,
                                        DragCompletedEventArgs e)
        {
            //e.HorizontalChange, e.VerticalChange;

            // Redraw rotateHandle.

            Point actualPos = Mouse.GetPosition(this);

            CustomStroke strokeTo = null;
            int number = 0;


            foreach (UIElement thumb in canvas.Children)
            {
                if (thumb.GetType() == typeof(StrokeAnchorPointThumb))
                {
                    Point thumbPosition = thumb.TransformToAncestor(canvas).Transform(new Point(0, 0));

                    StrokeAnchorPointThumb cheatThumb = thumb as StrokeAnchorPointThumb;
                    double y = thumbPosition.Y - actualPos.Y;
                    double x = thumbPosition.X - actualPos.X;

                    double distBetweenPoints = (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                    if (distBetweenPoints <= 30)
                    {
                        strokeTo = cheatThumb.stroke;
                        actualPos = thumbPosition;
                        number = cheatThumb.number;
                    }

                }
            }
            int linkStrokeAnchor = 0;
            if ((sender as Thumb) == anchors[1]) linkStrokeAnchor = 1;


            canvas.updateLink(linkStrokeAnchor, stroke, strokeTo?.guid.ToString(), number, actualPos);

            InvalidateArrange();
        }

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