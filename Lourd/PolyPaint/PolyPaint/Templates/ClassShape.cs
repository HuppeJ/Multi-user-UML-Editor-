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

        public ClassShape()
        {
        }

        public ClassShape(string id, int type, string name, ShapeStyle shapeStyle, List<string> links, List<string> attributes, List<string> methods)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.shapeStyle = shapeStyle;
            this.links = links;
            this.attributes = attributes;
            this.methods = methods;
        }

    }
}