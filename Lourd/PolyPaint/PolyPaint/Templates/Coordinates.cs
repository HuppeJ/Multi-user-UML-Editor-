using System.Windows;

namespace PolyPaint.Templates
{
    public class Coordinates
    {
        public double x { get; set; }
        public double y { get; set; }

        public Coordinates(double X, double Y)
        {
            x = X;
            y = Y;
        }

        public Coordinates() { }

        public Coordinates(Point point)
        {
            x = point.X;
            y = point.Y;
        }
        public Point ToPoint()
        {
            return new Point(x, y);
        }
    }
}