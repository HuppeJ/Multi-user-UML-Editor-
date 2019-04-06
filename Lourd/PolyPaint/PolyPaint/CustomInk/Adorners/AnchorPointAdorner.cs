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
using PolyPaint.Enums;
using PolyPaint.CustomInk.Adorners;

namespace PolyPaint.CustomInk
{
    class AnchorPointAdorner : CustomAdorner
    {
        List<Thumb> anchors;
        List<StrokeAnchorPointThumb> cheatAnchors;

        VisualCollection visualChildren;
        
        const int HANDLEMARGIN = 3;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public ShapeStroke shapeStroke;

        public CustomInkCanvas canvas;

        RotateTransform rotation;

        private Path linkPreview;
        LineGeometry linkPreviewGeom = new LineGeometry();

        public AnchorPointAdorner(UIElement adornedElement, CustomStroke customStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            adornedStroke = customStroke;

            visualChildren = new VisualCollection(this);

            linkPreview = new Path();
            linkPreview.Stroke = Brushes.Gray;
            linkPreview.StrokeThickness = 2;
            visualChildren.Add(linkPreview);

            shapeStroke = customStroke as ShapeStroke;
            canvas = actualCanvas;
            // rotation initiale de la stroke (pour dessiner le rectangle)
            // Bug. Cheat, but the geometry, the selection Rectangle (newRect) should be the right one.. geom of the stroke?
            strokeBounds = customStroke.GetCustomBound();
            Point center = customStroke.GetCenter();
            rotation = new RotateTransform((customStroke as ShapeStroke).shapeStyle.rotation, center.X, center.Y);

            anchors = new List<Thumb>();
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());
            anchors.Add(new Thumb());

            foreach (Thumb anchor in anchors)
            {
                anchor.Cursor = Cursors.ScrollAll;
                anchor.Width = 6;
                anchor.Height = 6;
                anchor.Background = Brushes.IndianRed;

                anchor.DragStarted += new DragStartedEventHandler(dragHandle_DragStarted);
                anchor.DragDelta += new DragDeltaEventHandler(dragHandle_DragDelta);
                anchor.DragCompleted += new DragCompletedEventHandler(dragHandle_DragCompleted);

                visualChildren.Add(anchor);
            }

            cheatAnchors = new List<StrokeAnchorPointThumb>();
            cheatAnchors.Add(new StrokeAnchorPointThumb(shapeStroke, canvas, 0));
            cheatAnchors.Add(new StrokeAnchorPointThumb(shapeStroke, canvas, 1));
            cheatAnchors.Add(new StrokeAnchorPointThumb(shapeStroke, canvas, 2));
            cheatAnchors.Add(new StrokeAnchorPointThumb(shapeStroke, canvas, 3));
            foreach (Thumb cheatAnchor in cheatAnchors)
            {
                cheatAnchor.Cursor = Cursors.SizeNWSE;
                cheatAnchor.Width = 1;
                cheatAnchor.Height = 1;

                canvas.Children.Add(cheatAnchor);
            }

            strokeBounds = customStroke.GetCustomBound();

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (strokeBounds.IsEmpty)
            {
                return finalSize;
            }

            ArrangeAnchor(0, 0, -(strokeBounds.Height / 2 + HANDLEMARGIN));
            ArrangeAnchor(1, strokeBounds.Width / 2 + HANDLEMARGIN, 0);
            ArrangeAnchor(2, 0, strokeBounds.Height / 2 + HANDLEMARGIN);
            ArrangeAnchor(3, -(strokeBounds.Width / 2 + HANDLEMARGIN), 0);

            linkPreview.Arrange(new Rect(finalSize));

            return finalSize;
        }

        private void ArrangeAnchor(int anchorNumber, double xOffset, double yOffset)
        {
            // The rectangle that determines the position of the Thumb.
            Rect handleRect = new Rect(strokeBounds.X + xOffset,
                                  strokeBounds.Y + yOffset,
                                  strokeBounds.Width,
                                  strokeBounds.Height);
            handleRect.Transform(rotation.Value);

            // Draws the thumb and the rectangle around the strokes.
            anchors[anchorNumber].Arrange(handleRect);
            cheatAnchors[anchorNumber].Arrange(handleRect);
        }

