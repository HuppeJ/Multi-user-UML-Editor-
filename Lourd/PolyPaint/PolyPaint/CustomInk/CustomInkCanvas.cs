using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PolyPaint.CustomInk
{
    class CustomInkCanvas : InkCanvas
    {
        CustomDynamicRenderer customRenderer = new CustomDynamicRenderer();

        public CustomInkCanvas() : base()
        {
            // Use the custom dynamic renderer on the
            // custom InkCanvas.
            this.DynamicRenderer = customRenderer;
        }

        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            // Remove the original stroke and add a custom stroke.
            this.Strokes.Remove(e.Stroke);
            CustomStroke customStroke = new CustomStroke(e.Stroke.StylusPoints);
            this.Strokes.Add(customStroke);

            // Pass the custom stroke to base class' OnStrokeCollected method.
            InkCanvasStrokeCollectedEventArgs args = new InkCanvasStrokeCollectedEventArgs(customStroke);
            base.OnStrokeCollected(args);
        }
    }
}
