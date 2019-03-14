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
            Point point;
            double xCenter = GetCenter().X;
            double yCenter = GetCenter().Y;
            double margin = 10;
            double halfWidth = GetBounds().Width / 2 + margin;
            double halfHeight = GetBounds().Height / 2 + margin;

            // gi AJOUTER LE ROTATE sur les bounds? creer
            double cosTheta = Math.Cos(rotation);
            double sinTheta = Math.Sin(rotation);
            double halfWidthRotated = halfWidth * cosTheta + halfHeight * sinTheta;
            double halfHeightRotated = -halfWidth * sinTheta + halfHeight * cosTheta;

            switch (anchorNumber)
            {
                case 0:
                    point = new Point(xCenter, yCenter - halfHeightRotated);
                    break;
                case 1:
                    point = new Point(xCenter + halfWidthRotated, yCenter);
                    break;
                case 2:
                    point = new Point(xCenter, yCenter + halfHeightRotated);
                    break;
                default:
                    point = new Point(xCenter - halfWidthRotated, yCenter);
                    break;
            }

            return point;
        }

        private Point rotatePoint(double x, double y)
        {
            double cosTheta = Math.Cos(rotation);
            double sinTheta = Math.Sin(rotation);

            return new Point(x * cosTheta + y * sinTheta, -x * sinTheta + y * cosTheta);
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