using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.CustomInk
{
    class ClassTextBox : StackPanel
    {
        public CustomTextBox tb1;
        public CustomTextBox tb2;
        public CustomTextBox tb3;

        public ClassTextBox() : base()
        {
            tb1 = new CustomTextBox("This is textbox 1");
            tb2 = new CustomTextBox("This is textbox 2");
            tb3 = new CustomTextBox("This is textbox 3");

            Orientation = Orientation.Vertical;

            Children.Add(tb1);
            Children.Add(tb2);
            Children.Add(tb3);
        }
    }
}