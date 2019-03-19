using System.Windows.Input;
using System;
using PolyPaint.Templates;
using System.Collections.Generic;
using PolyPaint.Enums;
using System.Windows;
using System.Windows.Ink;
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
            from.SetDefaults();
            to = new AnchorPoint();
            to.SetDefaults();
            type = (int)StrokeTypes.LINK;
            style = new LinkStyle();
            style.SetDefaults();
            path = new List<Coordinates>();

            StylusPoint firstPoint = pts[0];
            StylusPoint lastPoint = pts[pts.Count - 1];
            // garder uniquement le premier point
            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(1);
            }
            path.Add(new Coordinates(StylusPoints[0].ToPoint()));
            AddAssociationArrow(firstPoint, lastPoint);

            StylusPoints.Add(new StylusPoint(lastPoint.X, lastPoint.Y));
            path.Add(new Coordinates(lastPoint.X, lastPoint.Y));


            //xStep = (actualArrowPoint1.X - firstPoint.X) / 10;
            //yStep = (actualArrowPoint1.Y - firstPoint.Y) / 10;


            //for (int i = 1; i < 10; i++)
            //{
            //    StylusPoints.Add(new StylusPoint(firstPoint.X + i * xStep, firstPoint.Y + i * yStep));
            //}

            //for (double i = firstPoint.X; i <= actualArrowPoint1.X; i+=1)
            //{
            //    for (double j = firstPoint.Y; i <= actualArrowPoint1.Y; j += 1)
            //    {
            //        StylusPoints.Add(new StylusPoint(i, j));
            //        //StylusPoints.Add(new StylusPoint(firstPoint.X + i * xStep, firstPoint.Y + i * yStep));
            //    }
            //}

        }

        private void AddAssociationArrow(Coordinates firstPoint, Coordinates lastPoint)
        {
            double deltaY = lastPoint.y - firstPoint.y;
            double deltaX = lastPoint.x - firstPoint.x;

            double norm = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            Vector uVector = new Vector(deltaX / norm, deltaY / norm);
            Point pointOnStroke = Point.Add(firstPoint.ToPoint(), 10 * uVector);
            Point arrowPoint1 = rotatePoint(pointOnStroke.X - firstPoint.x, pointOnStroke.Y - firstPoint.y, 45);
            Point arrowPoint2 = rotatePoint(pointOnStroke.X - firstPoint.x, pointOnStroke.Y - firstPoint.y, -45);

            Point actualArrowPoint1 = new Point(arrowPoint1.X + firstPoint.x, arrowPoint1.Y + firstPoint.y);
            Point actualArrowPoint2 = new Point(arrowPoint2.X + firstPoint.x, arrowPoint2.Y + firstPoint.y);

            StylusPoints.Add(new StylusPoint(actualArrowPoint1.X, actualArrowPoint1.Y));
            StylusPoints.Add(new StylusPoint(path[0].x, path[0].y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint2.X, actualArrowPoint2.Y));
            StylusPoints.Add(new StylusPoint(path[0].x, path[0].y));
        }

        private void AddAssociationArrow(StylusPoint firstPoint, StylusPoint lastPoint)
        {
            double deltaY = lastPoint.Y - firstPoint.Y;
            double deltaX = lastPoint.X - firstPoint.X;

            double norm = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            Vector uVector = new Vector(deltaX / norm, deltaY / norm);
            Point pointOnStroke = Point.Add(firstPoint.ToPoint(), 10 * uVector);
            Point arrowPoint1 = rotatePoint(pointOnStroke.X - firstPoint.X, pointOnStroke.Y - firstPoint.Y, 45);
            Point arrowPoint2 = rotatePoint(pointOnStroke.X - firstPoint.X, pointOnStroke.Y - firstPoint.Y, -45);

            Point actualArrowPoint1 = new Point(arrowPoint1.X + firstPoint.X, arrowPoint1.Y + firstPoint.Y);
            Point actualArrowPoint2 = new Point(arrowPoint2.X + firstPoint.X, arrowPoint2.Y + firstPoint.Y);

            StylusPoints.Add(new StylusPoint(actualArrowPoint1.X, actualArrowPoint1.Y));
            StylusPoints.Add(StylusPoints[0]);
            StylusPoints.Add(new StylusPoint(actualArrowPoint2.X, actualArrowPoint2.Y));
            StylusPoints.Add(StylusPoints[0]);

        }

        public Point rotatePoint(double x, double y, double rotation)
        {
            double rotationInRad = rotation * Math.PI / 180;
            double cosTheta = Math.Cos(rotationInRad);
            double sinTheta = Math.Sin(rotationInRad);

            return new Point(x * cosTheta - y * sinTheta, x * sinTheta + y * cosTheta);
        }


        public LinkStroke(Point pointFrom, string formId, int anchor, StylusPointCollection stylusPointCollection) : base(stylusPointCollection)
        {
            guid = new Guid();
            name = "";
            from = new AnchorPoint(formId, anchor, "");
            to = new AnchorPoint();
            to.SetDefaults();
            type = (int)StrokeTypes.LINK;
            style = new LinkStyle();
            style.SetDefaults();
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
                StylusPoints.Add(new StylusPoint(path[i].x, path[i].y));
            }

            AddAssociationArrow(path[0], path[path.Count - 1]);

            StylusPoints.Add(new StylusPoint(path[path.Count - 1].x, path[path.Count - 1].y));

            if (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(0);
            }

        }

        

        public void addToPointToLink(Point pointTo, string formId, int anchor)
        {
            path.Add(new Coordinates(pointTo));
            to = new AnchorPoint(formId, anchor, "");

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
            return from?.formId != null || to?.formId != null;
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