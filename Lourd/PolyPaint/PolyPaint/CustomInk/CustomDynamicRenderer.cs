using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;

namespace PolyPaint.CustomInk
{
    class CustomDynamicRenderer: DynamicRenderer 
    {
        [ThreadStatic]
        static private Brush brush = null;

        [ThreadStatic]
        static private Pen pen = null;

        private Point prevPoint;

        protected override void OnStylusDown(RawStylusInput rawStylusInput)
        {
            // Allocate memory to store the previous point to draw from.
            prevPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            base.OnStylusDown(rawStylusInput);
        }

        protected override void OnDraw(DrawingContext drawingContext,
                                       StylusPointCollection stylusPoints,
                                       Geometry geometry, Brush fillBrush)
        {
            if (brush == null)
            {
                Color primaryColor;

                if (fillBrush is SolidColorBrush)
                {
                    primaryColor = ((SolidColorBrush)fillBrush).Color;
                }
                else
                {
                    primaryColor = Colors.Red;
                }

                brush = new LinearGradientBrush(Colors.Blue, primaryColor, 20d);
            }

            drawingContext.DrawGeometry(brush, null, geometry);
        }
    }
}
