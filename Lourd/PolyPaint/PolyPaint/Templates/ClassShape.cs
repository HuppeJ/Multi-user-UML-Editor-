using System.Collections.Generic;

namespace PolyPaint.Templates
{
    class ClassShape
    {
        public string id { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public ShapeStyle shapeStyle { get; set; }
        public List<string> links { get; set; }
        public List<string> attributes { get; set; }
        public List<string> methods { get; set; }

    }
}