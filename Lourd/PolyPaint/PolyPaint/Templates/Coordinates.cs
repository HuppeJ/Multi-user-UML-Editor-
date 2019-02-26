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
    }
}