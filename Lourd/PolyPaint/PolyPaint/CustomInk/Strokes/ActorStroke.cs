using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Globalization;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace PolyPaint.CustomInk
{
    public class ActorStroke : ShapeStroke
    {
        
        private Point headCenter;
        private Point neck;
        private Point torso;
        private Point leftHand;
        private Point rightHand;
        private Point hip;
        private Point leftFoot;
        private Point rightFoot;
        
        private double headRadiusX = 12.5;
        private double headRadiusY = 12.5;
        /*
        private Vector headCenter = new Vector(0, -22.5);
        private Vector neck = new Vector(0, -10);
        private Vector torso = new Vector(0, -5);
        private Vector hip = new Vector(0, 20);
        private Vector leftHand = new Vector(-10, -15);
        private Vector rightHand = new Vector(10, -15);
        private Vector leftFoot = new Vector(-12.5, 35);
        private Vector rightFoot = new Vector(12.5, 35);
        */
        public ActorStroke(StylusPointCollection pts) : base(pts)
        {
            shapeStyle.coordinates = new Coordinates(pts[pts.Count - 1].ToPoint());
            shapeStyle.width = 1;
            shapeStyle.height = 1;
            UpdateShapePoints();

            strokeType = (int)StrokeTypes.ROLE;

            /*Point lastPoint = pts[pts.Count - 1].ToPoint();
            for (double i = lastPoint.X; i < shapeStyle.width + lastPoint.X; i += 0.5)
            {
                for (double j = lastPoint.Y; j < shapeStyle.height + lastPoint.Y; j += 0.5)
                {
                    StylusPoints.Add(new StylusPoint(i, j));
                }
            }*/
        }

        public ActorStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts, basicShape)
        {
            
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
            Brush brush1 = Brushes.Blue;
            Brush brush2 = Brushes.Red;
            Pen pen = new Pen(brush2, 3);

            Pen pen2 = new Pen(brush2, 1);
            pen.DashStyle = DashStyles.Dash;
            //drawingContext.DrawEllipse(brush2, null, headCenter, 12.5, 12.5);
            
            /*
            double rotationInRad = shapeStyle.rotation * Math.PI / 180;

            double m11 = Math.Cos(-rotationInRad);
            double m12 = -Math.Sin(-rotationInRad);
            double m21 = Math.Sin(-rotationInRad);
            double m22 = Math.Cos(-rotationInRad);

            Matrix rotationMatrix = new Matrix(m11, m12, m21, m22, 0, 0);

            Vector headCenterRotated = rotationMatrix.Transform(headCenter);
            Vector neckRotated = rotationMatrix.Transform(neck);
            Vector torsoRotated = rotationMatrix.Transform(torso);
            Vector hipRotated = rotationMatrix.Transform(hip);
            Vector leftHandRotated = rotationMatrix.Transform(leftHand);
            Vector rightHandRotated = rotationMatrix.Transform(rightHand);
            Vector leftFootRotated = rotationMatrix.Transform(leftFoot);
            Vector rightFootRotated = rotationMatrix.Transform(rightFoot);
            */

            UpdateShapePoints();

            TransformGroup transform = new TransformGroup();

            transform.Children.Add(new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y));


            // drawingContext.DrawRectangle(null, pen2, GetBounds());
            drawingContext.PushTransform(transform);

            drawingContext.DrawEllipse(brush1, pen, headCenter, headRadiusX, headRadiusY);
            drawingContext.DrawLine(pen, neck, hip);
            drawingContext.DrawLine(pen, torso, leftHand);
            drawingContext.DrawLine(pen, torso, rightHand);
            drawingContext.DrawLine(pen, hip, leftFoot);
            drawingContext.DrawLine(pen, hip, rightFoot);

            // Create the image
            /*BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri("../../Resources/actor.png", UriKind.Relative);
            img.EndInit();

            Rect bounds = GetBounds();
            double x = (bounds.Right + bounds.Left) / 2;
            double y = (bounds.Bottom + bounds.Top) / 2;

            TransformGroup transform = new TransformGroup();

            transform.Children.Add(new RotateTransform(shapeStyle.rotation, x, y));

            drawingContext.PushTransform(transform);

            drawingContext.DrawImage(img, new Rect(GetTheFirstPoint(), GetTheLastPoint()));*/
        }

        public override Rect GetBounds()
        {
            /*double rotationInRad = shapeStyle.rotation * Math.PI / 180;

            double m11 = Math.Cos(rotationInRad);
            double m12 = -Math.Sin(rotationInRad);
            double m21 = Math.Sin(rotationInRad);
            double m22 = Math.Cos(rotationInRad);

            Matrix rotationMatrix = new Matrix(m11, m12, m21, m22, 0, 0);*/
            double width = shapeStyle.width * 30;
            double height = shapeStyle.height * 70;

            Rect rect = new Rect(shapeStyle.coordinates.x, shapeStyle.coordinates.y, 
                width, height);

            /*rect.Offset(-shapeStyle.coordinates.x - width/2, 
                -shapeStyle.coordinates.y - height / 2);

            rect.Transform(rotationMatrix);

            rect.Offset(shapeStyle.coordinates.x + width / 2, 
                shapeStyle.coordinates.y + height / 2);
                */
            return rect;
        }

        internal override bool HitTestPoint(Point point)
        {
            return GetBounds().Contains(point);
        }

        
        public void UpdateShapePoints()
        {
            double width = shapeStyle.width * 30;
            double height = shapeStyle.height * 70;

            headRadiusX = width / 2;
            headRadiusY = height / 6;

            neck = shapeStyle.coordinates.ToPoint();
            neck.Offset(width / 2, headRadiusY * 2);

            torso = new Point(neck.X, neck.Y + headRadiusY);

            leftHand = new Point(neck.X - width / 2, neck.Y + headRadiusY / 2);

            rightHand = new Point(neck.X + width / 2, neck.Y + headRadiusY / 2);

            hip = new Point(neck.X, neck.Y + headRadiusY * 3);

            leftFoot = new Point(neck.X - width / 2, neck.Y + headRadiusY * 4);

            rightFoot = new Point(neck.X + width / 2, neck.Y + headRadiusY * 4);

            headCenter = new Point(neck.X, neck.Y - headRadiusY);
        }

        public override Point GetCenter()
        {
            Rect rect = GetBounds();
            return new Point (rect.X + shapeStyle.width * 30 / 2, rect.Y + shapeStyle.height * 70 / 2);
        }

    }
}