        void dragHandle_DragStarted(object sender,
                                        DragStartedEventArgs e)
        {
            canvas.addAnchorPoints();
            linkPreviewGeom.StartPoint = Mouse.GetPosition(this);
        }

        void dragHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            linkPreviewGeom.EndPoint = Mouse.GetPosition(this);

            linkPreview.Data = linkPreviewGeom;
            linkPreview.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
        }

        void dragHandle_DragCompleted(object sender,
                                        DragCompletedEventArgs e)
        {
            Point actualPos = Mouse.GetPosition(this);
            if (actualPos.X < 0 || actualPos.Y < 0)
            {
                visualChildren.Remove(linkPreview);
                InvalidateArrange();
                return;
            }
            ShapeStroke strokeTo = null;
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
                    if (distBetweenPoints <= 10)
                    {
                        strokeTo = cheatThumb.stroke as ShapeStroke;
                        actualPos = thumbPosition;
                        number = cheatThumb.number;
                    }

                }
            }


            int linkAnchorNumber = 0;
            if (sender as Thumb == anchors[1]) linkAnchorNumber = 1;
            if (sender as Thumb == anchors[2]) linkAnchorNumber = 2;
            if (sender as Thumb == anchors[3]) linkAnchorNumber = 3;
            Point pos = (sender as Thumb).TransformToAncestor(canvas).Transform(new Point(0, 0));
            pos.X += 5;
            pos.Y += 5;

            if(shapeStroke is ActorStroke)
            {
                if (strokeTo is ActivityStroke)
                    CreateLink(actualPos, strokeTo, number, linkAnchorNumber, LinkTypes.ONE_WAY_ASSOCIATION, pos);
                else
                    MessageBox.Show("A role can only be linked to an activity.");
            } else if (shapeStroke is ArtifactStroke)
            {
                if (strokeTo is ActivityStroke)
                    CreateLink(actualPos, strokeTo, number, linkAnchorNumber, LinkTypes.ONE_WAY_ASSOCIATION, pos);
                else
                    MessageBox.Show("An artifact can only be linked to an activity.");
            } else if (shapeStroke is ActivityStroke)
            {
                if (strokeTo is ArtifactStroke)
                    CreateLink(actualPos, strokeTo, number, linkAnchorNumber, LinkTypes.ONE_WAY_ASSOCIATION, pos);
                else
                    MessageBox.Show("An activity can only be linked to an artifact.");
            } else if (strokeTo != null && strokeTo.isProccessStroke())
            {
                MessageBox.Show("Cannot create link.");
            }
            else
            {
                CreateLink(actualPos, strokeTo, number, linkAnchorNumber, LinkTypes.LINE, pos);
            }
            
            visualChildren.Remove(linkPreview);
            InvalidateArrange();
        }
        
        private void CreateLink(Point actualPos, ShapeStroke strokeTo, int number, int linkAnchorNumber, LinkTypes linkType, Point pos)
        {
            LinkStroke linkBeingCreated = new LinkStroke(pos, shapeStroke?.guid.ToString(), linkAnchorNumber, linkType, new StylusPointCollection { new StylusPoint(0, 0) });
            shapeStroke?.linksFrom.Add(linkBeingCreated.guid.ToString());

            linkBeingCreated.addToPointToLink(actualPos, strokeTo?.guid.ToString(), number);
            strokeTo?.linksTo.Add(linkBeingCreated.guid.ToString());

            canvas.AddStroke(linkBeingCreated);
            DrawingService.CreateLink(linkBeingCreated);

            StrokeCollection shapesToUpdate = new StrokeCollection();
            if (shapeStroke != null)
            {
                shapesToUpdate.Add(shapeStroke);
            }
            if (strokeTo != null)
            {
                shapesToUpdate.Add(strokeTo);
            }
            DrawingService.UpdateShapes(shapesToUpdate);

            // canvas.Select(new StrokeCollection { linkBeingCreated });
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