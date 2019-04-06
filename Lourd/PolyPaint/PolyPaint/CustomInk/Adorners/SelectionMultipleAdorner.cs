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
using System.Windows.Media.Imaging;
using PolyPaint.CustomInk.Adorners;

namespace PolyPaint.CustomInk
{
    class SelectionMultipleAdorner : CustomAdorner
    {
        Thumb moveThumb;
        private DeleteButton deleteButton;
        private CenterAlignButton centerAlignButton;
        private LeftAlignButton leftAlignButton;

        VisualCollection visualChildren;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public StrokeCollection strokesSelected;

        public CustomInkCanvas canvas;

        private Path resizePreview;
        private Path outerBoundPath;

        List<Path> shapeBorders = new List<Path>();

        RectangleGeometry NewRectangle = new RectangleGeometry();
        RectangleGeometry OldRectangle = new RectangleGeometry();

        private Rect rectangleDelete;
        private Rect rectangleCenter;
        private Rect rectangleLeftAlign;

        public SelectionMultipleAdorner(UIElement adornedElement, StrokeCollection strokes, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            adornedStroke = null;

            visualChildren = new VisualCollection(this);
            strokesSelected = strokes;

            foreach (CustomStroke stroke in strokesSelected)
            {
                if(stroke is ShapeStroke)
                {
                    RotateTransform rotation = new RotateTransform((stroke as ShapeStroke).shapeStyle.rotation, stroke.GetCenter().X, stroke.GetCenter().Y);
                    Path path = new Path();
                    path.Data = new RectangleGeometry(stroke.GetCustomBound(), 0, 0, rotation);
                    path.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF809dce"));
                    path.StrokeThickness = 2;
                    shapeBorders.Add(path);

                    visualChildren.Add(path);
                }
            }

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
            resizePreview.StrokeDashArray = new DoubleCollection { 5, 2 };
            resizePreview.StrokeThickness = 1;
            visualChildren.Add(resizePreview);

            canvas = actualCanvas;

            outerBoundPath = new Path();
            outerBoundPath.Stroke = Brushes.Black;
            outerBoundPath.StrokeDashArray = new DoubleCollection { 5, 2 }; 
            outerBoundPath.StrokeThickness = 1;
            outerBoundPath.Data = new RectangleGeometry(strokeBounds);
            visualChildren.Add(outerBoundPath);
            
            deleteButton = new DeleteButton(strokes, canvas);
            deleteButton.Cursor = Cursors.Hand;
            deleteButton.Width = 20;
            deleteButton.Height = 20;
            deleteButton.Background = Brushes.White;

            BitmapImage img2 = new BitmapImage();
            img2.BeginInit();
            img2.UriSource = new Uri("../../Resources/trash.png", UriKind.Relative);
            img2.EndInit();

            System.Windows.Controls.Image image2 = new System.Windows.Controls.Image();
            image2.Source = img2;
            deleteButton.Content = image2;

            visualChildren.Add(deleteButton);

            centerAlignButton = new CenterAlignButton(strokes, canvas);
            centerAlignButton.Cursor = Cursors.Hand;
            centerAlignButton.Width = 20;
            centerAlignButton.Height = 20;
            centerAlignButton.Background = Brushes.White;

            BitmapImage img3 = new BitmapImage();
            img3.BeginInit();
            img3.UriSource = new Uri("../../Resources/NewLook/horizontal-align-center.png", UriKind.Relative);
            img3.EndInit();

            System.Windows.Controls.Image image3 = new System.Windows.Controls.Image();
            image3.Source = img3;
            centerAlignButton.Content = image3;

            visualChildren.Add(centerAlignButton);

            leftAlignButton = new LeftAlignButton(strokes, canvas);
            leftAlignButton.Cursor = Cursors.Hand;
            leftAlignButton.Width = 20;
            leftAlignButton.Height = 20;
            leftAlignButton.Background = Brushes.White;

            BitmapImage img4 = new BitmapImage();
            img4.BeginInit();
            img4.UriSource = new Uri("../../Resources/NewLook/left-align.png", UriKind.Relative);
            img4.EndInit();

            System.Windows.Controls.Image image4 = new System.Windows.Controls.Image();
            image4.Source = img4;
            leftAlignButton.Content = image4;

            visualChildren.Add(leftAlignButton);

            rectangleCenter = new Rect(strokeBounds.X + strokeBounds.Width / 2 - 10, strokeBounds.Y - 20, 20, 20);
            rectangleLeftAlign = new Rect(strokeBounds.X, strokeBounds.Y - 20, 20, 20);
            rectangleDelete = new Rect(strokeBounds.TopRight.X - 20, strokeBounds.TopRight.Y - 20, 20, 20);
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

            deleteButton.Arrange(rectangleDelete);
            leftAlignButton.Arrange(rectangleLeftAlign);
            centerAlignButton.Arrange(rectangleCenter);

            foreach(Path path in shapeBorders)
            {
                path.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
            }

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