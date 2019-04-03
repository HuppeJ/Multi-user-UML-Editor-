using PolyPaint.Vues;
using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.CustomInk
{
    public class EditionButton : Button
    {
        public CustomStroke stroke;
        public CustomInkCanvas canvas;
        public int number;

        public EditionButton(CustomStroke stroke, CustomInkCanvas canvas) : base()
        {
            this.stroke = stroke;
            this.canvas = canvas;
        }

        protected override void OnClick()
        {
            var parent = canvas.Parent;
            while (!(parent is WindowDrawing))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            WindowDrawing windowDrawing = (WindowDrawing)parent;
            if (windowDrawing != null)
            {
                windowDrawing.RenameSelection();
            }
            
        }

    }

    public class DeleteButton : Button
    {
        public CustomStroke stroke;
        public CustomInkCanvas canvas;
        public int number;

        public DeleteButton(CustomStroke stroke, CustomInkCanvas canvas) : base()
        {
            this.stroke = stroke;
            this.canvas = canvas;
        }

        protected override void OnClick()
        {
            var parent = canvas.Parent;
            while (!(parent is WindowDrawing))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            WindowDrawing windowDrawing = (WindowDrawing)parent;
            if (windowDrawing != null)
            {
                windowDrawing.DeleteSelection();
            }

        }

    }
}