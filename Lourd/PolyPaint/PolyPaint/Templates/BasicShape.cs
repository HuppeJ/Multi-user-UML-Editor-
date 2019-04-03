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

        public BasicShape(bool isFromServer, string id, int type, string name, ShapeStyle shapeStyle, List<string> linksTo, List<string> linksFrom)
        {
            this.id = id;
            this.type = type;
            this.name = name;
            this.shapeStyle = shapeStyle.Clone();

            if (isFromServer)
            {
                this.shapeStyle.coordinates.x /= CustomStroke.WIDTH;
                this.shapeStyle.coordinates.y /= CustomStroke.HEIGHT;
                this.shapeStyle.width /= CustomStroke.WIDTH;
                this.shapeStyle.height /= CustomStroke.HEIGHT;
            }
            else
            {
                this.shapeStyle.coordinates.x *= CustomStroke.WIDTH;
                this.shapeStyle.coordinates.y *= CustomStroke.HEIGHT;
                this.shapeStyle.width *= CustomStroke.WIDTH;
                this.shapeStyle.height *= CustomStroke.HEIGHT;
            }

            this.linksTo = linksTo;
            this.linksFrom = linksFrom;
        }

    }
}