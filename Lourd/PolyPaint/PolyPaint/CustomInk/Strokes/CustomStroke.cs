using System.Windows.Ink;
using System.Windows.Input;
using System.Windows;
using System;
using PolyPaint.Templates;
using PolyPaint.CustomInk.Strokes;
using PolyPaint.Services;

namespace PolyPaint.CustomInk
{
    public abstract class CustomStroke : Stroke
    {
        public Guid guid;
        public string name;
        public int strokeType;
        public string owner = null;

        public const double WIDTH = 2.1;
        public const double HEIGHT = 2.1;

        public CustomStroke(StylusPointCollection pts) : base(pts)
        {
            if(ConnectionService.username != null)
                owner = ConnectionService.username;
        }

        public virtual Point GetCenter()
        {
            Rect strokeBounds = GetBounds();
            
            Point leftTopPoint = GetTheLeftTopPoint();
            Point rightBottomPoint = GetTheRightBottomPoint();
            Point center = new Point(strokeBounds.X + strokeBounds.Width / 2, strokeBounds.Y + strokeBounds.Height / 2);
                //new Point((leftTopPoint.X + rightBottomPoint.X) /2, (leftTopPoint.Y + rightBottomPoint.Y) / 2);

            return center;
        }

        public virtual Rect GetCustomBound()
        {
            return base.GetBounds();
        }

        public virtual Rect GetStraightBounds()
        {
            return base.GetBounds();
        }

        public virtual CustomStroke CloneRotated(double rotation) {
            CustomStroke newStroke = (CustomStroke)Clone();
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

        public bool isLinkStroke()
        {
            return GetType() == typeof(LinkStroke);
        }

        public bool isProccessStroke()
        {
            return GetType() == typeof(ActorStroke) || GetType() == typeof(ActivityStroke) || GetType() == typeof(ArtifactStroke);
        }

        public virtual void updatePosition(Rect newRect) { }

        public virtual void updateLinks() { }

        internal virtual bool HitTestPoint(Point point)
        {
            return false;
        }
        internal virtual bool HitTestPointIncludingEdition(Point point)
        {
            return false;
        }

        public abstract Rect GetEditingBounds();
    }
}