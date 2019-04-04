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
    public class ActorStroke : ShapeStroke
    {
        private double headRadiusX;
        private double headRadiusY;
        private Point headCenter;
        private Point neck;
        private Point torso;
        private Point leftHand;
        private Point rightHand;
        private Point hip;
        private Point leftFoot;
        private Point rightFoot;

        public ActorStroke(StylusPointCollection pts) : base(pts)
        {
            shapeStyle.width = 80;
            shapeStyle.height = 80;
            shapeStyle.coordinates = new Coordinates(pts[pts.Count - 1].ToPoint());

            UpdateShapePoints();

            strokeType = (int)StrokeTypes.ROLE;
        }

        public ActorStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts, basicShape)
        {
            
        }
        
        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);
            UpdateShapePoints();

            /*
            Rect editionBorder = new Rect(shapeStyle.coordinates.x - 15, shapeStyle.coordinates.y - 15,
                 shapeStyle.width + 30, shapeStyle.height + 30);
            */

            // drawingContext.DrawRectangle(null, pen2, editionBorder);

            drawingContext.DrawEllipse(fillColor, pen, headCenter, headRadiusX, headRadiusY);
            drawingContext.DrawLine(pen, neck, hip);
            drawingContext.DrawLine(pen, torso, leftHand);
            drawingContext.DrawLine(pen, torso, rightHand);
            drawingContext.DrawLine(pen, hip, leftFoot);
            drawingContext.DrawLine(pen, hip, rightFoot);

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

            headRadiusX = width / 2;
            headRadiusY = height / 6;

            neck = shapeStyle.coordinates.ToPoint();
            neck.Offset(width / 2, headRadiusY * 2);

            torso = new Point(neck.X, neck.Y + headRadiusY);

            leftHand = new Point(neck.X - width / 2, neck.Y + headRadiusY / 2);

            rightHand = new Point(neck.X + width / 2, neck.Y + headRadiusY / 2);

            hip = new Point(neck.X, neck.Y + headRadiusY * 3);

            leftFoot = new Point(neck.X - width / 2, neck.Y + headRadiusY * 4);

            rightFoot = new Point(neck.X + width / 2, neck.Y + headRadiusY * 4);

            headCenter = new Point(neck.X, neck.Y - headRadiusY);
        }

        public override Point GetCenter()
        {
            Rect rect = GetCustomBound();
            return new Point (rect.X + shapeStyle.width / 2, rect.Y + shapeStyle.height / 2);
        }
    }
}