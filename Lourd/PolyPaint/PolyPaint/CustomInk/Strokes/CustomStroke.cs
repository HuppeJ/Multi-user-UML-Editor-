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
    public abstract class CustomStroke : Stroke
    {
        public double rotation = 0.0;
        public Guid guid;
        public string name;
        public int type;
        public ShapeStyle shapeStyle;
        public List<string> linksTo;
        public List<string> linksFrom;

        public CustomStroke(StylusPointCollection pts) : base(pts)
        {
            guid = Guid.NewGuid();
            name = "This is a stroke";

            Point lastPoint = pts[pts.Count - 1].ToPoint();
            Coordinates coordinates = new Coordinates(lastPoint.X, lastPoint.Y);

            shapeStyle = new ShapeStyle(coordinates,100,100,0,"black",0,"none");

            if (type == 0) shapeStyle.width = 150;


            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(0);
            }
            for (double i = lastPoint.X; i < shapeStyle.width + lastPoint.X; i += 0.5)
            {
                for (double j = lastPoint.Y; j < shapeStyle.height + lastPoint.Y; j += 0.5)
                {
                    StylusPoints.Add(new StylusPoint(i, j));
                }
            }
            linksTo = new List<string>();
            linksFrom = new List<string>();
        }

        public CustomStroke(StylusPointCollection pts, BasicShape basicShape) : base(pts)
        {
            guid = Guid.Parse(basicShape.id);
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

        public CustomStroke CloneRotated(double rotation) {
            CustomStroke newStroke = (CustomStroke)Clone();

            // Changer les bounds? Gi
            //newStroke.GetBounds().Transform(rotation.Value);

            newStroke.rotation = rotation;
            return newStroke;
        }

        protected Point GetTheLeftTopPoint()
        {
            if (StylusPoints == null)
                throw new ArgumentNullException("StylusPoints");
            StylusPoint tmpPoint = new StylusPoint(double.MaxValue, double.MaxValue);
            foreach (StylusPoint point in StylusPoints)
            {
                if ((point.X < tmpPoint.X) || (point.Y < tmpPoint.Y))
                    tmpPoint = point;
            }
            return tmpPoint.ToPoint();
        }

        protected Point GetTheFirstPoint()
        {
            return StylusPoints[0].ToPoint();
        }

        protected Point GetTheLastPoint()
        {
            return StylusPoints[StylusPoints.Count - 1].ToPoint();
        }

        protected Point GetTheRightBottomPoint()
        {
            if (StylusPoints == null)
                throw new ArgumentNullException("StylusPoints");
            StylusPoint tmpPoint = new StylusPoint(0, 0);
            foreach (StylusPoint point in StylusPoints)
            {
                if ((point.X > tmpPoint.X) || (point.Y > tmpPoint.Y))
                    tmpPoint = point;
            }
            return tmpPoint.ToPoint();
        }

        public BasicShape GetBasicShape()
        {
            BasicShape basicShape = new BasicShape(guid.ToString(), type, name, shapeStyle, linksTo, linksFrom);
            return basicShape;
        }
    }
}