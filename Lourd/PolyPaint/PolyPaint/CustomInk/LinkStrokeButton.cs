using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.CustomInk
{
    public class LinkStrokeButton : Button
    {
        public CustomStroke stroke;
        public CustomInkCanvas canvas;
        public int number;

        public LinkStrokeButton(CustomStroke stroke, CustomInkCanvas canvas, int number) : base()
        {
            this.stroke = stroke;
            this.canvas = canvas;
            this.number = number;
        }

        protected override void OnClick()
        {
            //Background = Brushes.Blue;
            Point position = TransformToAncestor(canvas).Transform(new Point(0, 0));
            canvas.updateLink(stroke, number, position);
        }

    }
}
