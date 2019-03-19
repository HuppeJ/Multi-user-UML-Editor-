using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Globalization;
using PolyPaint.Templates;
using System.Collections.Generic;

namespace PolyPaint.CustomInk
{
    public abstract class ShapeStroke : CustomStroke
    {
        public ShapeStyle shapeStyle;
        public List<string> linksTo;
        public List<string> linksFrom;
        public List<Point> anchorPoints;

        public ShapeStroke(StylusPointCollection pts) : base(pts)
        {
            guid = Guid.NewGuid();
            name = "This is a stroke";

            int width = 100;
            if (type == 0) width = 150;

            Point lastPoint = pts[pts.Count - 1].ToPoint();
            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(0);
            }
            for (double i = lastPoint.X; i < width + lastPoint.X; i += 0.5)
            {
                for (double j = lastPoint.Y; j < 100 + lastPoint.Y; j += 0.5)
                {
                    StylusPoints.Add(new StylusPoint(i, j));
                }
            }

            // Quelle est la width?
            // Les coordonnees sont bien celles du coin en haut a gauche?
            double height = 100.0;
            double rotation = 0.0;
            //shapeStyle = new ShapeStyle(new Coordinates(lastPoint.X, lastPoint.Y), width, height, rotation);
            anchorPoints = new List<Point>();
        }

        public ShapeStroke(StylusPointCollection pts, BasicShape basicShape) : base(pts)
        {
            guid = new Guid(basicShape.id);
            name = basicShape.name;
            type = basicShape.type;
            shapeStyle = basicShape.shapeStyle;

            Point point = new Point(shapeStyle.coordinates.x, shapeStyle.coordinates.y);

            for (double i = point.X; i < shapeStyle.width + point.X; i += 0.5)
            {
                for (double j = point.Y; j < shapeStyle.height + point.Y; j += 0.5)
                {
                    StylusPoints.Add(new StylusPoint(i, j));
                }
            }
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
                    pointRotatedAroundOrigin = rotatePoint(halfWidth, 0);
                    break;
                case 2:
                    pointRotatedAroundOrigin = rotatePoint(0, -halfHeight);
                    break;
                default:
                    pointRotatedAroundOrigin = rotatePoint(-halfWidth, 0);
                    break;
            }

            return  new Point(xCenter - pointRotatedAroundOrigin.X, yCenter - pointRotatedAroundOrigin.Y);
        }

        private Point rotatePoint(double x, double y)
        {
            double rotationInRad = rotation * Math.PI / 180;
            double cosTheta = Math.Cos(rotationInRad);
            double sinTheta = Math.Sin(rotationInRad);
            
            return new Point(x * cosTheta - y * sinTheta, x * sinTheta + y * cosTheta);
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
            SolidColorBrush brush2 = new SolidColorBrush(drawingAttributes.Color);
            brush2.Freeze();
            // drawingContext.DrawRectangle(brush2, null, new Rect(GetTheLeftTopPoint(), GetTheRightBottomPoint()));
            // Create the source
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            // img.UriSource = new Uri("C:/Users/Alex/Pictures/Polar-bear-cub_917.jpg");
            img.UriSource = new Uri("../../Resources/artefact.png", UriKind.Relative);
            img.EndInit();

            //drawingContext.DrawImage(img, new Rect(GetTheFirstPoint(), GetTheLastPoint()));

            FormattedText formattedText = new FormattedText(
                "Hello",
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                32,
                Brushes.Black);

            drawingContext.DrawText(formattedText, GetTheFirstPoint());
        }
        
    }
}