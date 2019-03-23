using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Ink;

namespace PolyPaint.CustomInk
{
    public class ResizeAdorner : Adorner
    {
        // The Thumb to drag to rotate the strokes.
        Thumb upperThumb;
        Thumb upperLeftThumb;
        Thumb upperRightThumb;
        Thumb middleLeftThumb;
        Thumb middleRightThumb;
        Thumb bottomLeftThumb;
        Thumb bottonThumb;
        Thumb bottomRightThumb;

        // The surrounding border.
        Path line;
        Path projectedLine;

        VisualCollection visualChildren;

        // The center of the strokes.
        Point center;
        double deltaY;
        double deltaX;

        ScaleTransform scale;
        RotateTransform rotation;

        const int HANDLEMARGIN = 0;
        const int MARGIN = 5;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public CustomStroke stroke;

        public CustomInkCanvas canvas;

        public ResizeAdorner(UIElement adornedElement, CustomStroke strokeToResize, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            stroke = strokeToResize;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = strokeToResize.GetBounds();
            center = stroke.GetCenter();
            deltaY = 0;
            deltaX = 0;
            rotation = new RotateTransform((stroke as ShapeStroke).shapeStyle.rotation, center.X, center.Y);

            visualChildren = new VisualCollection(this);

            upperThumb = new Thumb();
            upperThumb.Cursor = Cursors.SizeNS;
            upperThumb.Width = 5;
            upperThumb.Height = 5;
            upperThumb.Background = Brushes.Gray;
            upperThumb.DragDelta += new DragDeltaEventHandler(upper_DragDelta);
            upperThumb.DragCompleted += new DragCompletedEventHandler(upper_DragCompleted);

            upperLeftThumb = new Thumb();
            upperLeftThumb.Cursor = Cursors.SizeNWSE;
            upperLeftThumb.Width = 5;
            upperLeftThumb.Height = 5;
            upperLeftThumb.Background = Brushes.Gray;
            upperLeftThumb.DragDelta += new DragDeltaEventHandler(upper_DragDelta);
            upperLeftThumb.DragCompleted += new DragCompletedEventHandler(upper_DragCompleted);

            upperRightThumb = new Thumb();
            upperRightThumb.Cursor = Cursors.SizeNESW;
            upperRightThumb.Width = 5;
            upperRightThumb.Height = 5;
            upperRightThumb.Background = Brushes.Gray;
            upperRightThumb.DragDelta += new DragDeltaEventHandler(upper_DragDelta);
            upperRightThumb.DragCompleted += new DragCompletedEventHandler(upper_DragCompleted);

            middleLeftThumb = new Thumb();
            middleLeftThumb.Cursor = Cursors.SizeWE;
            middleLeftThumb.Width = 5;
            middleLeftThumb.Height = 5;
            middleLeftThumb.Background = Brushes.Gray;
            middleLeftThumb.DragDelta += new DragDeltaEventHandler(upper_DragDelta);
            middleLeftThumb.DragCompleted += new DragCompletedEventHandler(upper_DragCompleted);

            middleRightThumb = new Thumb();
            middleRightThumb.Cursor = Cursors.SizeWE;
            middleRightThumb.Width = 5;
            middleRightThumb.Height = 5;
            middleRightThumb.Background = Brushes.Gray;
            middleRightThumb.DragDelta += new DragDeltaEventHandler(upper_DragDelta);
            middleRightThumb.DragCompleted += new DragCompletedEventHandler(upper_DragCompleted);

            bottomLeftThumb = new Thumb();
            bottomLeftThumb.Cursor = Cursors.SizeNESW;
            bottomLeftThumb.Width = 5;
            bottomLeftThumb.Height = 5;
            bottomLeftThumb.Background = Brushes.Gray;
            bottomLeftThumb.DragDelta += new DragDeltaEventHandler(upper_DragDelta);
            bottomLeftThumb.DragCompleted += new DragCompletedEventHandler(upper_DragCompleted);
            
            bottonThumb = new Thumb();
            bottonThumb.Cursor = Cursors.SizeNS;
            bottonThumb.Width = 5;
            bottonThumb.Height = 5;
            bottonThumb.Background = Brushes.Gray;
            bottonThumb.DragDelta += new DragDeltaEventHandler(upper_DragDelta);
            bottonThumb.DragCompleted += new DragCompletedEventHandler(upper_DragCompleted);

            bottomRightThumb = new Thumb();
            bottomRightThumb.Cursor = Cursors.SizeNWSE;
            bottomRightThumb.Width = 5;
            bottomRightThumb.Height = 5;
            bottomRightThumb.Background = Brushes.Gray;
            bottomRightThumb.DragDelta += new DragDeltaEventHandler(upper_DragDelta);
            bottomRightThumb.DragCompleted += new DragCompletedEventHandler(upper_DragCompleted);

            line = new Path();
            line.Stroke = Brushes.Pink;
            line.StrokeThickness = 2;

            line.Data = new RectangleGeometry(strokeBounds);

            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            line.RenderTransform = rotation;
            
            visualChildren.Add(line);
            visualChildren.Add(upperThumb);
            visualChildren.Add(upperLeftThumb);
            visualChildren.Add(upperRightThumb);
            visualChildren.Add(bottonThumb);
            visualChildren.Add(bottomLeftThumb);
            visualChildren.Add(bottomRightThumb);
            visualChildren.Add(middleLeftThumb);
            visualChildren.Add(middleRightThumb);
        }

