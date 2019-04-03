using PolyPaint.CustomInk;
using System.Collections.Generic;

namespace PolyPaint.Templates
{
    public class BasicShape
    {
        public string id { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public ShapeStyle shapeStyle { get; set; }
        public List<string> linksTo { get; set; }
        public List<string> linksFrom { get; set; }

        public BasicShape()
        {
        }

        public BasicShape(string id, int type, string name, ShapeStyle shapeStyle, List<string> linksTo, List<string> linksFrom)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.shapeStyle = shapeStyle.Clone();
            this.linksTo = linksTo;
            this.linksFrom = linksFrom;
        }

        public BasicShape forServer()
        {
            shapeStyle.width *= 2.1;
            shapeStyle.height *= 2.1;
            shapeStyle.coordinates.x *= 2.1;
            shapeStyle.coordinates.y *= 2.1;
            return this;
        }

    }
}