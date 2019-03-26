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
        public int linkType { get; set; }
        public double rotation { get; set; }

        #region constructors
        public LinkStroke(string id, string name, AnchorPoint from, AnchorPoint to, int strokeType, int linkType, LinkStyle style, List<Coordinates> path, StylusPointCollection pts) : base(pts)
        {
            guid = new Guid(id);
            this.name = name;
            this.from = from;
            this.to = to;
            this.strokeType = strokeType;
            this.linkType = linkType;
            this.style = style;
            this.path = path;
        }
        
        public LinkStroke(LinkStroke linkStroke, StylusPointCollection pts) : base(pts)
        {
            guid = Guid.NewGuid();
            name = linkStroke.name;
            from = new AnchorPoint();
            from.SetDefaults();
            to = new AnchorPoint();
            to.SetDefaults();
            strokeType = linkStroke.strokeType;
            linkType = linkStroke.linkType;
            style = linkStroke.style;
            path = new List<Coordinates>();
            path.AddRange(linkStroke.path);
            addStylusPointsToLink();
        }

        public LinkStroke(StylusPointCollection pts) : base(pts)
        {
            guid = Guid.NewGuid();
            name = "Link";
            from = new AnchorPoint();
            from.SetDefaults();
            to = new AnchorPoint();
            to.SetDefaults();
            strokeType = (int)StrokeTypes.LINK;
            linkType = (int)LinkTypes.LINE;
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

            if(!lastPoint.Equals(firstPoint))
            {
                StylusPoints.Add(new StylusPoint(lastPoint.X, lastPoint.Y));
                path.Add(new Coordinates(lastPoint.X, lastPoint.Y));
            }
            else
            {
                StylusPoints.Add(new StylusPoint(firstPoint.X + 100, lastPoint.Y + 100));
                path.Add(new Coordinates(StylusPoints[1].X, StylusPoints[1].Y));
            }
        }


        public LinkStroke(Point pointFrom, string formId, int anchor, StylusPointCollection stylusPointCollection) : base(stylusPointCollection)
        {
            guid = Guid.NewGuid();
            name = "";
            from = new AnchorPoint(formId, anchor, "");
            to = new AnchorPoint();
            to.SetDefaults();
            strokeType = (int)StrokeTypes.LINK;
            style = new LinkStyle();
            style.SetDefaults();
            path = new List<Coordinates>();
            path.Add(new Coordinates(pointFrom));
        }
        #endregion

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

            AddRelationArrows();

            // add the last point
            StylusPoints.Add(new StylusPoint(path[path.Count - 1].x, path[path.Count - 1].y));

            // remove the first point, which actually is an invalid point (from before move)
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

        private void AddRelationArrows()
        {
            switch ((LinkTypes)linkType)
            {
                case LinkTypes.LINE:
                    break;
                case LinkTypes.ONE_WAY_ASSOCIATION:
                    //on the to point
                    AddAssociationArrow(path[path.Count - 1], path[path.Count - 2], path.Count - 1);
                    break;
                case LinkTypes.TWO_WAY_ASSOCIATION:
                    // on the from point
                    AddAssociationArrow(path[0], path[1], 0);
                    //on the to point
                    AddAssociationArrow(path[path.Count - 1], path[path.Count - 2], path.Count - 1);
                    break;
                case LinkTypes.HERITAGE:
                    //on the to point
                    AddHeritageArrow(path[path.Count - 1], path[path.Count - 2]); ;
                    break;
                case LinkTypes.AGGREGATION:
                    //on the to point
                    AddAggregationArrow(path[path.Count - 1], path[path.Count - 2]);
                    break;
                case LinkTypes.COMPOSITION:
                    //on the to point
                    AddCompositionArrow(path[path.Count - 1], path[path.Count - 2]);
                    break;
                default:
                    break;
            }
        }

        #region AddArrow functions
        private void AddAssociationArrow(Coordinates firstPoint, Coordinates lastPoint, int pathIndex)
        {
            Point pointOnStroke = GetPointForArrow(firstPoint, lastPoint, 10);

            Point actualArrowPoint1 = rotatePointAroundPoint(pointOnStroke, firstPoint.ToPoint(), 45);
            Point actualArrowPoint2 = rotatePointAroundPoint(pointOnStroke, firstPoint.ToPoint(), -45);

            StylusPoints.Add(new StylusPoint(path[pathIndex].x, path[pathIndex].y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint1.X, actualArrowPoint1.Y));
            StylusPoints.Add(new StylusPoint(path[pathIndex].x, path[pathIndex].y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint2.X, actualArrowPoint2.Y));
            StylusPoints.Add(new StylusPoint(path[pathIndex].x, path[pathIndex].y));
        }

        private static Point GetPointForArrow(Coordinates firstPoint, Coordinates lastPoint, int distanceFromPoint)
        {
            double deltaY = lastPoint.y - firstPoint.y;
            double deltaX = lastPoint.x - firstPoint.x;

            double norm = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            Vector uVector = new Vector(deltaX / norm, deltaY / norm);
            Point pointOnStroke = Point.Add(firstPoint.ToPoint(), distanceFromPoint * uVector);
            return pointOnStroke;
        }

        private void AddHeritageArrow(Coordinates toPoint, Coordinates beforeToPoint)
        {
            Point pointOnStroke = GetPointForArrow(toPoint, beforeToPoint, 10);

            Point arrowPoint1 = rotatePoint(pointOnStroke.X - toPoint.x, pointOnStroke.Y - toPoint.y, 45);
            Point arrowPoint2 = rotatePoint(pointOnStroke.X - toPoint.x, pointOnStroke.Y - toPoint.y, -45);

            double arrowNorm = Math.Cos(Math.PI / 4);

            Point actualArrowPoint1 = new Point(arrowPoint1.X / arrowNorm + toPoint.x, arrowPoint1.Y / arrowNorm + toPoint.y);
            Point actualArrowPoint2 = new Point(arrowPoint2.X / arrowNorm + toPoint.x, arrowPoint2.Y / arrowNorm + toPoint.y);

            StylusPoints.Add(new StylusPoint(pointOnStroke.X, pointOnStroke.Y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint1.X, actualArrowPoint1.Y));
            StylusPoints.Add(new StylusPoint(toPoint.x, toPoint.y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint2.X, actualArrowPoint2.Y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint1.X, actualArrowPoint1.Y));
        }

        private void AddAggregationArrow(Coordinates toPoint, Coordinates beforeToPoint)
        {
            Point pointOnStroke = GetPointForArrow(toPoint, beforeToPoint, 10);
            Point pointBeforeOnStroke = GetPointForArrow(toPoint, beforeToPoint, 20);

            Point arrowPoint1 = rotatePoint(pointOnStroke.X - toPoint.x, pointOnStroke.Y - toPoint.y, 45);
            Point arrowPoint2 = rotatePoint(pointOnStroke.X - toPoint.x, pointOnStroke.Y - toPoint.y, -45);

            double arrowNorm = Math.Cos(Math.PI / 4);

            Point actualArrowPoint1 = new Point(arrowPoint1.X / arrowNorm + toPoint.x, arrowPoint1.Y / arrowNorm + toPoint.y);
            Point actualArrowPoint2 = new Point(arrowPoint2.X / arrowNorm + toPoint.x, arrowPoint2.Y / arrowNorm + toPoint.y);

            StylusPoints.Add(new StylusPoint(pointBeforeOnStroke.X, pointBeforeOnStroke.Y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint1.X, actualArrowPoint1.Y));
            StylusPoints.Add(new StylusPoint(toPoint.x, toPoint.y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint2.X, actualArrowPoint2.Y));
            StylusPoints.Add(new StylusPoint(pointBeforeOnStroke.X, pointBeforeOnStroke.Y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint1.X, actualArrowPoint1.Y));
        }

        private void AddCompositionArrow(Coordinates toPoint, Coordinates beforeToPoint)
        {
            Point pointOnStroke = GetPointForArrow(toPoint, beforeToPoint, 10);
            Point pointBeforeOnStroke = GetPointForArrow(toPoint, beforeToPoint, 20);

            Point arrowPoint1 = rotatePoint(pointOnStroke.X - toPoint.x, pointOnStroke.Y - toPoint.y, 45);
            Point arrowPoint2 = rotatePoint(pointOnStroke.X - toPoint.x, pointOnStroke.Y - toPoint.y, -45);

            double arrowNorm = Math.Cos(Math.PI / 4);

            Point actualArrowPoint1 = new Point(arrowPoint1.X / arrowNorm + toPoint.x, arrowPoint1.Y / arrowNorm + toPoint.y);
            Point actualArrowPoint2 = new Point(arrowPoint2.X / arrowNorm + toPoint.x, arrowPoint2.Y / arrowNorm + toPoint.y);

            double xStep1 = (actualArrowPoint1.X - pointBeforeOnStroke.X) / 10;
            double yStep1 = (actualArrowPoint1.Y - pointBeforeOnStroke.Y) / 10;
            double xStep2 = (actualArrowPoint2.X - pointBeforeOnStroke.X) / 10;
            double yStep2 = (actualArrowPoint2.Y - pointBeforeOnStroke.Y) / 10;


            for (int i = 1; i < 10; i++)
            {
                StylusPoints.Add(new StylusPoint(pointBeforeOnStroke.X + i * xStep1, pointBeforeOnStroke.Y + i * yStep1));
                StylusPoints.Add(new StylusPoint(pointBeforeOnStroke.X + i * xStep2, pointBeforeOnStroke.Y + i * yStep2));
            }

            StylusPoints.Add(new StylusPoint(actualArrowPoint1.X, actualArrowPoint1.Y));
            StylusPoints.Add(new StylusPoint(actualArrowPoint2.X, actualArrowPoint2.Y));

            xStep1 = (toPoint.x - actualArrowPoint1.X) / 10;
            yStep1 = (toPoint.y - actualArrowPoint1.Y) / 10;
            xStep2 = (toPoint.x - actualArrowPoint2.X) / 10;
            yStep2 = (toPoint.y - actualArrowPoint2.Y) / 10;

            for (int i = 1; i < 10; i++)
            {
                StylusPoints.Add(new StylusPoint(actualArrowPoint1.X + i * xStep1, actualArrowPoint1.Y + i * yStep1));
                StylusPoints.Add(new StylusPoint(actualArrowPoint2.X + i * xStep2, actualArrowPoint2.Y + i * yStep2));
            }
        }

        private void AddAssociationArrow(StylusPoint firstPoint, StylusPoint lastPoint)
        {
            Point pointOnStroke = GetPointForArrow(new Coordinates(firstPoint.ToPoint()), new Coordinates(lastPoint.ToPoint()), 10);

            Point actualArrowPoint1 = rotatePointAroundPoint(pointOnStroke, firstPoint.ToPoint(), 45);
            Point actualArrowPoint2 = rotatePointAroundPoint(pointOnStroke, firstPoint.ToPoint(), -45);

            StylusPoints.Add(new StylusPoint(actualArrowPoint1.X, actualArrowPoint1.Y));
            StylusPoints.Add(StylusPoints[0]);
            StylusPoints.Add(new StylusPoint(actualArrowPoint2.X, actualArrowPoint2.Y));
            StylusPoints.Add(StylusPoints[0]);

        }
        #endregion

        public Point rotatePoint(double x, double y, double rotation)
        {
            double rotationInRad = rotation * Math.PI / 180;
            double cosTheta = Math.Cos(rotationInRad);
            double sinTheta = Math.Sin(rotationInRad);

            return new Point(x * cosTheta - y * sinTheta, x * sinTheta + y * cosTheta);
        }

        public Point rotatePointAroundPoint(Point pointToRotate, Point center, double rotation)
        {
            double rotationInRad = rotation * Math.PI / 180;
            double cosTheta = Math.Cos(rotationInRad);
            double sinTheta = Math.Sin(rotationInRad);
            Vector originToCenter = center - new Point(0,0);
            Point pointAtOrigin = pointToRotate - originToCenter;

            Point pointRotated = new Point(pointAtOrigin.X * cosTheta - pointAtOrigin.Y * sinTheta, pointAtOrigin.X * sinTheta + pointAtOrigin.Y * cosTheta);

            return pointRotated + originToCenter;
        }

        public void RotateStroke(double rotationInDegrees)
        {
            Point center = GetCenter();

            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(0);
            }

            for (int i = 0; i < path.Count; i++)
            {
                Coordinates coords = path[i];
                Point rotatedPoint = rotatePointAroundPoint(coords.ToPoint(), center, rotationInDegrees);
                StylusPoints.Add(new StylusPoint(rotatedPoint.X, rotatedPoint.Y));

                path[i] = new Coordinates(rotatedPoint);
            }

            StylusPoints.RemoveAt(0);

            AddRelationArrows();

            rotation += rotationInDegrees;

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

        #region rotation
        /*public override CustomStroke CloneRotated(double rotation)
        {
            LinkStroke newStroke = (LinkStroke)Clone();

            // Changer les bounds? Gi
            //newStroke.GetBounds().Transform(rotation.Value);

            newStroke.rotation = rotation;
            return newStroke;
        }*/

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