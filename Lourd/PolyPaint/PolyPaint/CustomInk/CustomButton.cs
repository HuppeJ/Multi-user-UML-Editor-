using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PolyPaint.CustomInk
{
    public class CustomButton : Button
    {
        public CustomStroke stroke;
        public CustomInkCanvas canvas;
        public int number;

        public CustomButton(CustomStroke stroke, CustomInkCanvas canvas, int number) : base()
        {
            this.stroke = stroke;
            this.canvas = canvas;
            this.number = number;
        }

        protected override void OnClick()
        {
            //Background = Brushes.Blue;
            Point position = TransformToAncestor(canvas).Transform(new Point(0, 0));
            canvas.createLink(stroke, number, position);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            //Background = Brushes.Blue;
            if (canvas.isUpdatingLink) { 
                Point position = TransformToAncestor(canvas).Transform(new Point(0, 0));
                canvas.createLink(stroke, number, position);
            }
        }

    }
}
