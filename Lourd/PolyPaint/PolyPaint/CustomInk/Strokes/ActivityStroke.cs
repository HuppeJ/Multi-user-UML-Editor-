using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Globalization;
using PolyPaint.Templates;

namespace PolyPaint.CustomInk
{
    public class ActivityStroke : CustomStroke
    {
        public ActivityStroke(StylusPointCollection pts) : base(pts)
        {
        }

        public ActivityStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts)
        {
        }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }
            if (null == drawingAttributes)
            {
                throw new ArgumentNullException("drawingAttributes");
            }
            DrawingAttributes originalDa = drawingAttributes.Clone();
            SolidColorBrush brush2 = new SolidColorBrush(drawingAttributes.Color);
            brush2.Freeze();
            // drawingContext.DrawRectangle(brush2, null, new Rect(GetTheLeftTopPoint(), GetTheRightBottomPoint()));

            // Create the image
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("../../Resources/activity.png", UriKind.Relative);
            img.EndInit();

            Rect bounds = GetBounds();
            double x = (bounds.Right + bounds.Left) / 2;
            double y = (bounds.Bottom + bounds.Top) / 2;

            TransformGroup transform = new TransformGroup();

            transform.Children.Add(new RotateTransform(rotation, x, y));

            drawingContext.PushTransform(transform);

            drawingContext.DrawImage(img, new Rect(GetTheFirstPoint(), GetTheLastPoint()));

        }
    }
}