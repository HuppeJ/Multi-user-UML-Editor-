using System;
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
    }
}
