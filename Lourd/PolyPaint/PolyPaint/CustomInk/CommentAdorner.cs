﻿using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PolyPaint.CustomInk
{
    class CommentAdorner : Adorner
    {
        private CustomStroke stroke;
        private CustomTextBox customTextBox;
        private CustomInkCanvas canvas;
        private Rect rectangle;

        VisualCollection visualChildren;

        // The center of the strokes.
        Point center;
        RotateTransform rotation;

        // Be sure to call the base class constructor.
        public CommentAdorner(UIElement adornedElement, CustomStroke stroke, CustomInkCanvas canvas)
          : base(adornedElement)
        {
            this.stroke = stroke;
            this.canvas = canvas;
            Rect bounds = stroke.GetBounds();
            center = stroke.GetCenter();
            rotation = new RotateTransform((stroke as ShapeStroke).shapeStyle.rotation, center.X, center.Y);

            rectangle = new Rect(bounds.TopLeft.X, bounds.TopLeft.Y, bounds.Width, bounds.Height);

            AddComment(stroke, canvas);
        }

        private void AddComment(CustomStroke stroke, CustomInkCanvas canvas)
        {
            visualChildren = new VisualCollection(this);
            customTextBox = new CustomTextBox(stroke.name, stroke.GetBounds().Width, stroke.GetBounds().Height);
            customTextBox.LayoutTransform = new RotateTransform((stroke as ShapeStroke).shapeStyle.rotation, center.X, center.Y);

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