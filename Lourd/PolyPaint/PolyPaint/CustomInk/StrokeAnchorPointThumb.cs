using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace PolyPaint.CustomInk
{
    public class StrokeAnchorPointThumb: Thumb
    {
        public CustomStroke stroke;
        public CustomInkCanvas canvas;
        public int number;

        public StrokeAnchorPointThumb(CustomStroke stroke, CustomInkCanvas canvas, int number) : base()
        {
            this.stroke = stroke;
            this.canvas = canvas;
            this.number = number;
        }


    }
}
