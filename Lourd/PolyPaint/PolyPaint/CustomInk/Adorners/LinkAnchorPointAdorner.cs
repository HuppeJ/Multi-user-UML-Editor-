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
    class LinkAnchorPointAdorner : Adorner
    {
        List<Thumb> anchors;
        VisualCollection visualChildren;

        // The center of the strokes.
        Point center;

        const int HANDLEMARGIN = 15;

        // The bounds of the Strokes;
        Rect strokeBounds = Rect.Empty;
        public LinkStroke linkStroke;
        public CustomInkCanvas canvas;

        private Path linkPreview;
        LineGeometry linkPreviewGeom = new LineGeometry();
        int linkStrokeAnchor;
        CustomStroke anchoredShapeStroke = null;

        public LinkAnchorPointAdorner(UIElement adornedElement, LinkStroke linkStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            linkPreview = new Path();
            linkPreview.Stroke = Brushes.Gray;
            linkPreview.StrokeThickness = 2;
            visualChildren.Add(linkPreview);

            this.linkStroke = linkStroke;
            canvas = actualCanvas;
            linkStrokeAnchor = this.linkStroke.path.Count;

            strokeBounds = linkStroke.GetBounds();
            center = this.linkStroke.GetCenter();

            anchors = new List<Thumb>();
            // Pour une ShapeStroke
            for (int i = 0; i < this.linkStroke.path.Count; i++)
            {
                anchors.Add(new Thumb());
            }

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

            center = linkStroke.GetCenter();

            List<Coordinates> strokePath = linkStroke.path;

            for (int i = 0; i < linkStroke.path.Count; i++)
            {
                ArrangeAnchor(i, -center.X + strokePath[i].x, -center.Y + strokePath[i].y);
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
            if (anchorNumber < anchors.Count)
            {
                anchors[anchorNumber].Arrange(handleRect);
            }
        }

        void dragHandle_DragStarted(object sender, DragStartedEventArgs e)
        {
            for (int i = 0; i < linkStroke.path.Count && linkStrokeAnchor == linkStroke.path.Count; i++)
            {
                if ((sender as Thumb) == anchors[i]) linkStrokeAnchor = i;
            }
            if(linkStrokeAnchor == 0 || linkStrokeAnchor == linkStroke.path.Count - 1)
            {
                canvas.addAnchorPoints();
                string formId = "";
                if(linkStrokeAnchor == 0)
                {
                    formId = linkStroke.from?.formId;
                } else
                {
                    formId = linkStroke.to?.formId;
                }
                canvas.StrokesDictionary.TryGetValue(formId, out anchoredShapeStroke);
            }
            canvas.isUpdatingLink = true;
        }

        void dragHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point pos = Mouse.GetPosition(this);

            if ((sender as Thumb) == anchors[1])
            {
                linkPreviewGeom.StartPoint = linkStroke.path[linkStroke.path.Count - 2].ToPoint();
                linkPreviewGeom.EndPoint = pos;
            }
            else if ((sender as Thumb) == anchors[0])
            {
                linkPreviewGeom.StartPoint = pos;
                linkPreviewGeom.EndPoint = linkStroke.path[1].ToPoint();
            }

            linkPreview.Data = linkPreviewGeom;
            linkPreview.Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
        }


        void dragHandle_DragCompleted(object sender,
                                        DragCompletedEventArgs e)
        {
            Point actualPos = Mouse.GetPosition(this);
            if (actualPos.X < 0 || actualPos.Y < 0)
            {
                canvas.isUpdatingLink = false;
                visualChildren.Remove(linkPreview);
                InvalidateArrange();
                return;
            }

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
                    if (distBetweenPoints <= 10)
                    {
                        strokeTo = cheatThumb.stroke;
                        actualPos = thumbPosition;
                        number = cheatThumb.number;
                    }
                }
            }

            if (anchoredShapeStroke != null)
            {
                if(linkStrokeAnchor == 0) //from is changing
                {

                }
                else // to is changing
                {

                }
            }

            if (shapeStroke.strokeType == (int)StrokeTypes.ROLE)
            {
                if (strokeTo?.GetType() == typeof(ActivityStroke))
                    CreateLink(actualPos, strokeTo, number, linkAnchorNumber, LinkTypes.ONE_WAY_ASSOCIATION, pos);
                else
                    MessageBox.Show("A role can only be linked to an activity.");
            }
            else if (shapeStroke.strokeType == (int)StrokeTypes.ARTIFACT)
            {
                if (strokeTo?.GetType() == typeof(ActivityStroke))
                    CreateLink(actualPos, strokeTo, number, linkAnchorNumber, LinkTypes.ONE_WAY_ASSOCIATION, pos);
                else
                    MessageBox.Show("An artifact can only be linked to an activity.");
            }
            else if (shapeStroke.strokeType == (int)StrokeTypes.ACTIVITY)
            {
                if (strokeTo?.GetType() == typeof(ArtifactStroke))
                    CreateLink(actualPos, strokeTo, number, linkAnchorNumber, LinkTypes.ONE_WAY_ASSOCIATION, pos);
                else
                    MessageBox.Show("An activity can only be linked to an artifact.");
            }
            else if (strokeTo?.GetType() == typeof(ArtifactStroke) || strokeTo?.GetType() == typeof(ActorStroke) || strokeTo?.GetType() == typeof(ActivityStroke))
            {
                MessageBox.Show("Cannot create link.");
            }
            else
            {
                CreateLink(actualPos, strokeTo, number, linkAnchorNumber, LinkTypes.LINE, pos);
            }


            canvas.updateLink(linkStrokeAnchor, linkStroke, strokeTo as ShapeStroke, number, actualPos);

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