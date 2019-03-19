using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media.Imaging;
using System.Globalization;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System.Collections.Generic;

namespace PolyPaint.CustomInk
{
    public class CommentStroke : ShapeStroke
    {

        public CommentStroke(StylusPointCollection pts) : base(pts)
        {
            type = (int)StrokeTypes.COMMENT;
            shapeStyle.height = 50;

            Point lastPoint = pts[pts.Count - 1].ToPoint();
            for (double i = lastPoint.X; i < shapeStyle.width + lastPoint.X; i += 0.5)
            {
                for (double j = lastPoint.Y; j < shapeStyle.height + lastPoint.Y; j += 0.5)
                {
                    StylusPoints.Add(new StylusPoint(i, j));
                }
            }
        }

        public CommentStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts, basicShape)
        {
            
        }

        public override BasicShape GetBasicShape()
        {
            return new BasicShape(guid.ToString(), type, name, shapeStyle, linksTo, linksFrom);
        }
    }
}