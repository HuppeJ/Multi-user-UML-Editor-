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
    public class ArtifactStroke : CustomStroke
    {
        public ArtifactStroke(StylusPointCollection pts) : base(pts)
        {
            type = (int)StrokeTypes.ARTIFACT;
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

            drawingContext.DrawImage(img, new Rect(GetTheFirstPoint(), GetTheLastPoint()));
        }

        public override void Rotate()
        {
            // à changer
            Matrix rotatingMatrix = new Matrix();
            Rect bounds = this.GetBounds();
            double x = (bounds.Right + bounds.Left) / 2;
            double y = (bounds.Bottom + bounds.Top) / 2;

            Point rotatePoint = new Point(x, y);

            rotatingMatrix.RotateAt(90, rotatePoint.X, rotatePoint.Y);
            this.Transform(rotatingMatrix, false);
        }
       
    }
}