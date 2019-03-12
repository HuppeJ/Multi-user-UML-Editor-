using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PolyPaint.CustomInk
{
    class EditionAdorner : Adorner
    {
        private CustomStroke stroke;
        private EditionButton button;
        private CustomInkCanvas canvas;
        private Rect rectangle;

        VisualCollection visualChildren;

        // Be sure to call the base class constructor.
        public EditionAdorner(UIElement adornedElement, CustomStroke stroke, CustomInkCanvas canvas)
          : base(adornedElement)
        {
            this.stroke = stroke;
            this.canvas = canvas;

            rectangle = new Rect(stroke.GetBounds().TopRight.X, stroke.GetBounds().TopRight.Y - 20, 40, 20);

            AddButton(stroke, canvas);
        }

        private void AddButton(CustomStroke stroke, CustomInkCanvas canvas)
        {
            visualChildren = new VisualCollection(this);
            button = new EditionButton(stroke, canvas);
            button.Cursor = Cursors.SizeNWSE;
            button.Width = 40;
            button.Height = 20;
            button.Background = Brushes.White;
            button.Content = "Edit";

            visualChildren.Add(button);
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
            button.Arrange(rectangle);
    
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