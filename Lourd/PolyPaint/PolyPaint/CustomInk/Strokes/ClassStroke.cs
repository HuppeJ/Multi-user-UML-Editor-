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
    public class ClassStroke : CustomStroke
    {
        public List<string> attributes;
        public List<string> methods;

        public ClassStroke(StylusPointCollection pts) : base(pts)
        {
            type = (int)StrokeTypes.CLASS_SHAPE;
            attributes = new List<string>();
            methods = new List<string>();
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

            transform.Children.Add(new RotateTransform(rotation, x, y));

            drawingContext.PushTransform(transform);
        }

        public override BasicShape GetBasicShape()
        {
            return new ClassShape(guid.ToString(), type, name, shapeStyle, linksTo, linksFrom, attributes, methods);
        }
    }
}