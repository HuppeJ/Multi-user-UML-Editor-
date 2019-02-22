using PolyPaint.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;

namespace PolyPaint.CustomInk
{
    class CustomInkCanvas : InkCanvas
    {
        CustomDynamicRenderer customRenderer = new CustomDynamicRenderer();
        

        public string StrokeType
        {
            get { return (string) GetValue(StrokeTypeProperty); }
            set { SetValue(StrokeTypeProperty, value); }
        }
        public static readonly DependencyProperty StrokeTypeProperty = DependencyProperty.Register(
          "StrokeType", typeof(string), typeof(CustomInkCanvas), new PropertyMetadata("class"));


        public CustomInkCanvas() : base()
        {
            // Use the custom dynamic renderer on the
            // custom InkCanvas.
            DynamicRenderer = customRenderer;
        }

        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            // Remove the original stroke and add a custom stroke.
            Strokes.Remove(e.Stroke);

            Stroke customStroke;

            switch (StrokeType)
            {
                case "role":
                    customStroke = new ArtifactStroke(e.Stroke.StylusPoints);
                    break;
                default:
                    customStroke = new CustomStroke(e.Stroke.StylusPoints);
                    break;
               
            }
            Strokes.Add(customStroke);

            // Pass the custom stroke to base class' OnStrokeCollected method.
            InkCanvasStrokeCollectedEventArgs args = new InkCanvasStrokeCollectedEventArgs(customStroke);
            base.OnStrokeCollected(args);
        }
    }
}
