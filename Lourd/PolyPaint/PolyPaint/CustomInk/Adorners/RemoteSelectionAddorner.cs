using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolyPaint.CustomInk
{
    class RemoteSelectionAddorner : Adorner
    {
        Rect strokeBounds = Rect.Empty;
        private CustomStroke stroke;
        private CustomInkCanvas canvas;

        private RectangleGeometry fill;
        private Path border;

        RotateTransform rotation;

        VisualCollection visualChildren;

        // Be sure to call the base class constructor.
        public RemoteSelectionAddorner(UIElement adornedElement, CustomStroke stroke, CustomInkCanvas canvas)
          : base(adornedElement)
        {
            this.stroke = stroke;
            this.canvas = canvas;
            visualChildren = new VisualCollection(this);
            strokeBounds = stroke.GetBounds();

            fill = new RectangleGeometry(strokeBounds);
            border = new Path();
            border.Data = fill;
            border.Stroke = Brushes.Red;
            border.StrokeThickness = 5;


            Point center = stroke.GetCenter();
            rotation = new RotateTransform((stroke as ShapeStroke).shapeStyle.rotation, center.X, center.Y);

            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            border.RenderTransform = rotation;

            visualChildren.Add(border);
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
            
            border.Arrange(new Rect(finalSize));
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
