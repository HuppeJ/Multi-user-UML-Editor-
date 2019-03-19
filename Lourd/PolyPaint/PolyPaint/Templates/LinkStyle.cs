namespace PolyPaint.Templates
{
    public class LinkStyle
    {
        public string color { get; set; }
        public int thickness { get; set; }
        public int type { get; set; }
        public int multiplicityFrom { get; set; }
        public int multiplicityTo { get; set; }

        public LinkStyle()
        {
        }

        public LinkStyle(string color, int thickness, int type, int multiplicityFrom, int multiplicityTo)
        {
            this.color = color;
            this.thickness = thickness;
            this.type = type;
            this.multiplicityFrom = multiplicityFrom;
            this.multiplicityTo = multiplicityTo;
        }
    }
}