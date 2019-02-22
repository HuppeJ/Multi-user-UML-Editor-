using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace PolyPaint.CustomInk
{
    public class ArtifactStroke : Stroke
    {
        public ArtifactStroke(StylusPointCollection pts) : base(pts)
        {
            Point lastPoint = pts[pts.Count - 1].ToPoint();
            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(0);
            }
            for (double i = lastPoint.X; i < 60 + lastPoint.X; i += 0.5)
            {
                for (double j = lastPoint.Y; j < 100 + lastPoint.Y; j += 0.5)
                {
                    StylusPoints.Add(new StylusPoint(i, j));
                }
            }
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
            // Create the source
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            // img.UriSource = new Uri("C:/Users/Alex/Pictures/Polar-bear-cub_917.jpg");
            img.UriSource = new Uri("../../Resources/artefact.png", UriKind.Relative);
            img.EndInit();

            drawingContext.DrawImage(img, new Rect(GetTheFirstPoint(), GetTheLastPoint()));

            FormattedText formattedText = new FormattedText(
                "Hello",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                32,
                Brushes.Black);

            // drawingContext.DrawText(formattedText, GetTheFirstPoint());


        }

        Point GetTheLeftTopPoint()
        {
            if (StylusPoints == null)
                throw new ArgumentNullException("StylusPoints");
            StylusPoint tmpPoint = new StylusPoint(double.MaxValue, double.MaxValue);
            foreach (StylusPoint point in StylusPoints)
            {
                if ((point.X < tmpPoint.X) || (point.Y < tmpPoint.Y))
                    tmpPoint = point;
            }
            return tmpPoint.ToPoint();
        }

        Point GetTheFirstPoint()
        {
            return StylusPoints[0].ToPoint();
        }

        Point GetTheLastPoint()
        {
            return StylusPoints[StylusPoints.Count - 1].ToPoint();
        }

        Point GetTheRightBottomPoint()
        {
            if (StylusPoints == null)
                throw new ArgumentNullException("StylusPoints");
            StylusPoint tmpPoint = new StylusPoint(0, 0);
            foreach (StylusPoint point in StylusPoints)
            {
                if ((point.X > tmpPoint.X) || (point.Y > tmpPoint.Y))
                    tmpPoint = point;
            }
            return tmpPoint.ToPoint();
        }
    }
}