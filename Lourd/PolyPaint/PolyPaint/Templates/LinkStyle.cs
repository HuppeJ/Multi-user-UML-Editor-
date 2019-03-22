namespace PolyPaint.Templates
{
    public class LinkStyle
    {
        public string color { get; set; }
        public int thickness { get; set; }
        public int type { get; set; }

        public LinkStyle()
        {
        }

        public LinkStyle(string color, int thickness, int type, string multiplicityFrom, string multiplicityTo)
        {
            this.color = color;
            this.thickness = thickness;
            this.type = type;
        }

        public void SetDefaults()
        {
            color = "#FF000000";
            thickness = 2;
            type = 0; //(int)StrokeTypes.LINK;
        }
    }
}