using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            IsReadOnly = true;
            IsHitTestVisible = false;
        }

        public CustomTextBox(string name, double width, double height) : base()
        {
            FontSize = 12;
            Text = name;
            BorderThickness = new Thickness(1);
            Width = width;
            Height = height;
            TextWrapping = TextWrapping.Wrap;
            AcceptsReturn = true;
            IsReadOnly = true;
            IsHitTestVisible = false;
        }
    }
}
