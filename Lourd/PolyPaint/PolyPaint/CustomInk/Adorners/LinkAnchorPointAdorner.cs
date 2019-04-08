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
using PolyPaint.CustomInk.Adorners;
using PolyPaint.Vues;
using System.Windows.Ink;

namespace PolyPaint.CustomInk
{
    class LinkAnchorPointAdorner : CustomAdorner
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
        CustomStroke anchoredShapeStrokeToUpdate = null;

        private bool hasFailed = false;

        public LinkAnchorPointAdorner(UIElement adornedElement, LinkStroke linkStroke, CustomInkCanvas actualCanvas)
            : base(adornedElement)
        {
            adornedStroke = linkStroke;

            visualChildren = new VisualCollection(this);

            linkPreview = new Path();
            linkPreview.Stroke = Brushes.Gray;
            linkPreview.StrokeThickness = 2;
            visualChildren.Add(linkPreview);

            this.linkStroke = linkStroke;
            canvas = actualCanvas;
            linkStrokeAnchor = this.linkStroke.path.Count;

            strokeBounds = linkStroke.GetStraightBounds();
            center = this.linkStroke.GetCenter();

            anchors = new List<Thumb>();
            // Pour une ShapeStroke
            for (int i = 0; i < this.linkStroke.path.Count; i++)
            {
                anchors.Add(new Thumb());
            }
            int index = 0;
            foreach (Thumb anchor in anchors)
            {
                anchor.Cursor = Cursors.ScrollAll;
                anchor.Width = 10;
                anchor.Height = 10;
                //anchor.Background = (Brush)new BrushConverter().ConvertFromString("#809dce");

                anchor.Background = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FFDDDDDD"),
                    (Color)ColorConverter.ConvertFromString("#809dce"), 45);

                anchor.DragDelta += new DragDeltaEventHandler(dragHandle_DragDelta);
                anchor.DragCompleted += new DragCompletedEventHandler(dragHandle_DragCompleted);
                anchor.DragStarted += new DragStartedEventHandler(dragHandle_DragStarted);

                SetAnchorRenderTransfrom(index, linkStroke.path[index].x, linkStroke.path[index].y);

                visualChildren.Add(anchor);
                index++;
            }
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
            // Draws the thumb and the rectangle around the strokes.
            if (anchorNumber < anchors.Count)
            {
                anchors[anchorNumber].Arrange(new Rect(new Size(canvas.ActualWidth, canvas.ActualHeight)));
            }
        }

        private void SetAnchorRenderTransfrom(int anchorNumber, double xOffset, double yOffset)
        {

            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(linkStroke.rotation, anchors[anchorNumber].Width / 2, anchors[anchorNumber].Height / 2));
            transform.Children.Add(new TranslateTransform(-canvas.ActualWidth / 2 + xOffset,
               -canvas.ActualHeight / 2 + yOffset));
            anchors[anchorNumber].RenderTransform = transform;
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
                string formIdToUpdate = "";
                string formId = "";
                if(linkStrokeAnchor == 0)
                {
                    formIdToUpdate = linkStroke.from?.formId;
                    formId = linkStroke.to?.formId;
                } else
                {
                    formIdToUpdate = linkStroke.to?.formId;
                    formId = linkStroke.from?.formId;
                }
                if(formIdToUpdate != null)
                {
                    canvas.StrokesDictionary.TryGetValue(formIdToUpdate, out anchoredShapeStrokeToUpdate);
                }
                if (formId != null)
                {
                    canvas.StrokesDictionary.TryGetValue(formId, out anchoredShapeStroke);
                }
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

            if(linkStroke.path.Count > 2)
            {
                linkPreview.Stroke = Brushes.Transparent;
                linkPreview.StrokeThickness = 0;
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

            CustomStroke newAnchoredStroke = null;
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
                        newAnchoredStroke = cheatThumb.stroke;
                        actualPos = thumbPosition;
                        number = cheatThumb.number;
                    }
                }
            }


            if (newAnchoredStroke != null)
            {
                //anchoredShapeStroke
                if (linkStrokeAnchor == 0) //from is changing
                {
                    if (anchoredShapeStroke != null) // there is a to
                    {
                        if (anchoredShapeStroke.isProccessStroke())
                        {
                            if (anchoredShapeStroke is ActorStroke)
                            {
                                ShowMessage("No shape can point to a role");
                            }
                            else if (anchoredShapeStroke is ActivityStroke)
                            {
                                if (newAnchoredStroke.GetType() != typeof(ActorStroke) &&
                                    newAnchoredStroke.GetType() != typeof(ArtifactStroke))
                                    ShowMessage("Only roles or artifacts can point to an activity.");
                            }
                            else if (anchoredShapeStroke is ArtifactStroke)
                            {
                                ShowMessage("A link cannot point to a role");
                            }
                        }
                        else if (anchoredShapeStroke is ClassStroke && linkStroke.isLinkAggCompHeritage())
                        {
                            if (!(newAnchoredStroke is ClassStroke))
                            {
                                ShowMessage("Aggregation, composition and heritage links must be between two classes.");
                            }
                        }
                        else if (newAnchoredStroke.isProccessStroke())
                        {
                            ShowMessage("Roles, artifacts and activities must be linked between themselves.");
                        }
                    }
                    else // there is no to
                    {
                        noNewStrokeLinkedToProcessOrClassCheck(newAnchoredStroke);
                    }
                }
                else // to is changing
                {
                    CustomStroke fromStroke = anchoredShapeStroke;
                    if (fromStroke != null) // there is a from
                    {
                        if (fromStroke is ActorStroke)
                        {
                            if (!(newAnchoredStroke is ActivityStroke))
                                ShowMessage("A role can only be linked to an activity.");
                        }
                        else if (fromStroke is ArtifactStroke)
                        {
                            if (!(newAnchoredStroke is ActivityStroke))
                                ShowMessage("An artifact can only be linked to an activity.");
                        }
                        else if (fromStroke is ActivityStroke)
                        {
                            if (!(newAnchoredStroke is ArtifactStroke))
                                ShowMessage("An activity can only be linked to an artifact.");
                        }
                        else if (newAnchoredStroke != null && newAnchoredStroke.isProccessStroke())
                        {
                            ShowMessage("Roles, artifacts and activities must be linked between themselves.");
                        }
                        else if (fromStroke is ClassStroke && linkStroke.isLinkAggCompHeritage())
                        {
                            if (!(newAnchoredStroke is ClassStroke))
                            {
                                ShowMessage("Aggregation, composition and heritage links must be between two classes.");
                            }
                        }
                    }
                    else // there is no from
                    {
                        noNewStrokeLinkedToProcessOrClassCheck(newAnchoredStroke);
                    }
                }
            }
            else // no newAnchoredStroke
            {
                if (anchoredShapeStroke != null && anchoredShapeStroke.isProccessStroke())
                {
                    ShowMessage("Roles, artifacts and activities cannot be linked to nothing.");
                }

                if (anchoredShapeStroke != null && anchoredShapeStroke is ClassStroke && linkStroke.isLinkAggCompHeritage())
                {
                    ShowMessage("Aggregation, composition and heritage links must be between two classes.");
                }

                //if (linkStrokeAnchor == 0) //from is changing to nothing. No from
                //{
                //}
                //else //to is changing to nothing. Not to.
                //{

                //}
            }
            // voir fonction du anchorPointAdorner

            if (!hasFailed)
            {
                canvas.updateLink(linkStrokeAnchor, linkStroke, newAnchoredStroke as ShapeStroke, number, actualPos);
            } else
            {
                StrokeCollection selectedStrokes = new StrokeCollection { canvas.GetSelectedStrokes() };
                canvas.Select(new StrokeCollection { });
                canvas.Select(selectedStrokes);
            }


            //TODO eliminate preview
            visualChildren.Remove(linkPreview);

            InvalidateArrange();
        }

        private void noNewStrokeLinkedToProcessOrClassCheck(CustomStroke newAnchoredStroke)
        {
            if (newAnchoredStroke.isProccessStroke())
            {
                ShowMessage("Roles, artifacts and activities cannot be linked to nothing.");
            }
            else if (newAnchoredStroke is ClassStroke && linkStroke.isLinkAggCompHeritage())
            {
                ShowMessage("Aggregation, composition and heritage links must be between two classes.");
            }
        }

        private void ShowMessage(string message)
        {
            hasFailed = true;

            var parent = canvas.Parent;
            while (!(parent is WindowDrawing))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            WindowDrawing windowDrawing = (WindowDrawing)parent;
            if (windowDrawing != null)
            {
                windowDrawing.OpenMessagePopup(message);
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