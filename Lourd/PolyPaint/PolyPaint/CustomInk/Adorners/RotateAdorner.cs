using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Ink;
using PolyPaint.CustomInk.Strokes;
using PolyPaint.CustomInk.Adorners;

namespace PolyPaint.CustomInk
{
    public class RotateAdorner : CustomAdorner
    {
        // The Thumb to drag to rotate the strokes.
        Thumb rotateHandle;

        VisualCollection visualChildren;

        // The center of the strokes.
        Point center;
        double lastAngle;

        RotateTransform rotation;
        private Path rotatePreview;
        Path line;

        RectangleGeometry NewRectangle = new RectangleGeometry();

        const int HANDLEMARGIN = 35;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public CustomStroke stroke;

        public CustomInkCanvas canvas;

        public RotateAdorner(UIElement adornedElement, CustomStroke strokeToRotate, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            adornedStroke = strokeToRotate;
            stroke = strokeToRotate;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = strokeToRotate.GetCustomBound();
            center = stroke.GetCenter();
            rotation = new RotateTransform((stroke as ShapeStroke).shapeStyle.rotation, center.X, center.Y);

            visualChildren = new VisualCollection(this);

            line = new Path();
            line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF809dce"));
            line.StrokeThickness = 1;

            line.Data = new LineGeometry(new Point(center.X, center.Y - strokeBounds.Height / 2 - 10),
                                         new Point(center.X, center.Y - strokeBounds.Height / 2 - HANDLEMARGIN));
            line.RenderTransform = rotation;

            visualChildren.Add(line);

            rotateHandle = new Thumb();
            rotateHandle.Cursor = Cursors.Hand;
            rotateHandle.Width = 10;
            rotateHandle.Height = 10;
            rotateHandle.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFc8d4ea"),
                (Color)ColorConverter.ConvertFromString("#FF809dce"), 45);

            rotateHandle.DragDelta += new DragDeltaEventHandler(rotateHandle_DragDelta);
            rotateHandle.DragCompleted += new DragCompletedEventHandler(rotateHandle_DragCompleted);

            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(rotation.Angle, 
                rotateHandle.Width / 2,
                rotateHandle.Height / 2 + HANDLEMARGIN + stroke.GetCustomBound().Height / 2));
            transform.Children.Add(new TranslateTransform(-canvas.ActualWidth / 2 + strokeBounds.X + strokeBounds.Width / 2,
                -canvas.ActualHeight / 2 + strokeBounds.Y - HANDLEMARGIN));

            rotateHandle.RenderTransform = transform;

            visualChildren.Add(rotateHandle);

            rotatePreview = new Path();
            rotatePreview.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBBBBBB"));
            rotatePreview.StrokeThickness = 1;
            visualChildren.Add(rotatePreview);
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
            Rect handleRect = new Rect(strokeBounds.X,
                                  strokeBounds.Y - (strokeBounds.Height / 2 +
                                                    HANDLEMARGIN),
                                  strokeBounds.Width, strokeBounds.Height);

            if (rotation != null)
            {
                handleRect.Transform(rotation.Value);
            }

            // Draws the thumb and the rectangle around the strokes.
            rotateHandle.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));

            rotatePreview.Arrange(new Rect(finalSize));

            line.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));

            return finalSize;
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

            Point pos = Mouse.GetPosition(this);

            double deltaX = pos.X - center.X;
            double deltaY = pos.Y - center.Y;

            if (deltaY.Equals(0))
            {

                return;
            }

            double tan = deltaX / deltaY;
            double angle = Math.Atan(tan);

            // Convert to degrees.
            angle = angle * 180 / Math.PI;

            // If the mouse crosses the vertical center, 
            // find the complementary angle.
            if (deltaY > 0)
            {
                angle = 180 - Math.Abs(angle);
            }

            // Rotate left if the mouse moves left and right
            // if the mouse moves right.
            if (deltaX < 0)
            {
                angle = -Math.Abs(angle);
            }
            else
            {
                angle = Math.Abs(angle);
            }

            if (Double.IsNaN(angle))
            {
                return;
            }

            // Apply the rotation to the strokes' outline.
            rotation = new RotateTransform(angle, center.X, center.Y);

            NewRectangle = new RectangleGeometry(stroke.GetCustomBound(), 0, 0, rotation);
            rotatePreview.Data = NewRectangle;
            rotatePreview.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));

            line.RenderTransform = rotation;

            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(rotation.Angle, 
                rotateHandle.Width / 2, 
                rotateHandle.Height / 2 + HANDLEMARGIN + stroke.GetCustomBound().Height / 2));
            transform.Children.Add(new TranslateTransform(-canvas.ActualWidth / 2 + strokeBounds.X + strokeBounds.Width / 2,
                -canvas.ActualHeight / 2 + strokeBounds.Y - HANDLEMARGIN));

            rotateHandle.RenderTransform = transform;
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
            
            visualChildren.Remove(rotatePreview);

            canvas.RotateStrokesWithAngle(rotation.Angle - (stroke as ShapeStroke).shapeStyle.rotation);

            // Save the angle of the last rotation.
            lastAngle = rotation.Angle;

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
