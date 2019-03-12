using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.CustomInk
{
    class CustomTextBox : TextBox
    {
        public CustomTextBox() : base()
        {
            FontSize = 12;
            BorderThickness = new Thickness(0);
        }

        public CustomTextBox(string name) : base()
        {
            FontSize = 12;
            Text = name;
            BorderThickness = new Thickness(1);
        }
    }
}
