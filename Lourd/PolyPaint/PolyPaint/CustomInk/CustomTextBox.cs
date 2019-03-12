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
            TextWrapping = TextWrapping.Wrap;
            AcceptsReturn = true;
        }

        public CustomTextBox(string name) : base()
        {
            FontSize = 12;
            Text = name;
            BorderThickness = new Thickness(1);
            Width = 150;
            MaxWidth = 150;
            TextWrapping = TextWrapping.Wrap;
            AcceptsReturn = true;
        }
    }
}
