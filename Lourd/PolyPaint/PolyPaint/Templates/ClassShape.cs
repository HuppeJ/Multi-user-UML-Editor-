using PolyPaint.CustomInk;
using System.Collections.Generic;

namespace PolyPaint.Templates
{
    public class ClassShape : BasicShape
    {
        public List<string> attributes { get; set; }
        public List<string> methods { get; set; }

        public ClassShape()
        {
        }

        public ClassShape(string id, int type, string name, ShapeStyle shapeStyle, List<string> linksTo, List<string> linksFrom, List<string> attributes, List<string> methods)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.shapeStyle = shapeStyle.Clone();
            this.shapeStyle.coordinates.x *= CustomStroke.WIDTH;
            this.shapeStyle.coordinates.y *= CustomStroke.HEIGHT;
            this.shapeStyle.width *= CustomStroke.WIDTH;
            this.shapeStyle.height *= CustomStroke.HEIGHT;
            this.linksTo = linksTo;
            this.linksFrom = linksFrom;
            this.attributes = attributes;
            this.methods = methods;
        }

    }
}