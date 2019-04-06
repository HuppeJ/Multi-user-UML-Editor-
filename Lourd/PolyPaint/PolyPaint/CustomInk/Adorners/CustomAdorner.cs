using System.Windows;
using System.Windows.Documents;

namespace PolyPaint.CustomInk.Adorners
{
    public class CustomAdorner : Adorner
    {
        public CustomStroke adornedStroke { get; set; }

        public CustomAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }
    }
}
