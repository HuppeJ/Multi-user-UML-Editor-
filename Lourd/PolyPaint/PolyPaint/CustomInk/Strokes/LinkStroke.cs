using System.Windows.Input;
using System;
using PolyPaint.Templates;
using System.Collections.Generic;
using PolyPaint.Enums;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Shapes;
using System.Windows.Media;

namespace PolyPaint.CustomInk.Strokes
{
    public class LinkStroke : CustomStroke
    {
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
            path = new List<Coordinates>();

            StylusPoint firstPoint = pts[0];
            StylusPoint lastPoint = pts[pts.Count - 1];
            double y = lastPoint.Y - firstPoint.Y;
            double x = lastPoint.X - firstPoint.X;

            int nbOfPoints = pts.Count;
            double yStep = y / nbOfPoints;
            double xStep = x / nbOfPoints;


            // garder uniquement le premier point
            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(1);
            }
            path.Add(new Coordinates(StylusPoints[0].ToPoint()));

            for (int i = 1; i <= nbOfPoints; i++)
            {
                StylusPoints.Add(new StylusPoint(firstPoint.X + i * xStep, firstPoint.Y + i * yStep));
                if (i == nbOfPoints)
                {
                    path.Add(new Coordinates(StylusPoints[nbOfPoints].ToPoint()));
                }
            }


        }

        public LinkStroke(Point pointFrom, string formId, int anchor, StylusPointCollection stylusPointCollection) : base(stylusPointCollection)
        {
            guid = new Guid();
            name = "";
            from = new AnchorPoint(formId, anchor, "0");
            to = new AnchorPoint();
            type = (int)StrokeTypes.LINK;
            style = new LinkStyle();
            path = new List<Coordinates>();
            path.Add(new Coordinates(pointFrom));
        }

        public void addStylusPointsToLink()
        {
            // garder uniquement le premier point
            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(1);
            }

            for (int i = 0; i < path.Count - 1; i++)
            {

                Point firstPoint = new Point(path[i].x, path[i].y);
                Point lastPoint = new Point(path[i + 1].x, path[i + 1].y);
                double y = lastPoint.Y - firstPoint.Y;
                double x = lastPoint.X - firstPoint.X;

                double nbOfPoints = Math.Floor(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                double yStep = y / nbOfPoints;
                double xStep = x / nbOfPoints;

                for (int j = 1; j < nbOfPoints; j++)
                {
                    StylusPoints.Add(new StylusPoint(firstPoint.X + j * xStep, firstPoint.Y + j * yStep));
                }
            }

            if (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(0);
            }
        }

        public void addToPointToLink(Point pointTo, string formId, int anchor)
        {
            path.Add(new Coordinates(pointTo));
            to = new AnchorPoint(formId, anchor, "0");

            addStylusPointsToLink();
        }

        public Point GetFromPoint(StrokeCollection strokes)
        {
            CustomStroke fromStroke;
            Point point = new Point();

            foreach (CustomStroke stroke in strokes)
            {
                if (stroke.guid.ToString() == this.from.formId)
                {
                    fromStroke = stroke;
                    point = (fromStroke as ShapeStroke).GetAnchorPoint(this.from.anchor);
                    //point = (fromStroke as ShapeStroke).anchorPoints[from.anchor];
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
                    point = (fromStroke as ShapeStroke).GetAnchorPoint(this.to.anchor);
                }
            }

            return point;
        }

        internal int GetIndexforNewPoint(Point point)
        {
            int index = 0;
            Rect rect;
            RectangleGeometry rectGeometry = new RectangleGeometry();

            for (int i = 0; i < path.Count - 1 && index == 0; i++)
            {
                rect = new Rect(path[i].ToPoint(), path[i + 1].ToPoint());
                rectGeometry.Rect = rect;
                if (rectGeometry.FillContains(point))
                {
                    index = i + 1;
                }
            }


            return index;
        }

        internal bool ContainsPoint(Point point)
        {
            return GetGeometry().FillContains(point);
        }

        public bool isAttached()
        {
            return from.formId != null || to.formId != null;
        }
        //public bool 

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