        /// <summary>
        /// Draw the rotation handle and the outline of
        /// the element.
        /// </summary>
        /// <param name="finalSize">The final area within the 
        /// parent that this element should use to arrange 
        /// itself and its children.</param>
        /// <returns>The actual size used. </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }
            
            // The rectangle that determines the position of the Thumb.
            Rect upper = new Rect(strokeBounds.X,
                                  strokeBounds.Y - strokeBounds.Height/2 - MARGIN,
                                  strokeBounds.Width, strokeBounds.Height);
            Rect bottom = new Rect(strokeBounds.X,
                                  strokeBounds.Y + strokeBounds.Height / 2 + MARGIN,
                                  strokeBounds.Width, strokeBounds.Height);
            Rect right = new Rect(strokeBounds.X + strokeBounds.Width - MARGIN,
                                  strokeBounds.Y,
                                  strokeBounds.Width, strokeBounds.Height);
            Rect left = new Rect(strokeBounds.X - strokeBounds.Width + MARGIN,
                                  strokeBounds.Y,
                                  strokeBounds.Width, strokeBounds.Height);
            Rect upperLeft = new Rect(strokeBounds.X - strokeBounds.Width + MARGIN,
                                  strokeBounds.Y - strokeBounds.Height / 2 - MARGIN,
                                  strokeBounds.Width, strokeBounds.Height);
            Rect upperRight = new Rect(strokeBounds.X + strokeBounds.Width - MARGIN,
                                  strokeBounds.Y - strokeBounds.Height / 2 - MARGIN,
                                  strokeBounds.Width, strokeBounds.Height);
            Rect bottomRight = new Rect(strokeBounds.X + strokeBounds.Width - MARGIN,
                                  strokeBounds.Y + strokeBounds.Height / 2 + MARGIN,
                                  strokeBounds.Width, strokeBounds.Height);
            Rect bottomLeft = new Rect(strokeBounds.X - strokeBounds.Width + MARGIN,
                                  strokeBounds.Y + strokeBounds.Height / 2 + MARGIN,
                                  strokeBounds.Width, strokeBounds.Height);

            if (rotation != null)
            {
                upper.Transform(rotation.Value);
                bottom.Transform(rotation.Value);
                right.Transform(rotation.Value);
                left.Transform(rotation.Value);
                upperLeft.Transform(rotation.Value);
                upperRight.Transform(rotation.Value);
                bottomLeft.Transform(rotation.Value);
                bottomRight.Transform(rotation.Value);
            }

            // Draws the thumb and the rectangle around the strokes.
            upperThumb.Arrange(upper);
            upperLeftThumb.Arrange(upperLeft);
            upperRightThumb.Arrange(upperRight);
            bottomLeftThumb.Arrange(bottomLeft);
            bottomRightThumb.Arrange(bottomRight);
            bottonThumb.Arrange(bottom);
            middleLeftThumb.Arrange(left);
            middleRightThumb.Arrange(right);

            line.Arrange(upper);
            return finalSize;
        }

        /// <summary>
        /// Rotates the rectangle representing the
        /// strokes' bounds as the user drags the
        /// Thumb.
        /// </summary>
        void upper_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // Find the angle of which to rotate the shape.  Use the right
            // triangle that uses the center and the mouse's position 
            // as vertices for the hypotenuse.

            Point pos = Mouse.GetPosition(this);
            
            deltaY = stroke.GetBounds().Y - pos.Y;

            if (deltaY.Equals(0) || deltaY < -strokeBounds.Height + 10)
            {
                return;
            }

            double ratioY = (deltaY + strokeBounds.Height) / strokeBounds.Height;
            
            // Apply the resize to the strokes' outline.
            scale = new ScaleTransform(1, ratioY);
            
            line.RenderTransform = scale;
        }

        /// <summary>
        /// Rotates the strokes to the same angle as outline.
        /// </summary>
        void upper_DragCompleted(object sender,
                                        DragCompletedEventArgs e)
        {
            if (scale == null)
            {
                return;
            }
            Point corner = new Point(strokeBounds.X + deltaX, strokeBounds.Y + deltaY);
            canvas.ResizeStrokesWithScales(scale.ScaleX, scale.ScaleY, corner);

            // Save the angle of the last rotation.
            // lastAngle = rotation.Angle;

            // Redraw rotateHandle.
            InvalidateArrange();
        }

        /// <summary>
        /// Gets the strokes of the adorned element 
        /// (in this case, an InkPresenter).
        /// </summary>
        private StrokeCollection AdornedStrokes
        {
            get
            {
                return ((InkPresenter)AdornedElement).Strokes;
            }
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
