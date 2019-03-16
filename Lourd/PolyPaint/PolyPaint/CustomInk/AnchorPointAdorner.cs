using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using PolyPaint.CustomInk.Strokes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System;

namespace PolyPaint.CustomInk
{
    class AnchorPointAdorner : Adorner
    {
        List<CustomButton> buttons;
        List<Thumb> anchors;
        List<StrokeAnchorPointThumb> cheatAnchors;

        VisualCollection visualChildren;

        // The center of the strokes.
        Point center;

        // The linkLine
        Path line;

        RotateTransform rotation;
        const int HANDLEMARGIN = 15;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public CustomStroke stroke;

        public CustomInkCanvas canvas;

        public AnchorPointAdorner(UIElement adornedElement, CustomStroke customStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            stroke = customStroke;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = customStroke.GetBounds();
            center = stroke.GetCenter();
            rotation = new RotateTransform(stroke.rotation, center.X, center.Y);

            anchors = new List<Thumb>();
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
           

            visualChildren = new VisualCollection(this);
            foreach (Thumb anchor in anchors)
            {
                anchor.Cursor = Cursors.SizeNWSE;
                anchor.Width = 10;
                anchor.Height = 10;
                anchor.Background = Brushes.IndianRed;

                anchor.DragStarted += new DragStartedEventHandler(dragHandle_DragStarted);
                anchor.DragDelta += new DragDeltaEventHandler(dragHandle_DragDelta);
                anchor.DragCompleted += new DragCompletedEventHandler(dragHandle_DragCompleted);

                visualChildren.Add(anchor);
            }

            cheatAnchors = new List<StrokeAnchorPointThumb>();
            cheatAnchors.Add(new StrokeAnchorPointThumb(customStroke, canvas, 0));
            cheatAnchors.Add(new StrokeAnchorPointThumb(customStroke, canvas, 1));
            cheatAnchors.Add(new StrokeAnchorPointThumb(customStroke, canvas, 2));
            cheatAnchors.Add(new StrokeAnchorPointThumb(customStroke, canvas, 3));
            foreach (Thumb cheatAnchor in cheatAnchors)
            {
                cheatAnchor.Cursor = Cursors.SizeNWSE;
                cheatAnchor.Width = 10;
                cheatAnchor.Height = 10;
                
                canvas.Children.Add(cheatAnchor);
            }

            strokeBounds = customStroke.GetBounds();
            line = new Path();
            visualChildren.Add(line);

        }

        /// <summary>
        /// Draw the rotation handle and the outline of
        /// the element.
        /// </summary>
        /// <param name="finalSize">The final area within the 
        /// parent that this element should use to arrange 
        /// itself and its children.</param>
        /// <returns>The actual size used. </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            center = stroke.GetCenter();

            ArrangeAnchor(0, 0, -(strokeBounds.Height / 2 + HANDLEMARGIN));
            ArrangeAnchor(1, strokeBounds.Width / 2 + HANDLEMARGIN, 0);
            ArrangeAnchor(2, 0, strokeBounds.Height / 2 + HANDLEMARGIN);
            ArrangeAnchor(3, -(strokeBounds.Width / 2 + HANDLEMARGIN), 0);

            return finalSize;
        }

        private void ArrangeAnchor(int anchorNumber, double xOffset, double yOffset)
        {
            // The rectangle that determines the position of the Thumb.
            Rect handleRect = new Rect(strokeBounds.X + xOffset,
                                  strokeBounds.Y + yOffset,
                                  strokeBounds.Width,
                                  strokeBounds.Height);

            if (rotation != null)
            {
                handleRect.Transform(rotation.Value);
            }

            // Draws the thumb and the rectangle around the strokes.
            anchors[anchorNumber].Arrange(handleRect);
            cheatAnchors[anchorNumber].Arrange(handleRect);
        }

        void dragHandle_DragStarted(object sender,
                                        DragStartedEventArgs e)
        {
            //e.HorizontalChange, e.VerticalChange;
            canvas.addAnchorPoints();

            int number = 0;
            if (sender as Thumb == anchors[1]) number = 1;
            if (sender as Thumb == anchors[2]) number = 2;
            if (sender as Thumb == anchors[3]) number = 3;
            Point pos = Mouse.GetPosition(this);

            canvas.createLink(stroke, number, pos);


            InvalidateArrange();
        }

        void dragHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {

            //Point pos = Mouse.GetPosition(this);

            //double deltaX = pos.X - center.X;
            //double deltaY = pos.Y - center.Y;

            //if (deltaY.Equals(0) && deltaX.Equals(0))
            //{
            //    return;
            //}

            //int number = 0;
            //if (sender as Thumb == anchors[1]) number = 1;
            //if (sender as Thumb == anchors[2]) number = 2;
            //if (sender as Thumb == anchors[3]) number = 3;

            //Point initialPosition = anchors[number].TransformToAncestor(canvas).Transform(new Point(0, 0));

            //// works!!
            //visualChildren.Remove(line);
            //line.Data = new LineGeometry(initialPosition,
            //                             pos);
            //line.Stroke = Brushes.Blue;
            //line.StrokeThickness = 1;
            //visualChildren.Add(line);

            //line.RenderTransform = rotation;
        }

        void dragHandle_DragCompleted(object sender,
                                        DragCompletedEventArgs e)
        {
            //e.HorizontalChange, e.VerticalChange;

            // Redraw rotateHandle.
                        
            Point actualPos = Mouse.GetPosition(this);

            CustomStroke strokeTo = null;
            int number = 0;


            foreach (UIElement thumb in canvas.Children)
            {
                if (thumb.GetType() == typeof(StrokeAnchorPointThumb)) {
                    Point thumbPosition = thumb.TransformToAncestor(canvas).Transform(new Point(0, 0));

                    StrokeAnchorPointThumb cheatThumb = thumb as StrokeAnchorPointThumb;
                    double y = thumbPosition.Y - actualPos.Y;
                    double x = thumbPosition.X - actualPos.X;

                    double distBetweenPoints = (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                    if(distBetweenPoints <= 30)
                    {
                        strokeTo = cheatThumb.stroke;
                        actualPos = thumbPosition;
                        number = cheatThumb.number;
                    }

                }
            }
            canvas.createLink(strokeTo, number, actualPos);


            InvalidateArrange();
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