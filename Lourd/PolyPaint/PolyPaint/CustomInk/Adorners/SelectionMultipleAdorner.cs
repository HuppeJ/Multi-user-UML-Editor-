using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using PolyPaint.CustomInk.Strokes;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System;
using System.Windows.Ink;
using PolyPaint.Services;
using PolyPaint.Vues;

namespace PolyPaint.CustomInk
{
    class SelectionMultipleAdorner : Adorner
    {
        Thumb moveThumb;

        VisualCollection visualChildren;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public StrokeCollection strokesSelected;

        public CustomInkCanvas canvas;

        private Path resizePreview;
        private Path outerBoundPath;

        RectangleGeometry NewRectangle = new RectangleGeometry();
        RectangleGeometry OldRectangle = new RectangleGeometry();

        public SelectionMultipleAdorner(UIElement adornedElement, StrokeCollection strokes, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);
            strokesSelected = strokes;

            double maxX = -9999999;
            double minX = 9999999;
            double maxY = -9999999;
            double minY = 9999999;

            foreach(CustomStroke stroke in strokesSelected)
            {
                if(stroke.GetEditingBounds().X < minX)
                    minX = stroke.GetEditingBounds().X;
                if (stroke.GetEditingBounds().X + stroke.GetEditingBounds().Width > maxX)
                    maxX = stroke.GetEditingBounds().X + stroke.GetEditingBounds().Width;
                if (stroke.GetEditingBounds().Y < minY)
                    minY = stroke.GetEditingBounds().Y;
                if (stroke.GetEditingBounds().Y + stroke.GetEditingBounds().Height > maxY)
                    maxY = stroke.GetEditingBounds().Y + stroke.GetEditingBounds().Height;
            }

            strokeBounds = new Rect(new Point(minX, minY), new Point(maxX, maxY));

            moveThumb = new Thumb();
            moveThumb.Cursor = Cursors.SizeAll;
            moveThumb.Height = strokeBounds.Height;
            moveThumb.Width = strokeBounds.Width;
            moveThumb.Background = Brushes.Transparent;
            moveThumb.DragDelta += new DragDeltaEventHandler(Move_DragDelta);
            moveThumb.DragCompleted += new DragCompletedEventHandler(Move_DragCompleted);
            moveThumb.DragStarted += new DragStartedEventHandler(All_DragStarted);

            visualChildren.Add(moveThumb);

            resizePreview = new Path();
            resizePreview.Stroke = Brushes.Gray;
            resizePreview.StrokeThickness = 2;
            visualChildren.Add(resizePreview);

            canvas = actualCanvas;

            outerBoundPath = new Path();
            outerBoundPath.Stroke = Brushes.Gray;
            outerBoundPath.StrokeDashArray = new DoubleCollection { 5, 2 }; 
            outerBoundPath.StrokeThickness = 1;
            outerBoundPath.Data = new RectangleGeometry(strokeBounds);
            visualChildren.Add(outerBoundPath);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            resizePreview.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
            outerBoundPath.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
            
            moveThumb.Arrange(strokeBounds);

            return finalSize;
        }

        void All_DragStarted(object sender, DragStartedEventArgs e)
        {
            OldRectangle = new RectangleGeometry(strokeBounds);
        }

        void Move_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (e.HorizontalChange == 0 && e.VerticalChange == 0)
            {
                visualChildren.Remove(resizePreview);
                InvalidateArrange();
                return;
            }

            visualChildren.Remove(resizePreview);

            canvas.MoveShape(NewRectangle.Rect.X - OldRectangle.Rect.X, NewRectangle.Rect.Y - OldRectangle.Rect.Y);

            canvas.RefreshLinks(false);
            canvas.RefreshChildren();

            InvalidateArrange();

            DrawingService.UpdateShapes(strokesSelected);
        }
        
        private void generatePreview(Rect rectangle)
        {
            NewRectangle = new RectangleGeometry(rectangle);
            resizePreview.Data = NewRectangle;
            resizePreview.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
        }

        void Move_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.HorizontalChange != 0 || e.VerticalChange != 0)
            {
                Rect rectangle = new Rect(strokeBounds.X + e.HorizontalChange,
                                          strokeBounds.Y + e.VerticalChange,
                                          strokeBounds.Width,
                                          strokeBounds.Height);
                generatePreview(rectangle);
            }
        }

        // Override the VisualChildrenCount and 
        // GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount
        {
            get { return visualChildren.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }


    }
}