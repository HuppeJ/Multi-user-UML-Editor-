using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System.Collections.Generic;

namespace PolyPaint.CustomInk
{
    public class ClassStroke : ShapeStroke
    {
        public List<string> attributes;
        public List<string> methods;

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

            shapeStyle.width = 150;
            Point lastPoint = pts[pts.Count - 1].ToPoint();
            for (double i = lastPoint.X; i < shapeStyle.width + lastPoint.X; i += 0.5)
            {
                for (double j = lastPoint.Y; j < shapeStyle.height + lastPoint.Y; j += 0.5)
                {
                    StylusPoints.Add(new StylusPoint(i, j));
                }
            }
        }

        public ClassStroke(ClassShape classShape, StylusPointCollection pts) : base(pts, classShape)
        {
            attributes = classShape.attributes;
            methods = classShape.methods;
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
    }
}