using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Globalization;
using PolyPaint.Enums;
using PolyPaint.Templates;

namespace PolyPaint.CustomInk
{
    public class ArtifactStroke : ShapeStroke
    {
        public ArtifactStroke(StylusPointCollection pts) : base(pts)
        {
            type = (int)StrokeTypes.ARTIFACT;

            Point lastPoint = pts[pts.Count - 1].ToPoint();
            for (double i = lastPoint.X; i < shapeStyle.width + lastPoint.X; i += 0.5)
            {
                for (double j = lastPoint.Y; j < shapeStyle.height + lastPoint.Y; j += 0.5)
                {
                    StylusPoints.Add(new StylusPoint(i, j));
                }
            }
        }

        public ArtifactStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts)
        {
            type = (int)StrokeTypes.ARTIFACT;
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
            
            // Create the image
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("../../Resources/artefact.png", UriKind.Relative);
            img.EndInit();
            
            Rect bounds = GetBounds();
            double x = (bounds.Right + bounds.Left) / 2;
            double y = (bounds.Bottom + bounds.Top) / 2;

            TransformGroup transform = new TransformGroup();
            
            transform.Children.Add(new RotateTransform(rotation, x, y));

            drawingContext.PushTransform(transform);

            drawingContext.DrawImage(img, new Rect(GetTheFirstPoint(), GetTheLastPoint()));
        }
       
    }
}