using System.Collections.Generic;

namespace PolyPaint.Templates
{
    class EditCanvas
    {
        public Canvas canevas { get; set; }
        public string username { get; set; }

        public EditCanvas()
        {
        }

        public EditCanvas(Canvas canevas, string username)
        {
            this.canevas = canevas;
            this.username = username;
        }
    }
}