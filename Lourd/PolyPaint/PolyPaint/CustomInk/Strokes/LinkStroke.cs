using System.Windows.Input;
using System;
using PolyPaint.Templates;
using System.Collections.Generic;
using PolyPaint.Enums;
using System.Windows;

namespace PolyPaint.CustomInk.Strokes
{
    public class LinkStroke : CustomStroke
    {
        private Link link;

        public AnchorPoint from { get; set; }
        public AnchorPoint to { get; set; }
        public LinkStyle style { get; set; }
        public List<Coordinates> path { get; set; }

        public LinkStroke(string id, string name, AnchorPoint from, AnchorPoint to, int type, LinkStyle style, List<Coordinates> path, StylusPointCollection pts) : base(pts)
        {
            this.guid = new Guid(id);
            this.name = name;
            this.from = from;
            this.to = to;
            this.type = type;
            this.style = style;
            this.path = path;
        }

        public LinkStroke(StylusPointCollection pts) : base(pts)
        {
            guid = Guid.NewGuid();
            name = "Link";
            from = new AnchorPoint();
            to = new AnchorPoint();
            type = (int)StrokeTypes.LINK;
            style = new LinkStyle();
            path = new List<Coordinates> { };

            StylusPoint firstPoint = pts[0];
            StylusPoint lastPoint = pts[pts.Count - 1];
            double y = lastPoint.Y - firstPoint.Y;
            double x = lastPoint.X - firstPoint.X;

            int nbOfPoints = pts.Count;
            double yStep = y / nbOfPoints;
            double xStep = x / nbOfPoints;
            

            // garder uniquement le dernier point
            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(0);
            }
            for (int i = 0; i < nbOfPoints; i++)
            {
                StylusPoints.Add(new StylusPoint(firstPoint.X + i * xStep, firstPoint.Y + i * yStep));
            }

        }

        public LinkStroke(Link link, StylusPointCollection dummyPtsForBaseConstructor) : base(dummyPtsForBaseConstructor)
        {
            guid = Guid.NewGuid();
            createLink(link);
        }

        public LinkStroke(Guid guid, Link existingLink, StylusPointCollection dummyPtsForBaseConstructor) : base(dummyPtsForBaseConstructor)
        {
            this.guid = guid;
            createLink(existingLink);
        }

        private void createLink(Link link)
        {
            name = link.name;
            from = link.from;
            to = link.to;
            type = (int)StrokeTypes.LINK;
            style = link.style;
            path = link.path;

            Point firstPoint = new Point(path[0].x, path[0].y);
            Point lastPoint = new Point(path[path.Count - 1].x, path[path.Count - 1].y);
            double y = lastPoint.Y - firstPoint.Y;
            double x = lastPoint.X - firstPoint.X;

            double nbOfPoints = Math.Floor(Math.Pow(x, 2) + Math.Pow(y, 2));
            double yStep = y / nbOfPoints;
            double xStep = x / nbOfPoints;


            for (int i = 1; i < nbOfPoints; i++)
            {
                StylusPoints.Add(new StylusPoint(firstPoint.X + i * xStep, firstPoint.Y + i * yStep));
            }
            StylusPoints.RemoveAt(0);
        }

        // ON DOIT FAIRE LA ROTATION DUN LINKSTROKE??? gi
        #region rotation
        public override CustomStroke CloneRotated(double rotation)
        {
            LinkStroke newStroke = (LinkStroke)Clone();

            // Changer les bounds? Gi
            //newStroke.GetBounds().Transform(rotation.Value);

            newStroke.rotation = rotation;
            return newStroke;
        }

        //protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        //{
        //    if (drawingContext == null)
        //    {
        //        throw new ArgumentNullException("drawingContext");
        //    }
        //    if (null == drawingAttributes)
        //    {
        //        throw new ArgumentNullException("drawingAttributes");
        //    }
        //    Rect bounds = GetBounds();
        //    double x = (bounds.Right + bounds.Left) / 2;
        //    double y = (bounds.Bottom + bounds.Top) / 2;

        //    TransformGroup transform = new TransformGroup();

        //    transform.Children.Add(new RotateTransform(rotation, x, y));

        //    drawingContext.PushTransform(transform);
        //}
        #endregion
    }
}