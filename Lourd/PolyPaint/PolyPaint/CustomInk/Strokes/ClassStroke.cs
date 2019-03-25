using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Globalization;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System.Collections.Generic;

namespace PolyPaint.CustomInk
{
    public class ClassStroke : ShapeStroke
    {
        public List<string> attributes;
        public List<string> methods;

        public static readonly int WIDTH = 150;
        public static readonly int HEIGHT = 30;

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
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }
            if (null == drawingAttributes)
            {
                throw new ArgumentNullException("drawingAttributes");
            }
            DrawingAttributes originalDa = drawingAttributes.Clone();

            Rect bounds = GetBounds();
            double x = (bounds.Right + bounds.Left) / 2;
            double y = (bounds.Bottom + bounds.Top) / 2;

            TransformGroup transform = new TransformGroup();

            transform.Children.Add(new RotateTransform(shapeStyle.rotation, x, y));

            drawingContext.PushTransform(transform);
        }

        public override BasicShape GetBasicShape()
        {
            return new ClassShape(guid.ToString(), strokeType, name, shapeStyle, linksTo, linksFrom, attributes, methods);
        }

        public override Point GetCenter()
        {
            Rect rect = GetBounds();
            return new Point(rect.X + shapeStyle.width * WIDTH / 2, rect.Y + shapeStyle.height * HEIGHT / 2);
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
    }
}