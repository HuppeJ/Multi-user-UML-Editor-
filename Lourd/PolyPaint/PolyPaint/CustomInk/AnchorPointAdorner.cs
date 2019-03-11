using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Collections.Generic;

namespace PolyPaint.CustomInk
{
    class AnchorPointAdorner : Adorner
    {
        // Be sure to call the base class constructor.
        public AnchorPointAdorner(UIElement adornedElement)
          : base(adornedElement)
        {
        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout system as part of a rendering pass.
        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

        //    // Some arbitrary drawing implements.
        //    SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
        //    renderBrush.Opacity = 0.2;
        //    Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
        //    double renderRadius = 5.0;

        //    // Draw a circle at each corner.
        //    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
        //    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
        //    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
        //    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        //}

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    var result = base.MeasureOverride(constraint);
        //    // ... add custom measure code here if desired ...
        //    InvalidateVisual();
        //    return result;
        //}

        // The Thumb to drag to rotate the strokes.
        Thumb rotateHandle;
        List<CustomButton> buttons;
        VisualCollection visualChildren;

        // The center of the strokes.
        Point center;

        RotateTransform rotation;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public CustomStroke stroke;

        public CustomInkCanvas canvas;

        public AnchorPointAdorner(UIElement adornedElement, CustomStroke strokeToRotate, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            stroke = strokeToRotate;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = strokeToRotate.GetBounds();
            center = new Point(strokeBounds.X + strokeBounds.Width / 2,
                               strokeBounds.Y + strokeBounds.Height / 2);
            rotation = new RotateTransform(stroke.rotation, center.X, center.Y);

            buttons = new List<CustomButton>();
            buttons.Add(new CustomButton(stroke, canvas, 0));
            buttons.Add(new CustomButton(stroke, canvas, 1));
            buttons.Add(new CustomButton(stroke, canvas, 2));
            buttons.Add(new CustomButton(stroke, canvas, 3));
            //buttons.Add(new Thumb());
            //buttons.Add(new Thumb());
            //buttons.Add(new Thumb());
            //buttons.Add(new Thumb());



            visualChildren = new VisualCollection(this);
            foreach(CustomButton button in buttons)
            {
                // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
                button.RenderTransform = rotation;
                //button.Height = 50;
                //button.Width = 50;
                button.Cursor = Cursors.SizeNWSE;
                button.Width = 10;
                button.Height = 10;
                button.Background = Brushes.Red;
                
                visualChildren.Add(button);
            }

            //strokeBounds = ((Path) adornedElement).Data.Bounds;
            strokeBounds = strokeToRotate.GetBounds();
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

            center = new Point(strokeBounds.X,
                               strokeBounds.Y);

            // The rectangle that determines the position of the Thumb.
            Rect handleRect = new Rect(center.X,
                                  center.Y - (strokeBounds.Height / 2),
                                  strokeBounds.Width, 
                                  strokeBounds.Height);

            if (rotation != null)
            {
                handleRect.Transform(rotation.Value);
            }

            // Draws the thumb and the rectangle around the strokes.
            buttons[0].Arrange(handleRect);

            // The rectangle that determines the position of the Thumb.
            handleRect = new Rect(center.X + (strokeBounds.Width / 2),
                                  center.Y,
                                  strokeBounds.Width,
                                  strokeBounds.Height);

            if (rotation != null)
            {
                handleRect.Transform(rotation.Value);
            }

            // Draws the thumb and the rectangle around the strokes.
            buttons[1].Arrange(handleRect);

            // The rectangle that determines the position of the Thumb.
            handleRect = new Rect(center.X,
                                  center.Y + (strokeBounds.Height / 2),
                                  strokeBounds.Width,
                                  strokeBounds.Height);

            if (rotation != null)
            {
                handleRect.Transform(rotation.Value);
            }

            // Draws the thumb and the rectangle around the strokes.
            buttons[2].Arrange(handleRect);

            // The rectangle that determines the position of the Thumb.
            handleRect = new Rect(center.X - (strokeBounds.Width / 2),
                                  center.Y,
                                  strokeBounds.Width,
                                  strokeBounds.Height);

            if (rotation != null)
            {
                handleRect.Transform(rotation.Value);
            }

            // Draws the thumb and the rectangle around the strokes.
            buttons[3].Arrange(handleRect);

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
            //rotation = new RotateTransform(angle, center.X, center.Y);
            //outline.RenderTransform = rotation;
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