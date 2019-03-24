using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using PolyPaint.Enums;
using PolyPaint.Templates;

namespace PolyPaint.CustomInk
{
    public class ArtifactStroke : ShapeStroke
    {
        private Point topLeft;
        private Point topRightUp;
        private Point topRightDown;
        private Point topRightInside;
        private Point bottomRight;
        private Point bottomLeft;
        private Point line1Left;
        private Point line1Right;
        private Point line2Left;
        private Point line2Right;
        private Point line3Left;
        private Point line3Right;

        private const int WIDTH = 40;
        private const int HEIGHT = 50;

        public ArtifactStroke(StylusPointCollection pts) : base(pts)
        {
            shapeStyle.coordinates = new Coordinates(pts[pts.Count - 1].ToPoint());

            UpdateShapePoints();

            strokeType = (int)StrokeTypes.ROLE;
        }

        public ArtifactStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts, basicShape)
        {

        }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);
            UpdateShapePoints();
            
            LineSegment top = new LineSegment(topRightUp, true);
            LineSegment diag = new LineSegment(topRightDown, true);
            LineSegment right = new LineSegment(bottomRight, true);
            LineSegment bottom = new LineSegment(bottomLeft, true);
            PathSegmentCollection segments = new PathSegmentCollection { top, diag, right, bottom };
            PathFigure figure = new PathFigure();
            figure.Segments = segments;
            figure.IsClosed = true;
            figure.StartPoint = topLeft;
            PathFigureCollection figures = new PathFigureCollection { figure };
            PathGeometry geometry = new PathGeometry();
            geometry.Figures = figures;

            drawingContext.DrawGeometry(fillColor, pen, geometry);

            drawingContext.DrawLine(pen, topRightUp, topRightInside);
            drawingContext.DrawLine(pen, topRightInside, topRightDown);

            drawingContext.DrawLine(pen, line1Left, line1Right);
            drawingContext.DrawLine(pen, line2Left, line2Right);
            drawingContext.DrawLine(pen, line3Left, line3Right);
        }

        public override Rect GetBounds()
        {
            double width = shapeStyle.width * WIDTH;
            double height = shapeStyle.height * HEIGHT;

            Rect rect = new Rect(shapeStyle.coordinates.x, shapeStyle.coordinates.y,
                width, height);

            return rect;
        }

        internal override bool HitTestPoint(Point point)
        {
            RotateTransform rotationTransform = new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y);
            return GetBounds().Contains(rotationTransform.Inverse.Transform(point));
        }

        internal override bool HitTestPointIncludingEdition(Point point)
        {
            Rect editionBorder = new Rect(shapeStyle.coordinates.x - 15, shapeStyle.coordinates.y - 15,
                WIDTH * shapeStyle.width + 30, HEIGHT * shapeStyle.height + 30);
            RotateTransform rotationTransform = new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y);
            return editionBorder.Contains(rotationTransform.Inverse.Transform(point));
        }


        private void UpdateShapePoints()
        {
            double width = shapeStyle.width * WIDTH;
            double height = shapeStyle.height * HEIGHT;

            topLeft = shapeStyle.coordinates.ToPoint();

            topRightUp = new Point(topLeft.X + width * 0.75, topLeft.Y);

            topRightDown = new Point(topLeft.X + width, topLeft.Y + height * 0.2);

            topRightInside = new Point(topLeft.X + width * 0.75, topLeft.Y + height * 0.2);

            bottomLeft = new Point(topLeft.X, topLeft.Y + height);

            bottomRight = new Point(topLeft.X + width, topLeft.Y + height);

            line1Left = new Point(topLeft.X + width * 0.2, topLeft.Y + height * 0.4);
            line1Right = new Point(topLeft.X + width * 0.8, topLeft.Y + height * 0.4);

            line2Left = new Point(topLeft.X + width * 0.2, topLeft.Y + height * 0.6);
            line2Right = new Point(topLeft.X + width * 0.8, topLeft.Y + height * 0.6);

            line3Left = new Point(topLeft.X + width * 0.2, topLeft.Y + height * 0.8);
            line3Right = new Point(topLeft.X + width * 0.8, topLeft.Y + height * 0.8);
        }

        public override Point GetCenter()
        {
            Rect rect = GetBounds();
            return new Point(rect.X + shapeStyle.width * WIDTH / 2, rect.Y + shapeStyle.height * HEIGHT / 2);
        }
    }
}