using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System.Globalization;

namespace PolyPaint.CustomInk
{
    public class ActivityStroke : ShapeStroke
    {
        private Point topLeft;
        private Point topRight;
        private Point right;
        private Point left;
        private Point bottomRight;
        private Point bottomLeft;

        public ActivityStroke(StylusPointCollection pts) : base(pts)
        {
            shapeStyle.width = 50;
            shapeStyle.height = 30;
            shapeStyle.coordinates = new Coordinates(pts[pts.Count - 1].ToPoint());

            UpdateShapePoints();

            strokeType = (int)StrokeTypes.ACTIVITY;
        }

        public ActivityStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts, basicShape)
        {

        }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);

            UpdateShapePoints();

            LineSegment topRightSeg = new LineSegment(topRight, true);
            LineSegment rightSeg = new LineSegment(right, true);
            LineSegment bottomRightSeg = new LineSegment(bottomRight, true);
            LineSegment leftSeg = new LineSegment(left, true);
            LineSegment bottomLeftSeg = new LineSegment(bottomLeft, true);
            PathSegmentCollection segments = new PathSegmentCollection { topRightSeg, rightSeg, bottomRightSeg, bottomLeftSeg, leftSeg };
            PathFigure figure = new PathFigure();
            figure.Segments = segments;
            figure.IsClosed = true;
            figure.StartPoint = topLeft;
            PathFigureCollection figures = new PathFigureCollection { figure };
            PathGeometry geometry = new PathGeometry();
            geometry.Figures = figures;

            drawingContext.DrawGeometry(fillColor, pen, geometry);

            FormattedText formattedText = new FormattedText(name, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface("Arial"), 12, Brushes.Black);

            formattedText.MaxTextWidth = shapeStyle.width;
            formattedText.TextAlignment = TextAlignment.Center;
            formattedText.MaxTextHeight = 100;

            drawingContext.DrawText(formattedText, GetCustomBound().BottomLeft);
        }

        public override Rect GetBounds()
        {
            double width = shapeStyle.width;
            double height = shapeStyle.height;

            Rect rect = new Rect(shapeStyle.coordinates.x, shapeStyle.coordinates.y,
                width, height);

            RotateTransform rotationTransform = new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y);
            rect.Transform(rotationTransform.Value);

            return rect;
        }

        public override Rect GetCustomBound()
        {
            double width = shapeStyle.width;
            double height = shapeStyle.height;

            Rect rect = new Rect(shapeStyle.coordinates.x, shapeStyle.coordinates.y,
                width, height);

            return rect;
        }

        internal override bool HitTestPoint(Point point)
        {
            RotateTransform rotationTransform = new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y);
            return GetCustomBound().Contains(rotationTransform.Inverse.Transform(point));
        }

        internal override bool HitTestPointIncludingEdition(Point point)
        {
            Rect bounds = GetEditingBounds();
            return bounds.Contains(point);
        }

        public override Rect GetEditingBounds()
        {
            Rect bounds = GetCustomBound();
            RotateTransform rotationTransform = new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y);
            Point topLeft = rotationTransform.Transform(bounds.TopLeft);
            Point topRight = rotationTransform.Transform(bounds.TopRight);
            Point bottomLeft = rotationTransform.Transform(bounds.BottomLeft);
            Point bottomRight = rotationTransform.Transform(bounds.BottomRight);
            double minX = Math.Min(Math.Min(Math.Min(topLeft.X, topRight.X), bottomLeft.X), bottomRight.X);
            double maxX = Math.Max(Math.Max(Math.Max(topLeft.X, topRight.X), bottomLeft.X), bottomRight.X);
            double minY = Math.Min(Math.Min(Math.Min(topLeft.Y, topRight.Y), bottomLeft.Y), bottomRight.Y);
            double maxY = Math.Max(Math.Max(Math.Max(topLeft.Y, topRight.Y), bottomLeft.Y), bottomRight.Y);

            bounds = new Rect(new Point(minX - 15, minY - 15), new Point(maxX + 15, maxY + 15));
            return bounds;
        }

        private void UpdateShapePoints()
        {
            double width = shapeStyle.width;
            double height = shapeStyle.height;

            topLeft = shapeStyle.coordinates.ToPoint();

            topRight = new Point(topLeft.X + width * 0.68, topLeft.Y);

            right = new Point(topLeft.X + width, topLeft.Y + height * 0.5);

            bottomLeft = new Point(topLeft.X, topLeft.Y + height);

            left = new Point(topLeft.X + width * 0.32, topLeft.Y + height * 0.5);

            bottomRight = new Point(topLeft.X + width * 0.68, topLeft.Y + height);
        }

        public override Point GetCenter()
        {
            Rect rect = GetCustomBound();
            return new Point(rect.X + shapeStyle.width / 2, rect.Y + shapeStyle.height / 2);
        }
    }
}