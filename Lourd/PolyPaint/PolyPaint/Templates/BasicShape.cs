using System.Collections.Generic;

namespace PolyPaint.Templates
{
    public class BasicShape
    {
        public string id { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public ShapeStyle shapeStyle { get; set; }
        public List<string> links { get; set; }
    }
}