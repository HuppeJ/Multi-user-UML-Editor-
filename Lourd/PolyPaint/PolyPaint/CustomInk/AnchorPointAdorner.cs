using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace PolyPaint.CustomInk
{
    class AnchorPointAdorner : Adorner
    {
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

            center = new Point(strokeBounds.X, strokeBounds.Y);

            ArrangeButton(0, 0, - strokeBounds.Height / 2);
            ArrangeButton(1, strokeBounds.Width / 2, 0);
            ArrangeButton(2, 0, strokeBounds.Height / 2);
            ArrangeButton(3, - strokeBounds.Width / 2, 0);

            return finalSize;
        }

        private void ArrangeButton(int buttonNumber, double xOffset, double yOffset)
        {
            // The rectangle that determines the position of the Thumb.
            Rect handleRect = new Rect(center.X + xOffset,
                                  center.Y + yOffset,
                                  strokeBounds.Width,
                                  strokeBounds.Height);

            if (rotation != null)
            {
                handleRect.Transform(rotation.Value);
            }

            // Draws the thumb and the rectangle around the strokes.
            buttons[buttonNumber].Arrange(handleRect);
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