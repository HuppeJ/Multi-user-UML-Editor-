using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.CustomInk
{
    class ClassTextBox : StackPanel
    {
        public CustomTextBox tb1;
        public CustomTextBox tb2;
        public CustomTextBox tb3;

        public ClassTextBox(string name, List<string> attributes, List<string> methods) : base()
        {
            tb1 = new CustomTextBox(name);
            tb1.TextAlignment = TextAlignment.Center;
            
            tb2 = new CustomTextBox(getString(attributes));
            tb3 = new CustomTextBox(getString(methods));

            Orientation = Orientation.Vertical;

            Children.Add(tb1);
            Children.Add(tb2);
            Children.Add(tb3);
        }

        private string getString(List<string> list)
        {
            string completeString = "";

            foreach(string str in list)
            {
                completeString += str;
                completeString += Environment.NewLine;
            }

            return completeString.Trim();
        }
    }
}