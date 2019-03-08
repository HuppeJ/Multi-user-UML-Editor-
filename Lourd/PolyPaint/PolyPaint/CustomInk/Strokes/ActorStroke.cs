using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace PolyPaint.CustomInk
{
    public class ActorStroke : CustomStroke
    {
        public ActorStroke(StylusPointCollection pts) : base(pts)
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
            img.UriSource = new Uri("../../Resources/actor.png", UriKind.Relative);
            img.Rotation = rotation;
            img.EndInit();

            drawingContext.DrawImage(img, new Rect(GetTheFirstPoint(), GetTheLastPoint()));

        }
        
    }
}