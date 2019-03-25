using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Globalization;
using PolyPaint.Templates;
using System.Collections.Generic;
using PolyPaint.Enums;

namespace PolyPaint.CustomInk
{
    public abstract class ShapeStroke : CustomStroke
    {
        public ShapeStyle shapeStyle;
        public List<string> linksTo;
        public List<string> linksFrom;
        public List<Point> anchorPoints;
        protected Pen pen;
        protected SolidColorBrush fillColor;

        public ShapeStroke(StylusPointCollection pts) : base(pts)
        {
            guid = Guid.NewGuid();
            name = "This is a stroke";

            Point lastPoint = pts[pts.Count - 1].ToPoint();
            Coordinates coordinates = new Coordinates(lastPoint.X, lastPoint.Y);

            shapeStyle = new ShapeStyle(coordinates,1,1,0, "#000000", 0, null);

            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(0);
            }
            
            linksTo = new List<string>();
            linksFrom = new List<string>();
        }

        
        public ShapeStroke(StylusPointCollection pts, BasicShape basicShape) : base(pts)
        {
            guid = Guid.Parse(basicShape.id);
            name = basicShape.name;
            strokeType = basicShape.type;
            shapeStyle = basicShape.shapeStyle;
            linksTo = new List<string>();
            linksFrom = new List<string>();

            Point point = new Point(shapeStyle.coordinates.x, shapeStyle.coordinates.y);

            if(basicShape.type != (int)StrokeTypes.PHASE)
            {
                for (double i = point.X; i < shapeStyle.width + point.X; i += 0.5)
                {
                    for (double j = point.Y; j < shapeStyle.height + point.Y; j += 0.5)
                    {
                        StylusPoints.Add(new StylusPoint(i, j));
                    }
                }
            }

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

            SolidColorBrush borderColor;
            if (shapeStyle.borderColor != null)
                borderColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(shapeStyle.borderColor));
            else
                borderColor = null;
            
            if (shapeStyle.backgroundColor != null)
                fillColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(shapeStyle.backgroundColor));
            else
                fillColor = null;

            // TODO: add border weight
            pen = new Pen(borderColor, 1);

            // TODO: add border style
            // pen.DashStyle = DashStyles.Dash;

            TransformGroup transform = new TransformGroup();

            transform.Children.Add(new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y));

            // drawingContext.DrawRectangle(null, pen2, GetBounds());
            drawingContext.PushTransform(transform);
        }

        public Point GetAnchorPoint(int anchorNumber)
        {
            double xCenter = GetCenter().X;
            double yCenter = GetCenter().Y;
            double margin = 10;
            double halfWidth = GetBounds().Width / 2 + margin;
            double halfHeight = GetBounds().Height / 2 + margin;

            Point pointRotatedAroundOrigin;

            switch (anchorNumber)
            {
                case 0:
                    pointRotatedAroundOrigin = rotatePoint(0, halfHeight);
                    break;
                case 1:
                    pointRotatedAroundOrigin = rotatePoint(-halfWidth, 0);
                    break;
                case 2:
                    pointRotatedAroundOrigin = rotatePoint(0, -halfHeight);
                    break;
                default:
                    pointRotatedAroundOrigin = rotatePoint(halfWidth, 0);
                    break;
            }

            return new Point(xCenter - pointRotatedAroundOrigin.X, yCenter - pointRotatedAroundOrigin.Y);
        }
        
        public virtual BasicShape GetBasicShape()
        {
            BasicShape basicShape = new BasicShape(guid.ToString(), strokeType, name, shapeStyle, linksTo, linksFrom);
            return basicShape;
        }

        public override void updatePosition(Rect newRect)
        {
            double diffX = newRect.X - GetBounds().X;
            double diffY = newRect.Y - GetBounds().Y;
            shapeStyle.coordinates.x += diffX;
            shapeStyle.coordinates.y += diffY;
        }

        public override void updateLinks()
        {
            foreach (string link in linksFrom)
            {

            }
        }

        public override CustomStroke CloneRotated(double rotation)
        {
            ShapeStroke newStroke = (ShapeStroke)Clone();

            newStroke.shapeStyle.rotation += rotation;
            return newStroke;
        }

        private Point rotatePoint(double x, double y)
        {
            double rotationInRad = shapeStyle.rotation * Math.PI / 180;
            double cosTheta = Math.Cos(rotationInRad);
            double sinTheta = Math.Sin(rotationInRad);

            return new Point(x * cosTheta - y * sinTheta, x * sinTheta + y * cosTheta);
        }
    }
}