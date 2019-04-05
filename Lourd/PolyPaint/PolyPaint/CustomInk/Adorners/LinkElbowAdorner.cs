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
using PolyPaint.Enums;
using PolyPaint.Services;
using System.Windows.Ink;
using PolyPaint.CustomInk.Adorners;

namespace PolyPaint.CustomInk
{
    class LinkElbowAdorner : CustomAdorner 
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
        public LinkStroke linkStroke;
        public CustomInkCanvas canvas;
        private Point initialMousePosition;
        private int indexInPath;

        public LinkElbowAdorner(Point mousePosition, int index, UIElement adornedElement, LinkStroke linkStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            adornedStroke = linkStroke;

            initialMousePosition = mousePosition;
            indexInPath = index;

            this.linkStroke = linkStroke;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = linkStroke.GetStraightBounds();
            center = this.linkStroke.GetCenter();

            anchors = new List<Thumb>();
            // The linkstroke must already be selected
            if (!isOnLinkStrokeEnds(initialMousePosition))
            {
                anchors.Add(new Thumb());
            }

            visualChildren = new VisualCollection(this);
            foreach (Thumb anchor in anchors)
            {
                anchor.Cursor = Cursors.ScrollAll;
                anchor.Width = 6;
                anchor.Height = 6;
                anchor.Background = Brushes.Black;

                anchor.DragDelta += new DragDeltaEventHandler(dragHandle_DragDelta);
                anchor.DragCompleted += new DragCompletedEventHandler(dragHandle_DragCompleted);
                anchor.DragStarted += new DragStartedEventHandler(dragHandle_DragStarted);

                visualChildren.Add(anchor);
            }

            line = new Path();

            visualChildren.Add(line);

        }

        private bool isOnLinkStrokeEnds(Point initialMousePosition)
        {

            double strokeBeginLength = 10;
            if ((LinkTypes)linkStroke.linkType == LinkTypes.ONE_WAY_ASSOCIATION)
            {
                strokeBeginLength = 20;
            }
            double strokeEndLength = GetUnmovableEndLength();


            double x = linkStroke.path[0].x - initialMousePosition.X;
            double y = linkStroke.path[0].y - initialMousePosition.Y;

            double distBetweenPoints = (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
            if (distBetweenPoints <= strokeBeginLength)
            {
                return true;
            }

            x = linkStroke.path[linkStroke.path.Count - 1].x - initialMousePosition.X;
            y = linkStroke.path[linkStroke.path.Count - 1].y - initialMousePosition.Y;

            distBetweenPoints = (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
            if (distBetweenPoints <= strokeEndLength)
            {
                return true;
            }

            return false;
        }

        private double GetUnmovableEndLength()
        {
            double strokeEndLength;
            switch ((LinkTypes)linkStroke.linkType)
            {
                case LinkTypes.LINE:
                    strokeEndLength = 10;
                    break;
                case LinkTypes.ONE_WAY_ASSOCIATION:
                    strokeEndLength = 20;
                    break;
                case LinkTypes.TWO_WAY_ASSOCIATION:
                    strokeEndLength = 20;
                    break;
                case LinkTypes.HERITAGE:
                    strokeEndLength = 20;
                    break;
                case LinkTypes.AGGREGATION:
                case LinkTypes.COMPOSITION:
                    strokeEndLength = 30;
                    break;
                default:
                    strokeEndLength = 10;
                    break;
            }

            return strokeEndLength;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            center = linkStroke.GetCenter();

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

            linkStroke.path.Insert(indexInPath, new Coordinates(actualPos));
            linkStroke.addStylusPointsToLink();

            DrawingService.UpdateLinks(new StrokeCollection { linkStroke });

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