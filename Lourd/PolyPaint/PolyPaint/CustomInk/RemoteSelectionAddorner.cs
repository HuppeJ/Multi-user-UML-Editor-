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

            visualChildren.Add(border);
        }
    }
}
