using PolyPaint.CustomInk.Strokes;
using PolyPaint.Enums;
using PolyPaint.Templates;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolyPaint.CustomInk
{
    public class CustomInkCanvas : InkCanvas
    {
        private CustomDynamicRenderer customRenderer = new CustomDynamicRenderer();

        private StrokeCollection clipboard;

        public StylusPoint firstPoint;
        public bool isUpdatingLink = false;
        
        #region links
        
        public void updateLink(int linkStrokeAnchor, LinkStroke linkBeingUpdated, string strokeToAttachGuid, int strokeToAttachAnchor, Point pointPosition)
        {

            if (isUpdatingLink)
            {
                if (linkStrokeAnchor == 0)
                {
                    // mettre a jour la position du point initial (from)
                    linkBeingUpdated.path[0] = new Coordinates(pointPosition.X, pointPosition.Y);
                    linkBeingUpdated.from = new AnchorPoint(strokeToAttachGuid, strokeToAttachAnchor, "0");
                }
                else
                {
                    // mettre a jour la position du point final (to)
                    linkBeingUpdated.path[linkBeingUpdated.path.Count - 1] = new Coordinates(pointPosition.X, pointPosition.Y);
                    linkBeingUpdated.to = new AnchorPoint(strokeToAttachGuid, strokeToAttachAnchor, "0");
                }

                linkBeingUpdated.addStylusPointsToLink();
                RefreshChildren();
            }

        }
        
        public void RefreshLinks()
        {
            foreach (CustomStroke customStroke in Strokes)
            {
                if (isLinkStroke(customStroke))
                {
                    LinkStroke linkStroke = customStroke as LinkStroke;

                    if (linkStroke.isAttached())
                    {
                        if (SelectedStrokes.Count == 1 && SelectedStrokes.Contains(linkStroke)) { } // do nothing 
                        else
                        {
                            if (linkStroke.from.formId == null)
                            {
                                StylusPoint point = linkStroke.StylusPoints[0];
                                linkStroke.path[0] = new Coordinates(point.X, point.Y);
                            }
                            else if (linkStroke.to.formId == null)
                            {
                                StylusPoint point = linkStroke.StylusPoints[linkStroke.StylusPoints.Count - 1];
                                linkStroke.path[linkStroke.path.Count - 1] = new Coordinates(point.X, point.Y);
                            }


                            foreach (CustomStroke selectedStroke in SelectedStrokes)
                            {
                                if (linkStroke.from?.formId == selectedStroke.guid.ToString())
                                {
                                    Point fromPoint = linkStroke.GetFromPoint(this.Strokes);
                                    // mettre a jour les positions des points initial et final
                                    linkStroke.path[0] = new Coordinates(fromPoint.X, fromPoint.Y);
                                }
                                if (linkStroke.to?.formId == selectedStroke.guid.ToString())
                                {
                                    Point toPoint = linkStroke.GetToPoint(this.Strokes);
                                    // mettre a jour les positions des points initial et final
                                    linkStroke.path[linkStroke.path.Count - 1] = new Coordinates(toPoint.X, toPoint.Y);
                                }

                                linkStroke.addStylusPointsToLink();
                            }
                        }
                    }
                    else if (SelectedStrokes.Contains(linkStroke)) // update path if linkStroke is not attached and has been moved or resized
                    {
                        StylusPoint point = linkStroke.StylusPoints[0];
                        linkStroke.path[0] = new Coordinates(point.X, point.Y);

                        point = linkStroke.StylusPoints[linkStroke.StylusPoints.Count - 1];
                        linkStroke.path[linkStroke.path.Count - 1] = new Coordinates(point.X, point.Y);
                    }
                }
            }
        }
        #endregion

        #region StrokeType dependency property
        public string StrokeType
        {
            get { return (string)GetValue(StrokeTypeProperty); }
            set { SetValue(StrokeTypeProperty, value); }
        }
        public static readonly DependencyProperty StrokeTypeProperty = DependencyProperty.Register(
          "StrokeType", typeof(string), typeof(CustomInkCanvas), new PropertyMetadata("CLASS_SHAPE"));
        #endregion
        #region SelectedStrokes dependency property
        public StrokeCollection SelectedStrokes
        {
            get { return (StrokeCollection)GetValue(SelectedStrokesProperty); }
            set { SetValue(SelectedStrokesProperty, value); }
        }
        public static readonly DependencyProperty SelectedStrokesProperty = DependencyProperty.Register(
          "SelectedStrokes", typeof(StrokeCollection), typeof(CustomInkCanvas), new PropertyMetadata(new StrokeCollection()));
        #endregion

        public CustomInkCanvas() : base()
        {
            // Use the custom dynamic renderer on the custom InkCanvas.
            DynamicRenderer = customRenderer;

            clipboard = new StrokeCollection();
        }

        #region On.. event handlers
        protected override void OnSelectionChanging(InkCanvasSelectionChangingEventArgs e) {
            SelectedStrokes.Clear();
            foreach (Stroke stroke in e.GetSelectedStrokes())
            {
                SelectedStrokes.Add(stroke);
            }
            //base.OnSelectionChanging(e);
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
            RefreshChildren();
        }

        protected override void OnSelectionMoving(InkCanvasSelectionEditingEventArgs e)
        {
            base.OnSelectionMoving(e);
        }

        protected override void OnSelectionMoved(EventArgs e)
        {
            RefreshLinks();
            RefreshChildren();
        }

        protected override void OnSelectionResizing(InkCanvasSelectionEditingEventArgs e)
        {
            base.OnSelectionResizing(e);
        }

        protected override void OnSelectionResized(EventArgs e)
        {
            base.OnSelectionResized(e);
            RefreshChildren();
            RefreshLinks();
        }

        protected override void OnStrokeErased(RoutedEventArgs e)
        {
            base.OnStrokeErased(e);
        }

        protected override void OnStrokeErasing(InkCanvasStrokeErasingEventArgs e)
        {
            base.OnStrokeErasing(e);
        }
        #endregion

        #region OnStrokeCollected
        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            // Remove the original stroke and add a custom stroke.
            Strokes.Remove(e.Stroke);

            CustomStroke customStroke;
            StrokeTypes strokeType = (StrokeTypes) Enum.Parse(typeof(StrokeTypes), StrokeType);

            switch (strokeType)
            {
                case StrokeTypes.CLASS_SHAPE:
                    customStroke = new ClassStroke(e.Stroke.StylusPoints);
                    break;
                case StrokeTypes.ARTIFACT:
                    customStroke = new ArtifactStroke(e.Stroke.StylusPoints);
                    break;
                case StrokeTypes.ACTIVITY:
                    customStroke = new ActivityStroke(e.Stroke.StylusPoints);
                    break;
                case StrokeTypes.ROLE:
                    customStroke = new ActorStroke(e.Stroke.StylusPoints);
                    break;
                case StrokeTypes.COMMENT:
                    customStroke = new ClassStroke(e.Stroke.StylusPoints);
                    break;
                case StrokeTypes.PHASE:
                    customStroke = new ClassStroke(e.Stroke.StylusPoints);
                    break;
                case StrokeTypes.LINK:
                    customStroke = new LinkStroke(e.Stroke.StylusPoints);
                    break;
                default:
                    customStroke = new ClassStroke(e.Stroke.StylusPoints);
                    break;
               
            }
            Strokes.Add(customStroke);
            firstPoint = customStroke.StylusPoints[0];
            SelectedStrokes = new StrokeCollection { Strokes[Strokes.Count - 1] };

            //drawingService.UpdateShape("id", 0, "strokeName", shapeStyle, new List<string>(), new List<string>());

            // Pass the custom stroke to base class' OnStrokeCollected method.
            InkCanvasStrokeCollectedEventArgs args = new InkCanvasStrokeCollectedEventArgs(customStroke);
            base.OnStrokeCollected(args);
            
            AddTextBox(customStroke);
        }

        private void AddTextBox(CustomStroke stroke)
        {
            if(stroke.type == (int) StrokeTypes.CLASS_SHAPE)
            {
                Point point = stroke.GetBounds().TopLeft;
                double x = point.X;
                double y = point.Y;

                ClassStroke classStroke = (ClassStroke)stroke;
                ClassTextBox sp = new ClassTextBox(classStroke.name, classStroke.attributes, classStroke.methods);
                this.Children.Add(sp);
                InkCanvas.SetTop(sp, y);
                InkCanvas.SetLeft(sp, x);
            }
            else
            {
                Point point = stroke.GetBounds().BottomLeft;
                double x = point.X;
                double y = point.Y;

                CustomTextBox tb = new CustomTextBox();
                tb.Text = stroke.name;
                tb.Uid = stroke.guid.ToString();

                this.Children.Add(tb);
                InkCanvas.SetTop(tb, y);
                InkCanvas.SetLeft(tb, x);
            }
        }
        #endregion

        #region RotateStrokes
        public void RotateStrokes()
        {
            StrokeCollection strokes = GetSelectedStrokes();

            if (strokes.Count == 0)
                return;

            foreach (CustomStroke selectedStroke in strokes)
            {
                double rotation = selectedStroke.rotation;
                if (rotation.Equals(360))
                    rotation = 0;
                else
                    rotation += 10;
                Stroke newStroke = selectedStroke.CloneRotated(rotation);
                StrokeCollection newStrokes = new StrokeCollection();
                newStrokes.Add(newStroke);
                Strokes.Replace(selectedStroke, newStrokes);
            }
        }
        #endregion

        #region RotateStrokesWithAngle
        public void RotateStrokesWithAngle(double rotation)
        {
            StrokeCollection strokes = GetSelectedStrokes();
            StrokeCollection selectedNewStrokes = new StrokeCollection();

            foreach (CustomStroke selectedStroke in strokes)
            {
                //double rotation = selectedStroke.rotation;
                if (rotation.Equals(360))
                    rotation = 0;
                //else
                //    rotation += 10;
                Stroke newStroke = selectedStroke.CloneRotated(rotation);
                StrokeCollection newStrokes = new StrokeCollection { newStroke };
                Strokes.Replace(selectedStroke, newStrokes);

                selectedNewStrokes.Add(newStrokes);
            }

            //SelectedStrokes.Add(newStrokes); // non necessaire, pcq le .Select les ajoute 
            Select(selectedNewStrokes);
        }
        #endregion

        #region PasteStrokes
        public void PasteStrokes()
        {
            StrokeCollection strokes = GetSelectedStrokes();

            if (strokes.Count == 0)
            {
                // strokes from clipboard will be pasted
                strokes = clipboard;
            }

            StrokeCollection newStrokes = new StrokeCollection();

            foreach (Stroke stroke in strokes)
            {
                Stroke newStroke = stroke.Clone();

                // TODO : 2 options. 1- Avoir un compteur de Paste qui incremente a chaque Paste, le reinitialiser quand 
                // nouveau OnSelectionChanged. 2- Coller au coin du canvas
                // Voir quoi faire avec le client leger
                Matrix translateMatrix = new Matrix();
                translateMatrix.Translate(20.0, 20.0);
                newStroke.Transform(translateMatrix, false);

                Strokes.Add(newStroke);
                newStrokes.Add(newStroke);
            }

            Select(newStrokes);
        }
        #endregion

        #region CutStrokes
        public void CutStrokes()
        {
            StrokeCollection selection = GetSelectedStrokes();
            // put selection in clipboard to be able to paste it
            clipboard = selection;

            // cut selection from canvas
            CutSelection();

            // To delete the adorners
            RefreshChildren();
        }
        #endregion

        #region RefreshChildren
        public void RefreshChildren()
        {
            // ne fonctionne pas :( fait que des strokes ne sont plus ajoutees apres une 2e
            //removeAdorners();
            Children.Clear();

            isUpdatingLink = false;
            
            StrokeCollection selectedStrokes = new StrokeCollection();

            // Add selection adorners to selectedStrokes
            foreach (CustomStroke selectedStroke in GetSelectedStrokes())
            {
                selectedStrokes.Add(selectedStroke);
                addAdorners(selectedStroke);
            }

            // Add text boxes (names) to all strokes
            foreach (CustomStroke stroke in Strokes)
            {
                AddTextBox(stroke);
            }

            // refresh all the links
            //refreshLinks();

            Select(selectedStrokes);
        }

        private void addAdorners(CustomStroke selectedStroke)
        {
            Path path = new Path();
            path.Data = selectedStroke.GetGeometry();

            Children.Add(path);
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
            myAdornerLayer.Add(new EditionAdorner(path, selectedStroke, this));

            if (!isLinkStroke(selectedStroke))
            {
                myAdornerLayer.Add(new RotateAdorner(path, selectedStroke, this));
                myAdornerLayer.Add(new AnchorPointAdorner(path, selectedStroke, this));
            } else
            {
                myAdornerLayer.Add(new LinkAnchorPointAdorner(path, selectedStroke as LinkStroke, this));
            }

        }

        public void addAnchorPoints()
        {
            foreach (CustomStroke stroke in Strokes)
            {
                if(stroke.GetType() != typeof(LinkStroke))
                {
                    Path path = new Path();
                    path.Data = stroke.GetGeometry();

                    Children.Add(path);
                    AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
                    myAdornerLayer.Add(new AnchorPointAdorner(path, stroke, this));
                }
            }
        }

        // ne fonctionne pas :( fait que des strokes ne sont plus ajoutees apres une 2e stroke ajoutee
        private void removeAdorners()
        {
            for(int i = 0; i < Children.Count; i++)
            {
                if (Children[i].GetType() == typeof(Path))
                {
                    Children.RemoveAt(i);
                }
                else if (Children[i]?.GetType() == typeof(CustomTextBox))
                {
                    Children.RemoveAt(i);
                }

            }
            //List<UIElement> children = new List<UIElement>();

            //foreach (UIElement child in Children)
            //{
            //    if (child.GetType() == typeof(Path))
            //    {
            //        children.Add(child);
            //    }
            //}

            //foreach (UIElement child in children)
            //{
            //    Children.Remove(child);
            //}
        }
        #endregion

        private static bool isLinkStroke(Stroke stroke)
        {
            return stroke.GetType() == typeof(LinkStroke);
        }

    }
}
