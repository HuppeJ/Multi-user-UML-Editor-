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
    public class CommentStroke : ShapeStroke
    {
        private Point topLeft;
        private Point topRight;
        private Point bottomRight;
        private Point bottomLeft;

        private const int WIDTH = 50;
        private const int HEIGHT = 50;

        public CommentStroke(StylusPointCollection pts) : base(pts)
        {
            shapeStyle.coordinates = new Coordinates(pts[pts.Count - 1].ToPoint());

            UpdateShapePoints();

            strokeType = (int)StrokeTypes.COMMENT;
        }

        public CommentStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts, basicShape)
        {

        }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);

            UpdateShapePoints();
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
            if (shapeStyle.height < 0.2)
                shapeStyle.height = 0.2;

            double width = shapeStyle.width * WIDTH;
            double height = shapeStyle.height * HEIGHT;

            topLeft = shapeStyle.coordinates.ToPoint();

            topRight = new Point(topLeft.X + width, topLeft.Y);

            bottomLeft = new Point(topLeft.X, topLeft.Y + height);

            bottomRight = new Point(topLeft.X + width, topLeft.Y + height);
        }

        public override Point GetCenter()
        {
            Rect rect = GetBounds();
            return new Point(rect.X + shapeStyle.width * WIDTH / 2, rect.Y + shapeStyle.height * HEIGHT / 2);
        }
    }
}