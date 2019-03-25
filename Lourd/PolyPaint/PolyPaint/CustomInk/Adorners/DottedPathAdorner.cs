using PolyPaint.CustomInk.Strokes;
using PolyPaint.Templates;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolyPaint.CustomInk
{
    class DottedPathAdorner : Adorner
    {
        Rect strokeBounds = Rect.Empty;
        private LinkStroke linkStroke;
        private CustomInkCanvas canvas;

        private RectangleGeometry fill;
        private Path linkPath;
        LineGeometry lineGeom;


        VisualCollection visualChildren;

        // Be sure to call the base class constructor.
        public DottedPathAdorner(UIElement adornedElement, LinkStroke stroke, CustomInkCanvas canvas)
          : base(adornedElement)
        {
            this.linkStroke = stroke;
            this.canvas = canvas;
            visualChildren = new VisualCollection(this);
            strokeBounds = stroke.GetBounds();

            linkPath = new Path();
            
            linkPath.Data = (adornedElement as Path).Data;

            linkPath.Stroke = (Brush) new BrushConverter().ConvertFromString(stroke.style.color);
            linkPath.StrokeThickness = stroke.style.thickness;
            linkPath.StrokeDashArray = new DoubleCollection { 1, 1 };
            linkPath.IsHitTestVisible = false;

            //PathGeometry linkPathGeom = new PathGeometry();
            //lineGeom = new LineGeometry();
            //for (int i = 1; i < linkStroke.path.Count; i++)
            //{
            //    lineGeom.StartPoint = linkStroke.path[i - 1].ToPoint();
            //    lineGeom.EndPoint = linkStroke.path[i].ToPoint();
            //    linkPathGeom.AddGeometry(lineGeom);
            //}
            //linkPath.Data = linkPathGeom;

            visualChildren.Add(linkPath);
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

            linkPath.Arrange(new Rect(finalSize));
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
