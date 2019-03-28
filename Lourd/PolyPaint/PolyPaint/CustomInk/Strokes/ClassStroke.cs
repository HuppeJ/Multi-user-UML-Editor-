using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System.Collections.Generic;
using System.Globalization;

namespace PolyPaint.CustomInk
{
    public class ClassStroke : ShapeStroke
    {
        public List<string> attributes;
        public List<string> methods;

        private Point topLeft;
        private Point topRight;
        private Point middleLeft1;
        private Point middleRight1;
        private Point middleLeft2;
        private Point middleRight2;
        private Point bottomRight;
        private Point bottomLeft;

        public static readonly int WIDTH = 150;
        public static readonly int HEIGHT = 30;
        public override Stroke Clone()
        {
            Stroke stroke = base.Clone();
            (stroke as ClassStroke).attributes = new List<string>();
            (stroke as ClassStroke).attributes.AddRange(attributes);
            (stroke as ClassStroke).methods = new List<string>();
            (stroke as ClassStroke).methods.AddRange(methods);

            return stroke;
        }

        public ClassStroke(StylusPointCollection pts) : base(pts)
        {
            strokeType = (int)StrokeTypes.CLASS_SHAPE;
            attributes = new List<string>();
            methods = new List<string>();
        }

        public ClassStroke(ClassShape classShape, StylusPointCollection pts) : base(pts, classShape)
        {
            attributes = classShape.attributes;
            methods = classShape.methods;
        }

        public virtual ClassShape GetClassShape()
        {
            return new ClassShape(guid.ToString(), strokeType, name, shapeStyle, linksTo, linksFrom, attributes, methods);
        }

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            base.DrawCore(drawingContext, drawingAttributes);

            UpdateShapePoints();

            drawingContext.DrawRectangle(fillColor, pen, new Rect(topLeft, bottomRight));

            drawingContext.DrawLine(pen, middleLeft1, middleRight1);

            drawingContext.DrawLine(pen, middleLeft2, middleRight2);

            FormattedText title = new FormattedText(name, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface("Arial"), 12, Brushes.Black);

            title.MaxTextWidth = shapeStyle.width * WIDTH;
            title.MaxLineCount = 1;
            title.Trimming = TextTrimming.CharacterEllipsis;

            drawingContext.DrawText(title, topLeft);

            string attributesStr = "";
            foreach (string attribute in attributes)
            {
                attributesStr += attribute;
                attributesStr += '\n';
            }

            FormattedText attributesText = new FormattedText(attributesStr, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface("Arial"), 12, Brushes.Black);

            attributesText.MaxTextWidth = shapeStyle.width * WIDTH;
            attributesText.MaxTextHeight = (shapeStyle.height * HEIGHT - 20) / 2;
            attributesText.Trimming = TextTrimming.CharacterEllipsis;

            drawingContext.DrawText(attributesText, middleLeft1);

            string methodsStr = "";
            foreach (string method in methods)
            {
                methodsStr += method;
                methodsStr += '\n';
            }

            FormattedText methodsText = new FormattedText(methodsStr, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface("Arial"), 12, Brushes.Black);

            methodsText.MaxTextWidth = shapeStyle.width * WIDTH;
            methodsText.MaxTextHeight = (shapeStyle.height * HEIGHT - 20) / 2;
            methodsText.Trimming = TextTrimming.CharacterEllipsis;

            drawingContext.DrawText(methodsText, middleLeft2);
        }

        public override BasicShape GetBasicShape()
        {
            return new ClassShape(guid.ToString(), strokeType, name, shapeStyle, linksTo, linksFrom, attributes, methods);
        }

        public override Point GetCenter()
        {
            Rect rect = GetCustomBound();
            return new Point(rect.X + shapeStyle.width * WIDTH / 2, rect.Y + shapeStyle.height * HEIGHT / 2);
        }

        public override Rect GetBounds()
        {
            double width = shapeStyle.width * WIDTH;
            double height = shapeStyle.height * HEIGHT;

            Rect rect = new Rect(shapeStyle.coordinates.x, shapeStyle.coordinates.y,
                width, height);

            RotateTransform rotationTransform = new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y);
            rect.Transform(rotationTransform.Value);

            return rect;
        }

        public override Rect GetCustomBound()
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
            if (shapeStyle.height < 0.2)
                shapeStyle.height = 0.2;

            double width = shapeStyle.width * WIDTH;
            double height = shapeStyle.height * HEIGHT;

            topLeft = shapeStyle.coordinates.ToPoint();

            topRight = new Point(topLeft.X + width, topLeft.Y);

            middleLeft1 = new Point(topLeft.X, topLeft.Y + 20);

            middleRight1 = new Point(topLeft.X + width, topLeft.Y + 20);

            middleLeft2 = new Point(topLeft.X, topLeft.Y + 20 + (height - 20) / 2);

            middleRight2 = new Point(topLeft.X + width, topLeft.Y + 20 + (height - 20) / 2);

            bottomLeft = new Point(topLeft.X, topLeft.Y + height);

            bottomRight = new Point(topLeft.X + width, topLeft.Y + height);
        }
    }
}