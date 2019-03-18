﻿using System.Windows;
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
    class LinkAnchorPointAdorner : Adorner
    {
        List<Thumb> anchors;
        VisualCollection visualChildren;

        // The center of the strokes.
        Point center;

        const int HANDLEMARGIN = 15;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public LinkStroke stroke;
        public CustomInkCanvas canvas;

        private Path linkPreview;
        LineGeometry linkPreviewGeom = new LineGeometry();


        public LinkAnchorPointAdorner(UIElement adornedElement, LinkStroke linkStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            linkPreview = new Path();
            linkPreview.Stroke = Brushes.Gray;
            linkPreview.StrokeThickness = 2;
            visualChildren.Add(linkPreview);

            stroke = linkStroke;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = linkStroke.GetBounds();
            center = stroke.GetCenter();

            anchors = new List<Thumb>();
            // Pour une ShapeStroke
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());

            foreach (Thumb anchor in anchors)
            {
                anchor.Cursor = Cursors.SizeNWSE;
                anchor.Width = 10;
                anchor.Height = 10;
                anchor.Background = Brushes.IndianRed;

                anchor.DragDelta += new DragDeltaEventHandler(dragHandle_DragDelta);
                anchor.DragCompleted += new DragCompletedEventHandler(dragHandle_DragCompleted);
                anchor.DragStarted += new DragStartedEventHandler(dragHandle_DragStarted);

                visualChildren.Add(anchor);
            }

            strokeBounds = linkStroke.GetBounds();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            center = stroke.GetCenter();

            List<Coordinates> strokePath = stroke.path;
            if (strokePath.Count > 1)
            {

                ArrangeAnchor(0, -center.X + strokePath[0].x, -center.Y + strokePath[0].y);
                ArrangeAnchor(1, -center.X + strokePath[strokePath.Count - 1].x, -center.Y + strokePath[strokePath.Count - 1].y);
            }

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
            canvas.addAnchorPoints();
            canvas.isUpdatingLink = true;
        }

        void dragHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point pos = Mouse.GetPosition(this);

            if ((sender as Thumb) == anchors[1])
            {
                linkPreviewGeom.StartPoint = new Point(stroke.path[0].x, stroke.path[0].y);
                linkPreviewGeom.EndPoint = pos;
            }
            else //if ((sender as Thumb) == anchors[0])
            {
                linkPreviewGeom.StartPoint = pos;
                linkPreviewGeom.EndPoint = new Point(stroke.path[stroke.path.Count - 1].x, stroke.path[stroke.path.Count - 1].y);
            }

            linkPreview.Data = linkPreviewGeom;
            linkPreview.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
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
                if (thumb.GetType() == typeof(StrokeAnchorPointThumb))
                {
                    Point thumbPosition = thumb.TransformToAncestor(canvas).Transform(new Point(0, 0));

                    StrokeAnchorPointThumb cheatThumb = thumb as StrokeAnchorPointThumb;
                    double y = thumbPosition.Y - actualPos.Y;
                    double x = thumbPosition.X - actualPos.X;

                    double distBetweenPoints = (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                    if (distBetweenPoints <= 30)
                    {
                        strokeTo = cheatThumb.stroke;
                        actualPos = thumbPosition;
                        number = cheatThumb.number;
                    }

                }
            }
            int linkStrokeAnchor = 0;
            if ((sender as Thumb) == anchors[1]) linkStrokeAnchor = 1;


            canvas.updateLink(linkStrokeAnchor, stroke, strokeTo?.guid.ToString(), number, actualPos);

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