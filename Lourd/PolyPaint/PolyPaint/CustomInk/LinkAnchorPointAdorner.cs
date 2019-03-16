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

        RotateTransform rotation;
        const int HANDLEMARGIN = 15;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public CustomStroke stroke;
        public CustomInkCanvas canvas;

        public LinkAnchorPointAdorner(UIElement adornedElement, CustomStroke customStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            stroke = customStroke;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = customStroke.GetBounds();
            center = stroke.GetCenter();
            rotation = new RotateTransform(stroke.rotation, center.X, center.Y);

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
           
            //    anchor.DragDelta += new DragDeltaEventHandler(rotateHandle_DragDelta);
            //    anchor.DragCompleted += new DragCompletedEventHandler(rotateHandle_DragCompleted);

            //    visualChildren.Add(anchor);
            //}

            anchors[0].Cursor = Cursors.SizeNWSE;
            anchors[0].Width = 10;
            anchors[0].Height = 10;
            anchors[0].Background = Brushes.IndianRed;

            anchors[0].DragDelta += new DragDeltaEventHandler(rotateHandle_DragDelta);
            anchors[0].DragCompleted += new DragCompletedEventHandler(rotateHandle_DragCompleted);
            visualChildren.Add(anchors[0]);

            anchors[1].Cursor = Cursors.SizeNWSE;
            anchors[1].Width = 10;
            anchors[1].Height = 10;
            anchors[1].Background = Brushes.Blue;
            
            anchors[1].DragDelta += new DragDeltaEventHandler(rotateHandle_DragDelta);
            anchors[1].DragCompleted += new DragCompletedEventHandler(rotateHandle_DragCompleted);
            visualChildren.Add(anchors[1]);

            strokeBounds = customStroke.GetBounds();

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

            line.Data = new LineGeometry(new Point((stroke as LinkStroke).path[0].x, (stroke as LinkStroke).path[0].y),
                                         new Point((stroke as LinkStroke).path[1].x, (stroke as LinkStroke).path[1].y)
                                        );
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

            if (rotation != null)
            {
                handleRect.Transform(rotation.Value);
            }

            // Draws the thumb and the rectangle around the strokes.
            anchors[anchorNumber].Arrange(handleRect);
        }

        /// <summary>
        /// Rotates the rectangle representing the
        /// strokes' bounds as the user drags the
        /// Thumb.
        /// </summary>
        void rotateHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // Find the angle of which to rotate the shape.  Use the right
            // triangle that uses the center and the mouse's position 
            // as vertices for the hypotenuse.

            canvas.addAnchorPoints();

            Point pos = Mouse.GetPosition(this);
            int number = 0;
            if (sender as Thumb == anchors[1]) number = 1;
            canvas.updateLink(stroke, number, pos);

            double deltaX = pos.X - center.X;
            double deltaY = pos.Y - center.Y;

            if (deltaY.Equals(0) && deltaX.Equals(0))
            {
                return;
            }


            // Apply the rotation to the strokes' outline.
            //rotation = new RotateTransform(angle, center.X, center.Y);

            line.Data = new LineGeometry(new Point(0, 0),
                                         new Point((stroke as LinkStroke).path[1].x, (stroke as LinkStroke).path[1].y)
                                        );
            line.Stroke = Brushes.Blue;
            line.StrokeThickness = 1;
            
            //line.RenderTransform = rotation;
        }

        /// <summary>
        /// Rotates the strokes to the same angle as outline.
        /// </summary>
        void rotateHandle_DragCompleted(object sender,
                                        DragCompletedEventArgs e)
        {
            if (rotation == null)
            {
                return;
            }

            //e.HorizontalChange, e.VerticalChange;

            // Redraw rotateHandle.

            canvas.isUpdatingLink = true;

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