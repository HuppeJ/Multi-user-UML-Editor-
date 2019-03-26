namespace PolyPaint.Templates
{
    public class ShapeStyle
    {
        public Coordinates coordinates { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double rotation { get; set; }
        public string borderColor { get; set; }
        public int borderStyle { get; set; }
        public string backgroundColor { get; set; }
        
        public ShapeStyle()
        {
        }

        public ShapeStyle(Coordinates coordinates, double width, double height, double rotation, string borderColor, int borderStyle, string backgroundColor)
        {
            this.coordinates = coordinates;
            this.width = width;
            this.height = height;
            this.rotation = rotation;
            this.borderColor = borderColor;
            this.borderStyle = borderStyle;
            this.backgroundColor = backgroundColor;
        }

        public ShapeStyle Clone()
        {
            return new ShapeStyle(new Coordinates(coordinates.x, coordinates.y), width, height, rotation, borderColor, borderStyle, backgroundColor);
        }
    }
}