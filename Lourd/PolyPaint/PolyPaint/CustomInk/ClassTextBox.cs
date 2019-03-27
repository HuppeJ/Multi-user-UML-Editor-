using PolyPaint.Templates;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace PolyPaint.CustomInk
{
    class ClassTextBox : StackPanel
    {
        public CustomTextBox tb1;
        public CustomTextBox tb2;
        public CustomTextBox tb3;

        private CustomInkCanvas canvas;
        private ClassStroke selectedStroke;

        public ClassTextBox(ClassStroke stroke, CustomInkCanvas canvas) : base()
        {
            IsHitTestVisible = false;
            this.canvas = canvas;
            selectedStroke = stroke;

            ShapeStyle shapeStyle = stroke.shapeStyle;
            tb1 = new CustomTextBox(stroke.name, ClassStroke.WIDTH * shapeStyle.width, 20);
            tb1.TextAlignment = TextAlignment.Center;

            tb2 = new CustomTextBox(getString(stroke.attributes), ClassStroke.WIDTH * shapeStyle.width, (ClassStroke.HEIGHT * shapeStyle.height -20) /2);
            tb2.MinLines = 3;

            tb3 = new CustomTextBox(getString(stroke.methods), ClassStroke.WIDTH * shapeStyle.width, (ClassStroke.HEIGHT * shapeStyle.height - 20) / 2);
            tb3.MinLines = 3;

            if (stroke.shapeStyle.backgroundColor != null)
            {
                tb1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(stroke.shapeStyle.backgroundColor));
                tb2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(stroke.shapeStyle.backgroundColor));
                tb3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(stroke.shapeStyle.backgroundColor));
            }
            if (stroke.shapeStyle.borderColor != null)
            {
                tb1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(stroke.shapeStyle.borderColor));
                tb2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(stroke.shapeStyle.borderColor));
                tb3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(stroke.shapeStyle.borderColor));
            }

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

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            //StrokeCollection sc = new StrokeCollection();
            //sc.Add(selectedStroke);
            //canvas.Select(sc);
        }
    }
}