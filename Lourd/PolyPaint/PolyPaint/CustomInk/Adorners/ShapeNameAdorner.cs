using PolyPaint.CustomInk.Strokes;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PolyPaint.CustomInk
{
    class ShapeNameAdorner : Adorner
    {
        private LinkStroke stroke;
        private CustomTextBox customTextBox;
        private CustomInkCanvas canvas;
        private Rect rectangle;
        private Rect strokeBounds;

        VisualCollection visualChildren;

        RotateTransform rotation;

        // Be sure to call the base class constructor.
        public ShapeNameAdorner(UIElement adornedElement, LinkStroke stroke, CustomInkCanvas canvas)
          : base(adornedElement)
        {
            this.stroke = stroke;
            this.canvas = canvas;

            AddName(stroke, canvas);

            strokeBounds = stroke.GetBounds();
            if (stroke.path.Count == 2)
            {
                rectangle = new Rect(strokeBounds.BottomLeft.X - strokeBounds.Width / 2 - 15,
                strokeBounds.BottomLeft.Y - strokeBounds.Height / 2,
                strokeBounds.Width + 30,
                customTextBox.MaxHeight);
            }
            else if (stroke.path.Count > 0 && stroke.path.Count % 2 == 1)
            {
                double ah = stroke.path.Count / 2;
                int middleIndex = (int) Math.Floor(ah); 
                rectangle = new Rect(stroke.path[middleIndex].x - 15,
                stroke.path[middleIndex].y,
                strokeBounds.Width + 30,
                customTextBox.MaxHeight);
            }
            else
            {
                int biggerMiddleIndex = stroke.path.Count / 2;
                int middleIndex = biggerMiddleIndex - 1;

                rectangle = new Rect(stroke.path[middleIndex].x - 15,
                stroke.path[middleIndex].y,
                strokeBounds.Width + 30,
                customTextBox.MaxHeight);
            }

            
        }

        private void AddName(CustomStroke stroke, CustomInkCanvas canvas)
        {
            visualChildren = new VisualCollection(this);
            customTextBox = new CustomTextBox();
            customTextBox.FontSize = 12;
            customTextBox.Background = null;
            customTextBox.BorderBrush = null;
            customTextBox.Text = stroke.name;
            customTextBox.Width = stroke.GetBounds().Width + 30;
            customTextBox.BorderBrush = null;
            customTextBox.TextAlignment = TextAlignment.Center;
            customTextBox.MaxHeight = 100;

            visualChildren.Add(customTextBox);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var result = base.MeasureOverride(constraint);
            // ... add custom measure code here if desired ...
            InvalidateVisual();
            return result;
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
            if (rectangle.IsEmpty)
            {
                return finalSize;
            }
            
            // Draws the rectangle
            customTextBox.Arrange(rectangle);

            return finalSize;
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