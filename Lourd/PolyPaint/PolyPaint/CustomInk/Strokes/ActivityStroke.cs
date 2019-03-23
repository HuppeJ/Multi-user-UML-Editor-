﻿using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using PolyPaint.Enums;
using PolyPaint.Templates;

namespace PolyPaint.CustomInk
{
    public class ActivityStroke : ShapeStroke
    {
        private Point topLeft;
        private Point topRight;
        private Point right;
        private Point bottomRight;
        private Point bottomLeft;

        private const int WIDTH = 50;
        private const int HEIGHT = 30;

        public ActivityStroke(StylusPointCollection pts) : base(pts)
        {
            shapeStyle.coordinates = new Coordinates(pts[pts.Count - 1].ToPoint());

            UpdateShapePoints();

            strokeType = (int)StrokeTypes.ROLE;
        }

        public ActivityStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts, basicShape)
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

            UpdateShapePoints();

            TransformGroup transform = new TransformGroup();

            transform.Children.Add(new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y));

            // drawingContext.DrawRectangle(null, pen2, GetBounds());
            drawingContext.PushTransform(transform);

            LineSegment topRightSeg = new LineSegment(topRight, true);
            LineSegment rightSeg = new LineSegment(right, true);
            LineSegment bottomRightSeg = new LineSegment(bottomRight, true);
            LineSegment bottomLeftSeg = new LineSegment(bottomLeft, true);
            PathSegmentCollection segments = new PathSegmentCollection { topRightSeg, rightSeg, bottomRightSeg, bottomLeftSeg };
            PathFigure figure = new PathFigure();
            figure.Segments = segments;
            figure.IsClosed = true;
            figure.StartPoint = topLeft;
            PathFigureCollection figures = new PathFigureCollection { figure };
            PathGeometry geometry = new PathGeometry();
            geometry.Figures = figures;

            drawingContext.DrawGeometry(brush1, pen, geometry);
        }

        public override Rect GetBounds()
        {
            double width = shapeStyle.width * WIDTH;
            double height = shapeStyle.height * HEIGHT;

            Rect rect = new Rect(shapeStyle.coordinates.x, shapeStyle.coordinates.y,
                width, height);

            return rect;
        }

        internal override bool HitTestPoint(Point point)
        {
            RotateTransform rotationTransform = new RotateTransform(shapeStyle.rotation, GetCenter().X, GetCenter().Y);
            return GetBounds().Contains(rotationTransform.Inverse.Transform(point));
        }


        private void UpdateShapePoints()
        {
            double width = shapeStyle.width * WIDTH;
            double height = shapeStyle.height * HEIGHT;

            topLeft = shapeStyle.coordinates.ToPoint();

            topRight = new Point(topLeft.X + width * 0.8, topLeft.Y);

            right = new Point(topLeft.X + width, topLeft.Y + height * 0.5);

            bottomLeft = new Point(topLeft.X, topLeft.Y + height);

            bottomRight = new Point(topLeft.X + width * 0.8, topLeft.Y + height);
        }

        public override Point GetCenter()
        {
            Rect rect = GetBounds();
            return new Point(rect.X + shapeStyle.width * WIDTH / 2, rect.Y + shapeStyle.height * HEIGHT / 2);
        }
    }
}