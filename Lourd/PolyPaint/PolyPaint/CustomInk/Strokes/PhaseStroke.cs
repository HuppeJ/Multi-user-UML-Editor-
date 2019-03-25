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
    public class PhaseStroke : ShapeStroke
    {
        public List<Coordinates> path { get; set; }

        public PhaseStroke(StylusPointCollection pts) : base(pts)
        {
            InitializePhase();
        }
        
        public PhaseStroke(BasicShape basicShape, StylusPointCollection pts) : base(pts, basicShape)
        {
            InitializePhase();
        }

        private void InitializePhase()
        {
            strokeType = (int)StrokeTypes.PHASE;
            double x = shapeStyle.coordinates.x;
            double y = shapeStyle.coordinates.y;

            // Top left
            StylusPoints.Add(new StylusPoint(x, y));
            // Bottom left
            StylusPoints.Add(new StylusPoint(x, y + shapeStyle.height));
            // Bottom right
            StylusPoints.Add(new StylusPoint(x + shapeStyle.width, y + shapeStyle.height));
            // Top right
            StylusPoints.Add(new StylusPoint(x + shapeStyle.width, y));
            // Top left
            StylusPoints.Add(new StylusPoint(x, y));
            //Mid left
            StylusPoints.Add(new StylusPoint(x, y + 20));
            //Mid right
            StylusPoints.Add(new StylusPoint(x + shapeStyle.width, y + 20));

            DrawingAttributes.Width = 3;
            DrawingAttributes.Height = 3;
            DrawingAttributes.Color = (Color)ColorConverter.ConvertFromString("#FF000000");
        }

        public override BasicShape GetBasicShape()
        {
            return new BasicShape(guid.ToString(), strokeType, name, shapeStyle, linksTo, linksFrom);
        }
    }
}