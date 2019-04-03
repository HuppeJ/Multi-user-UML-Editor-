using PolyPaint.CustomInk.Strokes;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolyPaint.CustomInk
{
    class DottedPathAdorner : Adorner
    {
        Rect strokeBounds = Rect.Empty;
        private LinkStroke linkStroke;
        private CustomInkCanvas canvas;

        private Path linkPath;
        LineGeometry lineGeom;
        PathGeometry linkPathGeom;
        Polygon polygon = new Polygon();

        Path path = new Path();

        VisualCollection visualChildren;

        // Be sure to call the base class constructor.
        public DottedPathAdorner(UIElement adornedElement, LinkStroke stroke, CustomInkCanvas canvas)
          : base(adornedElement)
        {
            this.linkStroke = stroke;
            this.canvas = canvas;
            visualChildren = new VisualCollection(this);
            strokeBounds = stroke.GetBounds();

            linkPath = new Path();
            linkPath.Stroke = (Brush) new BrushConverter().ConvertFromString(stroke.style.color);
            linkPath.StrokeThickness = stroke.DrawingAttributes.Height;
            linkPath.StrokeDashArray = new DoubleCollection { 1, 1 };
            linkPath.IsHitTestVisible = false;
            linkPath.Fill = Brushes.Black;

            linkPathGeom = new PathGeometry();
            lineGeom = new LineGeometry();

            for (int i = 1; i < linkStroke.path.Count - 1; i++)
            {
                lineGeom.StartPoint = linkStroke.path[i - 1].ToPoint();
                lineGeom.EndPoint = linkStroke.path[i].ToPoint();
                linkPathGeom.AddGeometry(lineGeom);
            }

            AddRelationArrows();
            linkPath.Data = linkPathGeom;

            visualChildren.Add(linkPath);
        }

        /// <summary>
        /// Draw the rotation handle and the outline of
        /// the element.
        /// </summary>
        /// <param name="finalSize">The final area within the 
        /// parent that this element should use to arrange 
        /// itself and its children.</param>
        /// <returns>The actual size used. </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            linkPath.Arrange(new Rect(finalSize));
            polygon.Arrange(new Rect(finalSize));
            return finalSize;
        }

        // Override the VisualChildrenCount and 
        // GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount
        {
            get { return visualChildren.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }

        private void AddRelationArrows()
        {
            switch ((LinkTypes)linkStroke.linkType)
            {
                case LinkTypes.LINE:
                    AddLineGeometry(linkStroke.path[linkStroke.path.Count - 2], linkStroke.path[linkStroke.path.Count - 1]);
                    break;
                case LinkTypes.ONE_WAY_ASSOCIATION:
                    //on the to point
                    AddAssociationArrow(linkStroke.path[linkStroke.path.Count - 1], linkStroke.path[linkStroke.path.Count - 2], linkStroke.path.Count - 1);
                    break;
                case LinkTypes.TWO_WAY_ASSOCIATION:
                    // on the from point
                    AddAssociationArrow(linkStroke.path[0], linkStroke.path[1], 0);
                    //on the to point
                    AddAssociationArrow(linkStroke.path[linkStroke.path.Count - 1], linkStroke.path[linkStroke.path.Count - 2], linkStroke.path.Count - 1);
                    break;
                case LinkTypes.HERITAGE:
                    //on the to point
                    AddHeritageArrow(linkStroke.path[linkStroke.path.Count - 1], linkStroke.path[linkStroke.path.Count - 2]); ;
                    break;
                case LinkTypes.AGGREGATION:
                    //on the to point
                    AddAggregationArrow(linkStroke.path[linkStroke.path.Count - 1], linkStroke.path[linkStroke.path.Count - 2]);
                    break;
                case LinkTypes.COMPOSITION:
                    AddCompositionArrow(linkStroke.path[linkStroke.path.Count - 1], linkStroke.path[linkStroke.path.Count - 2]);
                    break;
                default:
                    break;
            }
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

            AddLineGeometry(beforeToPoint, pointBeforeOnStroke);
            AddLineGeometry(pointBeforeOnStroke, actualArrowPoint1);
            AddLineGeometry(new Coordinates(actualArrowPoint1), toPoint);
            AddLineGeometry(toPoint, actualArrowPoint2);
            AddLineGeometry(actualArrowPoint2, pointBeforeOnStroke);
            
            polygon.Points = new PointCollection { pointBeforeOnStroke, actualArrowPoint1, toPoint.ToPoint(), actualArrowPoint2, pointBeforeOnStroke };
            polygon.Fill = (Brush)new BrushConverter().ConvertFromString(linkStroke.style.color);
            polygon.IsHitTestVisible = false;
            visualChildren.Add(polygon);
        }

        #region AddArrow functions
        private void AddAssociationArrow(Coordinates firstPoint, Coordinates lastPoint, int pathIndex)
        {
            List<Coordinates> path = linkStroke.path;
            Point pointOnStroke = GetPointForArrow(firstPoint, lastPoint, 10);

            Point actualArrowPoint1 = rotatePointAroundPoint(pointOnStroke, firstPoint.ToPoint(), 45);
            Point actualArrowPoint2 = rotatePointAroundPoint(pointOnStroke, firstPoint.ToPoint(), -45);
            if(pathIndex > 0)
            {
                AddLineGeometry(path[pathIndex - 1], path[pathIndex]);
            }

            AddLineGeometry(path[pathIndex], actualArrowPoint1);
            AddLineGeometry(path[pathIndex], actualArrowPoint2);
        }

        private void AddHeritageArrow(Coordinates toPoint, Coordinates beforeToPoint)
        {
            Point pointOnStroke = GetPointForArrow(toPoint, beforeToPoint, 10);

            Point arrowPoint1 = rotatePoint(pointOnStroke.X - toPoint.x, pointOnStroke.Y - toPoint.y, 45);
            Point arrowPoint2 = rotatePoint(pointOnStroke.X - toPoint.x, pointOnStroke.Y - toPoint.y, -45);

            double arrowNorm = Math.Cos(Math.PI / 4);

            Point actualArrowPoint1 = new Point(arrowPoint1.X / arrowNorm + toPoint.x, arrowPoint1.Y / arrowNorm + toPoint.y);
            Point actualArrowPoint2 = new Point(arrowPoint2.X / arrowNorm + toPoint.x, arrowPoint2.Y / arrowNorm + toPoint.y);

            AddLineGeometry(beforeToPoint, pointOnStroke);
            AddLineGeometry(actualArrowPoint2, actualArrowPoint1);
            AddLineGeometry(toPoint, actualArrowPoint2);
            AddLineGeometry(toPoint, actualArrowPoint1);
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
            
            AddLineGeometry(beforeToPoint, pointBeforeOnStroke);
            AddLineGeometry(pointBeforeOnStroke, actualArrowPoint1);
            AddLineGeometry(new Coordinates(actualArrowPoint1), toPoint);
            AddLineGeometry(toPoint, actualArrowPoint2);
            AddLineGeometry(actualArrowPoint2, pointBeforeOnStroke);
        }

        private void AddLineGeometry(Coordinates start, Point end)
        {
            lineGeom.StartPoint = start.ToPoint();
            lineGeom.EndPoint = end;
            linkPathGeom.AddGeometry(lineGeom);
        }

        private void AddLineGeometry(Point start, Point end)
        {
            lineGeom.StartPoint = start;
            lineGeom.EndPoint = end;
            linkPathGeom.AddGeometry(lineGeom);
        }

        private void AddLineGeometry(Coordinates start, Coordinates end)
        {
            lineGeom.StartPoint = start.ToPoint();
            lineGeom.EndPoint = end.ToPoint();
            linkPathGeom.AddGeometry(lineGeom);
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
        #endregion

        #region rotate
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
            Vector originToCenter = center - new Point(0, 0);
            Point pointAtOrigin = pointToRotate - originToCenter;

            Point pointRotated = new Point(pointAtOrigin.X * cosTheta - pointAtOrigin.Y * sinTheta, pointAtOrigin.X * sinTheta + pointAtOrigin.Y * cosTheta);

            return pointRotated + originToCenter;
        }
        #endregion
    }
}
