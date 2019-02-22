using PolyPaint.Enums;
using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.CustomInk
{
    class CustomInkCanvas : InkCanvas
    {
        CustomDynamicRenderer customRenderer = new CustomDynamicRenderer();
        

        public string CustomStrokeType
        {
            get { return (string) GetValue(CustomStrokeTypeProperty); }
            set { SetValue(CustomStrokeTypeProperty, value); }
        }
        public static readonly DependencyProperty CustomStrokeTypeProperty = DependencyProperty.Register(
          "CustomStrokeType", typeof(string), typeof(CustomInkCanvas), new PropertyMetadata("class"));


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
            CustomStroke customStroke = new CustomStroke(e.Stroke.StylusPoints);
            Strokes.Add(customStroke);

            // Pass the custom stroke to base class' OnStrokeCollected method.
            InkCanvasStrokeCollectedEventArgs args = new InkCanvasStrokeCollectedEventArgs(customStroke);
            base.OnStrokeCollected(args);
        }
    }
}
