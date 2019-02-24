using PolyPaint.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;

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

        public StrokeCollection SelectedStrokes
        {
            get { return (StrokeCollection) GetValue(SelectedStrokesProperty); }
            set { SetValue(SelectedStrokesProperty, value); }
        }
        public static readonly DependencyProperty SelectedStrokesProperty = DependencyProperty.Register(
          "SelectedStrokes", typeof(StrokeCollection), typeof(CustomInkCanvas), new PropertyMetadata(new StrokeCollection()));
       

        public CustomInkCanvas() : base()
        {
            // Use the custom dynamic renderer on the
            // custom InkCanvas.
            DynamicRenderer = customRenderer;
        }

        protected override void OnSelectionChanged(EventArgs e) {
            SelectedStrokes = this.GetSelectedStrokes();
        }

        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            // Remove the original stroke and add a custom stroke.
            Strokes.Remove(e.Stroke);

            Stroke customStroke;

            switch (StrokeType)
            {
                case "artifact":
                    customStroke = new ArtifactStroke(e.Stroke.StylusPoints);
                    break;
                case "activity":
                    customStroke = new ActivityStroke(e.Stroke.StylusPoints);
                    break;
                case "actor":
                    customStroke = new ActorStroke(e.Stroke.StylusPoints);
                    break;
                case "class":
                    customStroke = new ClassStroke(e.Stroke.StylusPoints);
                    break;
                default:
                    customStroke = new ClassStroke(e.Stroke.StylusPoints);
                    break;
               
            }
            Strokes.Add(customStroke);

            // Visual visual = this.GetVisualChild(this.Children.Count - 1);

            //AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
            //myAdornerLayer.Add(new AnchorPointAdorner(visual));

            // Pass the custom stroke to base class' OnStrokeCollected method.
            InkCanvasStrokeCollectedEventArgs args = new InkCanvasStrokeCollectedEventArgs(customStroke);
            base.OnStrokeCollected(args);
        }
    }
}
