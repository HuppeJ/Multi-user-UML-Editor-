using System.Windows.Input;
using System;
using PolyPaint.Templates;
using System.Collections.Generic;
using PolyPaint.Enums;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

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
        public LinkStroke(Link link, StylusPointCollection pts) : base(pts)
        {
            guid = new Guid(link.id);
            this.name = link.name;
            this.from = link.from;
            this.to = link.to;
            this.strokeType = (int)StrokeTypes.LINK;
            this.linkType = link.type;
            this.style = link.style;
            this.path = link.path;
            rotation = 0;

            // dotted
            if (style.type == 1)
            {
                DrawingAttributes.Color = Colors.Transparent;
            }
            else // normal line
            {
                DrawingAttributes.Color = (Color)ColorConverter.ConvertFromString(style.color);
            }

            switch (style.thickness)
            {
                case 0:
                    DrawingAttributes.Width = getThickness();
                    DrawingAttributes.Height = getThickness();
                    break;
                case 1:
                    DrawingAttributes.Width = getThickness();
                    DrawingAttributes.Height = getThickness();
                    break;
                case 2:
                    DrawingAttributes.Width = getThickness();
                    DrawingAttributes.Height = getThickness();
                    break;
                default:
                    DrawingAttributes.Width = 2;
                    DrawingAttributes.Height = 2;
                    break;
            }

            addStylusPointsToLink();
        }

        public LinkStroke(LinkStroke linkStroke, StylusPointCollection pts) : base(pts)
        {
            guid = Guid.NewGuid();
            name = linkStroke.name;
            from = new AnchorPoint(linkStroke.from);
            to = new AnchorPoint(linkStroke.to);
            strokeType = linkStroke.strokeType;
            linkType = linkStroke.linkType;
            style = new LinkStyle(linkStroke.style);
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
            linkType = (int)LinkTypes.TWO_WAY_ASSOCIATION;
            style = new LinkStyle();
            style.SetDefaults();
            path = new List<Coordinates>();

            DrawingAttributes.Width = getThickness();
            DrawingAttributes.Height = getThickness();

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

            addStylusPointsToLink();
        }


        public LinkStroke(Point pointFrom, string formId, int anchor, LinkTypes linkType, StylusPointCollection stylusPointCollection) : base(stylusPointCollection)
        {
            guid = Guid.NewGuid();
            name = "Link";
            this.linkType = (int)linkType;

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

        public bool isLinkAggCompHeritage()
        {
            return (LinkTypes)linkType == LinkTypes.HERITAGE ||
                (LinkTypes)linkType == LinkTypes.AGGREGATION || (LinkTypes)linkType == LinkTypes.COMPOSITION;
        }
               
        public int getThickness()
        {
            int thickness = 2;

            switch (style.thickness)
            {
                case 0:
                    thickness = 2;
                    break;
                case 1:
                    thickness = 4;
                    break;
                case 2:
                    thickness = 6;
                    break;
                default:
                    thickness = 2;
                    break;
            }

            return thickness;
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
                    StylusPoints.Add(new StylusPoint(path[path.Count - 1].x, path[path.Count - 1].y));
                    AddAssociationArrow(path[path.Count - 1], path[path.Count - 2], path.Count - 1);
                    break;
                case LinkTypes.TWO_WAY_ASSOCIATION:
                    AddTwoWayAssociationArrow();
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

        private void AddTwoWayAssociationArrow()
        {
            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(1);
            }
            // on the from point
            StylusPoints.Add(new StylusPoint(path[0].x, path[0].y));
            AddAssociationArrow(path[0], path[1], 0);

            for (int i = 1; i < path.Count; i++)
            {
                StylusPoints.Add(new StylusPoint(path[i].x, path[i].y));
            }

            //on the to point
            AddAssociationArrow(path[path.Count - 1], path[path.Count - 2], path.Count - 1);
        }

        #region AddArrow functions
        private void AddAssociationArrow(Coordinates firstPoint, Coordinates lastPoint, int pathIndex)
        {
            Point pointOnStroke = GetPointForArrow(firstPoint, lastPoint, 15);

            Point actualArrowPoint1 = rotatePointAroundPoint(pointOnStroke, firstPoint.ToPoint(), 45);
            Point actualArrowPoint2 = rotatePointAroundPoint(pointOnStroke, firstPoint.ToPoint(), -45);

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
            norm = norm == 0 ? 1 : norm;
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
            LineGeometry pathGeom;

            for (int i = 0; i < path.Count - 1 && index == 0; i++)
            {
                pathGeom = new LineGeometry(path[i].ToPoint(), path[i+1].ToPoint());

                double tolerance = 2;
                tolerance += getThickness();

                if (pathGeom.FillContains(point, tolerance, ToleranceType.Absolute))
                {
                    index = i + 1;
                }
            }


            return index;
        }

        internal bool ContainsPoint(Point point)
        {
            if(style.thickness == 0)
            {
                return GetGeometry().FillContains(point, 2, ToleranceType.Absolute);
            }
            return GetGeometry().FillContains(point);
        }

        public bool isAttached()
        {
            return from?.formId != null || to?.formId != null;
        }

        public override void updatePosition(Rect newRect)
        {
            double diffX = newRect.X - GetStraightBounds().X;
            double diffY = newRect.Y - GetStraightBounds().Y;

            for(int i=0; i<path.Count; i++)
            {
                path[i].x += diffX;
                path[i].y += diffY;
            }
        }

        public void updatePositionResizeNotAttached(Rect newRect)
        {
            double[] limits = GetMaxAndMin();
            Rect oldRect = GetStraightBounds();
            double widthRatio = oldRect.Width == 0 ? 1 : newRect.Width / oldRect.Width;
            double heightRatio = oldRect.Height == 0 ? 1: newRect.Height / oldRect.Height;
            foreach (Coordinates point in path)
            {
                point.x = newRect.Left + (point.x - oldRect.Left) * widthRatio;
                point.y = newRect.Top + (point.y - oldRect.Top) * heightRatio;
            }
        }

        public virtual Link GetLinkShape()
        {
            AnchorPoint fromForComm = from?.GetForServer();
            AnchorPoint toForComm = to?.GetForServer();
            List<Coordinates> newPath = new List<Coordinates>();
            foreach (Coordinates coords in path)
            {
                newPath.Add(new Coordinates(coords.x * 2.1, coords.y * 2.1));
            }
            return new Link(guid.ToString(), name, fromForComm, toForComm, linkType, style, newPath);
        }

        public override Rect GetStraightBounds()
        {
            double[] bounds = GetMaxAndMin();

            return new Rect(new Point(bounds[(int)LIMITS.MINX], bounds[(int)LIMITS.MINY]), 
                new Point(bounds[(int)LIMITS.MAXX], bounds[(int)LIMITS.MAXY]));
        }

        public override Rect GetBounds()
        {
            return Rect.Empty;
        }

        public double[] GetMaxAndMin()
        {
            double maxX = -999999999;
            double maxY = -999999999;
            double minX = 999999999;
            double minY = 999999999;
            foreach (Coordinates point in path)
            {
                if (point.x < minX)
                    minX = point.x;
                if (point.x > maxX)
                    maxX = point.x;
                if (point.y < minY)
                    minY = point.y;
                if (point.y > maxY)
                    maxY = point.y;
            }
            return new double[] { minX, minY, maxX, maxY};
        }

        private enum LIMITS
        {
            MINX,
            MINY,
            MAXX,
            MAXY
        }

        internal override bool HitTestPoint(Point point)
        {
            return GetGeometry().FillContains(point, 3, ToleranceType.Absolute);
        }

        internal override bool HitTestPointIncludingEdition(Point point)
        {
            Rect bounds = GetEditingBounds();
            return bounds.Contains(point);
        }

        public override Rect GetEditingBounds()
        {
            Rect bounds = GetStraightBounds();
            double minX = Math.Min(Math.Min(Math.Min(bounds.TopLeft.X, bounds.TopRight.X), bounds.BottomLeft.X), bounds.BottomRight.X);
            double maxX = Math.Max(Math.Max(Math.Max(bounds.TopLeft.X, bounds.TopRight.X), bounds.BottomLeft.X), bounds.BottomRight.X);
            double minY = Math.Min(Math.Min(Math.Min(bounds.TopLeft.Y, bounds.TopRight.Y), bounds.BottomLeft.Y), bounds.BottomRight.Y);
            double maxY = Math.Max(Math.Max(Math.Max(bounds.TopLeft.Y, bounds.TopRight.Y), bounds.BottomLeft.Y), bounds.BottomRight.Y);

            bounds = new Rect(new Point(minX - 15, minY - 15), new Point(maxX + 15, maxY + 15));
            return bounds;
        }

        public override Point GetCenter()
        {
            Rect strokeBounds = GetStraightBounds();

            Point leftTopPoint = GetTheLeftTopPoint();
            Point rightBottomPoint = GetTheRightBottomPoint();
            Point center = new Point(strokeBounds.X + strokeBounds.Width / 2, strokeBounds.Y + strokeBounds.Height / 2);
            //new Point((leftTopPoint.X + rightBottomPoint.X) /2, (leftTopPoint.Y + rightBottomPoint.Y) / 2);

            return center;
        }

        public override Rect GetCustomBound()
        {
            return GetStraightBounds();
        }
    }
}