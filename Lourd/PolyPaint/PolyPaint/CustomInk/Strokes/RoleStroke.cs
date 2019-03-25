using PolyPaint.Enums;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PolyPaint.CustomInk.Strokes
{
    class RoleStroke : ShapeStroke
    {
        public PointCollection path =
            new PointCollection { new Point(50, 130), new Point(100, 100), new Point(150, 130), new Point(100, 100), new Point(100, 10) };

        public RoleStroke(StylusPointCollection pts) : base(pts)
        {
            guid = Guid.NewGuid();
            name = "role";
            strokeType = (int)StrokeTypes.ROLE;

            Point center = GetCenter();

            while (StylusPoints.Count > 1)
            {
                StylusPoints.RemoveAt(0);
            }
            //StylusPoints.Add(new )
            foreach (Point point in path)
            {
                Point translatedPoint = point + new Vector(StylusPoints[0].X, StylusPoints[0].Y);

                StylusPoints.Add(new StylusPoint(translatedPoint.X, translatedPoint.Y));
            }
            StylusPoints.RemoveAt(0);
        }

        public void RotateStroke(double rotation)
        {
            Matrix mat = new Matrix();
            mat.RotateAt(rotation, GetCenter().X, GetCenter().Y);
            Transform(mat, true);

            //while (StylusPoints.Count > 1)
            //{
            //    StylusPoints.RemoveAt(0);
            //}

            ////StylusPoints.Add(new )
            //foreach (Point point in path)
            //{
            //    Point rotatedPoint = rotatePointAroundPoint(point, GetCenter(), rotation);
            //    StylusPoints.Add(new StylusPoint(rotatedPoint.X, rotatedPoint.Y));
            //}
            //this.rotation = rotation;

            //StylusPoints.RemoveAt(0);
        }

        public Point rotatePointAroundPoint(Point pointToRotate, Point center, double rotation)
        {
            double rotationInRad = rotation * Math.PI / 180;
            double cosTheta = Math.Cos(rotationInRad);
            double sinTheta = Math.Sin(rotationInRad);
            Vector originToCenter = center - new Point(0,0);
            Point pointAtOrigin = pointToRotate - originToCenter;

            Point pointRotated = new Point(pointAtOrigin.X * cosTheta - pointAtOrigin.Y * sinTheta, pointAtOrigin.X * sinTheta + pointAtOrigin.Y * cosTheta);

            return pointRotated + originToCenter;
        }

    }
}
