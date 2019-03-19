using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using PolyPaint.CustomInk.Strokes;
using System.Windows.Controls.Primitives;
using System;
using System.Windows.Shapes;
using PolyPaint.Templates;

namespace PolyPaint.CustomInk
{
    class LinkElbowAdorner : Adorner
    {
        List<Thumb> anchors;
        VisualCollection visualChildren;

        // The center of the strokes.
        Point center;

        // The linkLine
        Path line;

        const int HANDLEMARGIN = 15;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public LinkStroke stroke;
        public CustomInkCanvas canvas;
        private Point initialMousePosition;
        private int indexInPath;

        public LinkElbowAdorner(Point mousePosition, int index, UIElement adornedElement, LinkStroke linkStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            initialMousePosition = mousePosition;
            indexInPath = index;

            stroke = linkStroke;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = linkStroke.GetBounds();
            center = stroke.GetCenter();

            anchors = new List<Thumb>();
            // The linkstroke must already be selected
            if (!isOnLinkStrokeEnds(initialMousePosition))
            {
                anchors.Add(new Thumb());
            }

            visualChildren = new VisualCollection(this);
            foreach (Thumb anchor in anchors)
            {
                anchor.Cursor = Cursors.SizeNWSE;
                anchor.Width = 10;
                anchor.Height = 10;
                anchor.Background = Brushes.Black;

                anchor.DragDelta += new DragDeltaEventHandler(dragHandle_DragDelta);
                anchor.DragCompleted += new DragCompletedEventHandler(dragHandle_DragCompleted);
                anchor.DragStarted += new DragStartedEventHandler(dragHandle_DragStarted);

                visualChildren.Add(anchor);
            }

            strokeBounds = linkStroke.GetBounds();

            line = new Path();

            visualChildren.Add(line);

        }

        private bool isOnLinkStrokeEnds(Point initialMousePosition)
        {
            double x = stroke.path[0].x - initialMousePosition.X;
            double y = stroke.path[0].y - initialMousePosition.Y;

            double distBetweenPoints = (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
            if (distBetweenPoints <= 10)
            {
                return true;
            }

            x = stroke.path[stroke.path.Count - 1].x - initialMousePosition.X;
            y = stroke.path[stroke.path.Count - 1].y - initialMousePosition.Y;

            distBetweenPoints = (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
            if (distBetweenPoints <= 10)
            {
                return true;
            }

            return false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            center = stroke.GetCenter();

            for (int i = 0; i < anchors.Count; i++)
            {
                ArrangeAnchor(i, -center.X + initialMousePosition.X, -center.Y + initialMousePosition.Y);
            }

            line.Arrange(new Rect(finalSize));

            return finalSize;
        }

        private void ArrangeAnchor(int anchorNumber, double xOffset, double yOffset)
        {
            // The rectangle that determines the position of the Thumb.
            Rect handleRect = new Rect(strokeBounds.X + xOffset,
                                  strokeBounds.Y + yOffset,
                                  strokeBounds.Width,
                                  strokeBounds.Height);

            // Draws the thumb and the rectangle around the strokes.
            anchors[anchorNumber].Arrange(handleRect);
        }

        void dragHandle_DragStarted(object sender, DragStartedEventArgs e)
        {
        }

        void dragHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            
        }


        void dragHandle_DragCompleted(object sender,
                                        DragCompletedEventArgs e)
        {
            Point actualPos = Mouse.GetPosition(this);
            if (actualPos.X < 0 || actualPos.Y < 0)
            {
                visualChildren.Clear();
                InvalidateArrange();
                return;
            }

            stroke.path.Insert(indexInPath, new Coordinates(actualPos));
            stroke.addStylusPointsToLink();
            canvas.RefreshChildren();
            InvalidateArrange();
            visualChildren.Clear();
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