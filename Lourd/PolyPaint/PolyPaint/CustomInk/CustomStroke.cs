using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;

namespace PolyPaint.CustomInk
{
    public class CustomStroke : Stroke
    {
        public CustomStroke(StylusPointCollection pts) : base(pts)
        {
            StylusPoints = pts;
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
            drawingContext.DrawRectangle(brush2, null, new Rect(GetTheLeftTopPoint(), GetTheRightBottomPoint()));
        }

        Point GetTheLeftTopPoint()
        {
            if (this.StylusPoints == null)
                throw new ArgumentNullException("StylusPoints");
            StylusPoint tmpPoint = new StylusPoint(double.MaxValue, double.MaxValue);
            foreach (StylusPoint point in this.StylusPoints)
            {
                if ((point.X < tmpPoint.X) || (point.Y < tmpPoint.Y))
                    tmpPoint = point;
            }
            return tmpPoint.ToPoint();
        }

        Point GetTheRightBottomPoint()
        {
            if (this.StylusPoints == null)
                throw new ArgumentNullException("StylusPoints");
            StylusPoint tmpPoint = new StylusPoint(0, 0);
            foreach (StylusPoint point in this.StylusPoints)
            {
                if ((point.X > tmpPoint.X) || (point.Y > tmpPoint.Y))
                    tmpPoint = point;
            }
            return tmpPoint.ToPoint();
        }

        //    Brush brush;
        //    Pen pen;

        //    public CustomStroke(StylusPointCollection stylusPoints)
        //        : base(stylusPoints)
        //    {
        //        // Create the Brush and Pen used for drawing.
        //        brush = new LinearGradientBrush(Colors.Red, Colors.Blue, 20d);
        //        pen = new Pen(brush, 2d);
        //    }

        //    protected override void DrawCore(DrawingContext drawingContext,
        //                                     DrawingAttributes drawingAttributes)
        //    {
        //        // Allocate memory to store the previous point to draw from.
        //        Point prevPoint = new Point(double.NegativeInfinity,
        //                                    double.NegativeInfinity);

        //        // Draw linear gradient ellipses between 
        //        // all the StylusPoints in the Stroke.
        //        for (int i = 0; i < this.StylusPoints.Count; i++)
        //        {
        //            Point pt = (Point)this.StylusPoints[i];
        //            Vector v = Point.Subtract(prevPoint, pt);

        //            // Only draw if we are at least 4 units away 
        //            // from the end of the last ellipse. Otherwise, 
        //            // we're just redrawing and wasting cycles.
        //            if (v.Length > 4)
        //            {
        //                // Set the thickness of the stroke 
        //                // based on how hard the user pressed.
        //                double radius = this.StylusPoints[i].PressureFactor * 10d;
        //                drawingContext.DrawEllipse(brush, pen, pt, radius, radius);
        //                prevPoint = pt;
        //            }
        //        }
        //    }
        //}
    }
}