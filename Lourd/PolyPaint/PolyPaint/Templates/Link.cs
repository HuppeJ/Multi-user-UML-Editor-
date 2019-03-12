using PolyPaint.CustomInk;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Ink;

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

        public Link(Point pointFrom, string formId, int anchor)
        {
            id = new Guid().ToString();
            name = "";
            from = new AnchorPoint(formId, anchor, "0");
            //to = to;
            type = 0;
            style = new LinkStyle();
            path = new List<Coordinates>();
            path.Add(new Coordinates(pointFrom.X, pointFrom.Y));
        }

        public Point GetFromPoint(StrokeCollection strokes)
        {
            CustomStroke fromStroke;
            Point point = new Point();

            foreach(CustomStroke stroke in strokes)
            {
                if (stroke.guid.ToString() == this.from.formId)
                {
                    fromStroke = stroke;
                    point = fromStroke.GetAnchorPoint(this.from.anchor);
                }
            }

            return point;
        }

        public Point GetToPoint(StrokeCollection strokes)
        {
            CustomStroke fromStroke;
            Point point = new Point();

            foreach (CustomStroke stroke in strokes)
            {
                if (stroke.guid.ToString() == this.to.formId)
                {
                    fromStroke = stroke;
                    point = fromStroke.GetAnchorPoint(this.to.anchor);
                }
            }

            return point;
        }
    }
}