using System.Collections.Generic;

namespace PolyPaint.Templates
{
    public class Link
    {
        public string id { get; set; }
        public string name { get; set; }
        public AnchorPoint from { get; set; }
        public AnchorPoint to { get; set; }
        public int type { get; set; }
        public LinkStyle style { get; set; }
        public List<Coordinates> path { get; set; }

        public Link()
        {
        }

        public Link(string id, string name, AnchorPoint from, AnchorPoint to, int type, LinkStyle style, List<Coordinates> path)
        {
            this.id = id;
            this.name = name;
            this.from = from;
            this.to = to;
            this.type = type;
            this.style = style;
            this.path = path;
        }
    }
}