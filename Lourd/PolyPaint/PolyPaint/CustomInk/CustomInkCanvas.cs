using PolyPaint.CustomInk.Strokes;
using PolyPaint.Enums;
using PolyPaint.Services;
using PolyPaint.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Dictionary<string, CustomStroke> StrokesDictionary = new Dictionary<string, CustomStroke>();

        private StrokeCollection clipboard;
        private Templates.Canvas canvas;
        StrokeCollection oldSelectedStrokes = new StrokeCollection();

        public StylusPoint firstPoint;
        public bool isUpdatingLink = false;

        public List<string> remoteSelectionIds = new List<string>();
        private StrokeCollection beingSelected = new StrokeCollection();
        private PathFigure selectionPath = new PathFigure();
        private Size lastSize = new Size(0,0);

        #region Dictonary
        public void AddStroke(CustomStroke stroke)
        {
            Strokes.Add(stroke);
            StrokesDictionary[stroke.guid.ToString()] = stroke;
        }

        public void RemoveStroke(CustomStroke stroke)
        {
            StrokesDictionary.Remove(stroke.guid.ToString());
            Strokes.Remove(stroke);
        }

        public void ReplaceStrokes(Stroke selectedStroke, StrokeCollection newStrokes)
        {
            foreach (CustomStroke stroke in newStrokes)
            {
                StrokesDictionary[stroke.guid.ToString()] = stroke;
            }
            Strokes.Replace(selectedStroke, newStrokes);
        }
        #endregion

        #region Links
        public void updateLink(int linkStrokeAnchor, LinkStroke linkBeingUpdated, ShapeStroke strokeToAttach, int strokeToAttachAnchor, Point pointPosition)
        {

            if (isUpdatingLink)
            {
                linkBeingUpdated.path[linkStrokeAnchor] = new Coordinates(pointPosition);

                if (linkStrokeAnchor == 0)
                {
                    RemoveShapeStrokeLinkFrom(linkBeingUpdated);

                    // mettre a jour la position du point initial (from)
                    linkBeingUpdated.from = new AnchorPoint(strokeToAttach?.guid.ToString(), strokeToAttachAnchor, "");
                    strokeToAttach?.linksFrom.Add(linkBeingUpdated.guid.ToString());

                }
                else if (linkStrokeAnchor == linkBeingUpdated.path.Count - 1)
                {
                    RemoveShapeStrokeLinkTo(linkBeingUpdated);

                    // mettre a jour la position du point final (to)
                    linkBeingUpdated.to = new AnchorPoint(strokeToAttach?.guid.ToString(), strokeToAttachAnchor, "");
                    strokeToAttach?.linksTo.Add(linkBeingUpdated.guid.ToString());
                }

                linkBeingUpdated.addStylusPointsToLink();
                RefreshChildren();
            }

        }

        private void RemoveShapeStrokeLinkTo(LinkStroke linkBeingUpdated)
        {
            if (linkBeingUpdated.to?.formId != null)
            {
                CustomStroke shapeStrokeTo;
                if (StrokesDictionary.TryGetValue(linkBeingUpdated.to?.formId, out shapeStrokeTo))
                {
                    (shapeStrokeTo as ShapeStroke).linksTo.Remove(linkBeingUpdated.guid.ToString());
                }
            }
        }

        private void RemoveShapeStrokeLinkFrom(LinkStroke linkBeingUpdated)
        {
            if (linkBeingUpdated.from?.formId != null)
            {
                CustomStroke shapeStrokeFrom;
                if (StrokesDictionary.TryGetValue(linkBeingUpdated.from?.formId, out shapeStrokeFrom))
                {
                    (shapeStrokeFrom as ShapeStroke).linksFrom.Remove(linkBeingUpdated.guid.ToString());
                }
            }
        }

        public void RefreshLinks()
        {
            foreach (CustomStroke customStroke in Strokes)
            {
                if (customStroke.isLinkStroke())
                {
                    LinkStroke linkStroke = customStroke as LinkStroke;

                    if (linkStroke.isAttached())
                    {
                        if (SelectedStrokes.Count == 1 && SelectedStrokes.Contains(linkStroke))
                        {
                            // keep the same stylus points if linkstroke is attached and is the only one moved
                            linkStroke.addStylusPointsToLink();
                        }
                        else
                        {
                            // si plusieurs points dans le path, les mettre a jour si la selectedStroke a bouge
                            if (SelectedStrokes.Contains(linkStroke))
                            {
                                List<Coordinates> pathCopy = new List<Coordinates>(linkStroke.path);

                                StylusPoint point = linkStroke.StylusPoints[0];
                                double xDiff = point.X - linkStroke.path[0].x;
                                double yDiff = point.Y - linkStroke.path[0].y;

                                for (int i = 1; i < linkStroke.path.Count - 1; i++)
                                {
                                    linkStroke.path[i] = new Coordinates(linkStroke.path[i].x + xDiff, linkStroke.path[i].y + yDiff);
                                }
                            }
                            // update the free points of linkStrokes (from view)
                            if (linkStroke.from?.formId == null)
                            {
                                StylusPoint point = linkStroke.StylusPoints[0];
                                linkStroke.path[0] = new Coordinates(point.ToPoint());
                            }
                            else if (linkStroke.to?.formId == null)
                            {
                                StylusPoint point = linkStroke.StylusPoints[linkStroke.StylusPoints.Count - 1];
                                linkStroke.path[linkStroke.path.Count - 1] = new Coordinates(point.ToPoint());
                            }

                            // move the attached points of linkStrokes
                            foreach (CustomStroke selectedStroke in SelectedStrokes)
                            {
                                if (linkStroke.from?.formId == selectedStroke.guid.ToString())
                                {
                                    Point fromPoint = linkStroke.GetFromPoint(this.Strokes);
                                    // mettre a jour les positions des points initial et final
                                    linkStroke.path[0] = new Coordinates(fromPoint);
                                }
                                if (linkStroke.to?.formId == selectedStroke.guid.ToString())
                                {
                                    Point toPoint = linkStroke.GetToPoint(this.Strokes);
                                    // mettre a jour les positions des points initial et final
                                    linkStroke.path[linkStroke.path.Count - 1] = new Coordinates(toPoint);
                                }

                            }

                            // update the linkstroke (view)
                            linkStroke.addStylusPointsToLink();
                        }
                    }
                    else if (SelectedStrokes.Contains(linkStroke)) // update path if linkStroke is not attached and has been moved or resized
                    {
                        StylusPoint point = linkStroke.StylusPoints[0];
                        double xDiff = point.X - linkStroke.path[0].x;
                        double yDiff = point.Y - linkStroke.path[0].y;
                        linkStroke.path[0] = new Coordinates(point.ToPoint());

                        point = linkStroke.StylusPoints[linkStroke.StylusPoints.Count - 1];
                        linkStroke.path[linkStroke.path.Count - 1] = new Coordinates(point.ToPoint());

                        // si plusieurs points dans le path. gi ne fonctionne pas pour le resize :/
                        for (int i = 1; i < linkStroke.path.Count - 1; i++)
                        {
                            linkStroke.path[i] = new Coordinates(linkStroke.path[i].x + xDiff, linkStroke.path[i].y + yDiff);
                        }
                    }
                }
            }
        }

        internal void modifyLinkStrokePath(LinkStroke linkStroke, Point mousePosition)
        {
            if (linkStroke.ContainsPoint(mousePosition))
            {
                int index = linkStroke.GetIndexforNewPoint(mousePosition);
                Path path = new Path();
                path.Data = linkStroke.GetGeometry();

                Children.Add(path);
                AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
                myAdornerLayer.Add(new LinkElbowAdorner(mousePosition, index, path, linkStroke, this));
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

            /*
            canvas = new Templates.Canvas(Guid.NewGuid().ToString(), "qwe", ConnectionService.username,
                ConnectionService.username, 0, null, new List<BasicShape>(), new List<Link>(), new int[] { 1, 1 });
            DrawingService.CreateCanvas(canvas);
            DrawingService.JoinCanvas("qwe");
            */

            DrawingService.AddStroke += OnRemoteStroke;
            DrawingService.RemoveStrokes += OnRemoveStrokes;
            DrawingService.UpdateStroke += OnUpdateStroke;
            DrawingService.UpdateSelection += OnRemoteSelection;
            DrawingService.UpdateDeselection += OnRemoteDeselection;
            DrawingService.CanvasRoomJoined += RefreshLinks;
            DrawingService.OnResizeCanvas += OnResizeCanvas;
        }

        private void OnResizeCanvas(Coordinates dimensions)
        {
            Width = dimensions.x;
            Height = dimensions.y;
        }

        #region On.. event handlers
        protected override void OnSelectionChanging(InkCanvasSelectionChangingEventArgs e) {
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
            StrokeCollection strokesToSelect = new StrokeCollection();

            bool isAlreadySelected;
            foreach (CustomStroke newStroke in GetSelectedStrokes())
            {
                isAlreadySelected = false;
                foreach (CustomStroke oldStroke in oldSelectedStrokes)
                {
                    if (newStroke.guid.Equals(oldStroke.guid))
                    {
                        isAlreadySelected = true;
                        oldSelectedStrokes.Remove(oldStroke);
                        break;
                    }
                }
                if (!isAlreadySelected)
                {
                    if (!SelectedStrokes.Contains(newStroke))
                    {
                        SelectedStrokes.Add(newStroke);
                    }
                    strokesToSelect.Add(newStroke);
                }
            }
            if (strokesToSelect.Count > 0)
                DrawingService.SelectShapes(strokesToSelect);
            if (oldSelectedStrokes.Count > 0)
                DrawingService.DeselectShapes(oldSelectedStrokes);
            RefreshChildren();
            oldSelectedStrokes = GetSelectedStrokes().Clone();
        }

        protected override void OnSelectionMoving(InkCanvasSelectionEditingEventArgs e)
        {
            foreach (CustomStroke stroke in SelectedStrokes)
            {
                Vector delta = new Vector(e.NewRectangle.X - e.OldRectangle.X, e.NewRectangle.Y - e.OldRectangle.Y);
                double rotationInRad = (stroke as ShapeStroke).shapeStyle.rotation * Math.PI / 180;

                double m11 = Math.Cos(-rotationInRad);
                double m12 = -Math.Sin(-rotationInRad);
                double m21 = Math.Sin(-rotationInRad);
                double m22 = Math.Cos(-rotationInRad);

                Matrix rotationMatrix = new Matrix(m11, m12, m21, m22, 0, 0);

                delta = rotationMatrix.Transform(delta);

                (stroke as ShapeStroke).shapeStyle.coordinates.x += delta.X;
                (stroke as ShapeStroke).shapeStyle.coordinates.y += delta.Y;
                
                if (!stroke.isLinkStroke())
                {
                    stroke.updateLinks();
                }
            }
            base.OnSelectionMoving(e);
        }
        
        protected override void OnSelectionMoved(EventArgs e)
        {
            DrawingService.UpdateShapes(SelectedStrokes);
            DrawingService.UpdateLinks(SelectedStrokes);
            RefreshLinks();
            RefreshChildren();
        }

        protected override void OnSelectionResized(EventArgs e)
        {
            RefreshLinks();
            RefreshChildren();
            DrawingService.UpdateShapes(GetSelectedStrokes());
        }

        protected override void OnSelectionResizing(InkCanvasSelectionEditingEventArgs e)
        {
            // Update selected strokes height and width
            StrokeCollection strokes = GetSelectedStrokes();

            double heightRatio = e.NewRectangle.Height / e.OldRectangle.Height;
            double widthRatio = e.NewRectangle.Width / e.OldRectangle.Width;

            foreach(CustomStroke stroke in strokes)
            {
                if (!stroke.isLinkStroke())
                {
                    (stroke as ShapeStroke).shapeStyle.width *= widthRatio;
                    (stroke as ShapeStroke).shapeStyle.height *= heightRatio;

                    RotateTransform oldRotation = new RotateTransform((stroke as ShapeStroke).shapeStyle.rotation,
                                                             (stroke as ShapeStroke).GetCenter().X,
                                                             (stroke as ShapeStroke).GetCenter().Y);

                    RotateTransform newRotation = new RotateTransform((stroke as ShapeStroke).shapeStyle.rotation,
                                                             e.NewRectangle.Left + e.NewRectangle.Width / 2,
                                                             e.NewRectangle.Top + e.NewRectangle.Height / 2);

                    Point oldCorner = oldRotation.Inverse.Transform(e.OldRectangle.TopLeft);
                    Point newCorner = newRotation.Inverse.Transform(e.NewRectangle.TopLeft);
                    double deltaX = newCorner.X - oldCorner.X;
                    double deltaY = newCorner.Y - oldCorner.Y;

                    (stroke as ShapeStroke).shapeStyle.coordinates.x += deltaX;
                    (stroke as ShapeStroke).shapeStyle.coordinates.y += deltaX;



                    /*Point newRectCenter = new Point(e.NewRectangle.Left + e.NewRectangle.Width / 2,
                                                    e.NewRectangle.Top + e.NewRectangle.Height / 2);

                    Point oldRectCenter = new Point(e.OldRectangle.Left + e.OldRectangle.Width / 2,
                                                    e.OldRectangle.Top + e.OldRectangle.Height / 2);

                    Vector delta = new Vector(newRectCenter.X  - oldRectCenter.X, newRectCenter.Y - oldRectCenter.Y);
                    double rotationInRad = (stroke as ShapeStroke).shapeStyle.rotation * Math.PI / 180;

                    double m11 = Math.Cos(-rotationInRad);
                    double m12 = -Math.Sin(-rotationInRad);
                    double m21 = Math.Sin(-rotationInRad);
                    double m22 = Math.Cos(-rotationInRad);

                    Matrix rotationMatrix = new Matrix(m11, m12, m21, m22, 0, 0);

                    delta = rotationMatrix.Transform(delta);

                    (stroke as ShapeStroke).shapeStyle.coordinates.x += delta.X;
                    (stroke as ShapeStroke).shapeStyle.coordinates.y += delta.Y;*/
                }
            }
        }

        protected override void OnStrokeErasing(InkCanvasStrokeErasingEventArgs e)
        {
            StrokeCollection strokesToDelete = new StrokeCollection();
            strokesToDelete.Add(e.Stroke);

            UpdateAnchorPointsAndLinks(strokesToDelete);

            DrawingService.RemoveShapes(strokesToDelete);
            base.OnStrokeErasing(e);
        }

        // Remove deleted shapeStrokes' guids from anchor points (to and from) in LinkStrokes
        // And remove deleted linkStokes from linksTo and linksFrom in ShapeStrokes
        private void UpdateAnchorPointsAndLinks(StrokeCollection strokesToDelete)
        {
            foreach (CustomStroke strokeToDelete in strokesToDelete)
            {
                // Remove deleted linkStokes from linksTo and linksFrom in ShapeStrokes
                if (strokeToDelete.isLinkStroke())
                {
                    LinkStroke linkStroke = strokeToDelete as LinkStroke;
                    RemoveShapeStrokeLinkFrom(linkStroke);
                    RemoveShapeStrokeLinkTo(linkStroke);
                }
                else
                {
                    // Remove deleted shapeStrokes' guids from anchor points (to and from) in LinkStrokes
                    ShapeStroke shapeStroke = strokeToDelete as ShapeStroke;

                    foreach (string linkStrokeGuid in shapeStroke.linksFrom)
                    {
                        CustomStroke linkStroke;
                        if (StrokesDictionary.TryGetValue(linkStrokeGuid, out linkStroke))
                        {
                            (linkStroke as LinkStroke).from = new AnchorPoint();
                            (linkStroke as LinkStroke).from.SetDefaults();
                        }
                    }
                    foreach (string linkStrokeGuid in shapeStroke.linksTo)
                    {
                        CustomStroke linkStroke;
                        if (StrokesDictionary.TryGetValue(linkStrokeGuid, out linkStroke))
                        {
                            (linkStroke as LinkStroke).to = new AnchorPoint();
                            (linkStroke as LinkStroke).to.SetDefaults();
                        }
                    }
                }
            }
        }

        protected override void OnStrokeErased(RoutedEventArgs e)
        {
            base.OnStrokeErased(e);
        }
        #endregion

        #region OnRemote...
        private void OnRemoteStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            CustomStroke stroke = (CustomStroke)e.Stroke;
            AddStroke(stroke);
            AddTextBox(stroke);
        }

        private void OnRemoteSelection(StrokeCollection strokes)
        {
            foreach (CustomStroke stroke in strokes)
            {
                remoteSelectionIds.Add(stroke.guid.ToString());
            }
            RefreshChildren();
        }

        private void OnRemoteDeselection(StrokeCollection strokes)
        {
            foreach (CustomStroke stroke in strokes)
            {
                remoteSelectionIds.RemoveAt(remoteSelectionIds.IndexOf(stroke.guid.ToString()));
            }
            RefreshChildren();
        }

        private void OnRemoveStrokes(StrokeCollection strokes)
        {
            foreach (CustomStroke stroke in strokes)
            {
                foreach (CustomStroke stroke2 in Strokes)
                {
                    if(stroke.guid.Equals(stroke2.guid))
                    {
                        RemoveStroke(stroke2);
                        break;
                    }
                }
            }
            RefreshChildren();
        }
        #endregion

        private void OnUpdateStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            CustomStroke stroke = (CustomStroke)e.Stroke;
            foreach (CustomStroke stroke2 in Strokes)
            {
                if (stroke.guid.Equals(stroke2.guid))
                {
                    int index = Strokes.IndexOf(stroke2);
                    Strokes.RemoveAt(index);
                    Strokes.Insert(index, stroke);
                    break;
                }
            }
            RefreshChildren();
        }

        #region OnStrokeCollected
        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            // Remove the original stroke and add a custom stroke.
            Strokes.Remove(e.Stroke);

            StrokeTypes strokeType = (StrokeTypes)Enum.Parse(typeof(StrokeTypes), StrokeType);

            CustomStroke customStroke = CreateStroke(e.Stroke.StylusPoints, e, strokeType);

            AddStroke(customStroke);
            if (!customStroke.isLinkStroke())
            {
                DrawingService.CreateShape(customStroke as ShapeStroke);
            }
            else
            {
                DrawingService.CreateLink(customStroke as LinkStroke);
            }
            // firstPoint = customStroke.StylusPoints[0];
            SelectedStrokes = new StrokeCollection { Strokes[Strokes.Count - 1] };

            //drawingService.UpdateShape("id", 0, "strokeName", shapeStyle, new List<string>(), new List<string>());

            // Pass the custom stroke to base class' OnStrokeCollected method.
            InkCanvasStrokeCollectedEventArgs args = new InkCanvasStrokeCollectedEventArgs(customStroke);
            base.OnStrokeCollected(args);

            AddTextBox(customStroke);
        }


        private CustomStroke CreateStroke(StylusPointCollection pts, InkCanvasStrokeCollectedEventArgs e, StrokeTypes strokeType)
        {
            CustomStroke customStroke;
            switch (strokeType)
            {
                case StrokeTypes.CLASS_SHAPE:
                    customStroke = new ClassStroke(pts);
                    break;
                case StrokeTypes.ARTIFACT:
                    customStroke = new ArtifactStroke(pts);
                    break;
                case StrokeTypes.ACTIVITY:
                    customStroke = new ActivityStroke(pts);
                    break;
                case StrokeTypes.ROLE:
                    customStroke = new ActorStroke(pts);
                    break;
                case StrokeTypes.COMMENT:
                    customStroke = new CommentStroke(pts);
                    break;
                case StrokeTypes.PHASE:
                    customStroke = new PhaseStroke(pts);
                    break;
                case StrokeTypes.LINK:
                    customStroke = new LinkStroke(e.Stroke.StylusPoints);
                    break;
                default:
                    customStroke = new ClassStroke(pts);
                    break;

            }
            return customStroke;
        }

        private void AddTextBox(CustomStroke stroke)
        {
            Path path = new Path();
            switch (stroke.strokeType)
            {
                case (int)StrokeTypes.LINK:
                    double x = stroke.StylusPoints[0].X;
                    double y = stroke.StylusPoints[0].Y;
                    CreateNameTextBox(stroke, x, y);
                    CreateMultiplicityTextBox(stroke.StylusPoints, stroke as LinkStroke);
                    break;
                case (int)StrokeTypes.CLASS_SHAPE:
                    path.Data = stroke.GetGeometry();
                    Children.Add(path);
                    AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
                    myAdornerLayer.Add(new ClassAdorner(path, stroke, this));
                    break;
                case (int)StrokeTypes.COMMENT:
                    path.Data = stroke.GetGeometry();
                    Children.Add(path);
                    AdornerLayer commentAdorner = AdornerLayer.GetAdornerLayer(path);
                    commentAdorner.Add(new CommentAdorner(path, stroke, this));
                    break;
                case (int)StrokeTypes.PHASE:
                    path.Data = stroke.GetGeometry();
                    Children.Add(path);
                    AdornerLayer phaseAdorner = AdornerLayer.GetAdornerLayer(path);
                    phaseAdorner.Add(new PhaseAdorner(path, stroke, this));
                    break;
                default:
                    path.Data = stroke.GetGeometry();
                    Children.Add(path);
                    AdornerLayer defaultAdorner = AdornerLayer.GetAdornerLayer(path);
                    defaultAdorner.Add(new ShapeNameAdorner(path, stroke, this));
                    break;
            }
        }

        private void CreateMultiplicityTextBox(StylusPointCollection stylusPoints, LinkStroke stroke)
        {
            double fromX = stylusPoints[0].X;
            double fromY = stylusPoints[0].Y - 20;

            double toX = stylusPoints[stylusPoints.Count - 1].X;
            double toY = stylusPoints[stylusPoints.Count - 1].Y - 20;

            CustomTextBox from = new CustomTextBox();
            from.Text = "" + stroke.from.multiplicity;
            from.Uid = stroke.guid.ToString();
            Children.Add(from);
            SetTop(from, fromY);
            SetLeft(from, fromX);

            CustomTextBox to = new CustomTextBox();
            to.Text = "" + stroke.to.multiplicity;
            to.Uid = stroke.guid.ToString();
            Children.Add(to);
            SetTop(to, toY);
            SetLeft(to, toX);
        }

        private void CreateNameTextBox(CustomStroke stroke, double x, double y)
        {
            CustomTextBox tb = new CustomTextBox();
            tb.Text = stroke.name;
            tb.Uid = stroke.guid.ToString();

            Children.Add(tb);
            SetTop(tb, y);
            SetLeft(tb, x);
        }
        #endregion

        #region RotateStrokesWithAngle
        public void RotateStrokesWithAngle(double rotation)
        {
            StrokeCollection strokes = GetSelectedStrokes();
            StrokeCollection selectedNewStrokes = new StrokeCollection();

            foreach (CustomStroke selectedStroke in strokes)
            {
                Stroke newStroke = selectedStroke.CloneRotated(rotation);
                StrokeCollection newStrokes = new StrokeCollection { newStroke };
                ReplaceStrokes(selectedStroke, newStrokes);

                selectedNewStrokes.Add(newStrokes);
            }

            //SelectedStrokes.Add(newStrokes); // non necessaire, pcq le .Select les ajoute 
            Select(selectedNewStrokes);
            DrawingService.UpdateShapes(selectedNewStrokes);
            RefreshLinks();
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

                if((newStroke as CustomStroke).isLinkStroke())
                {
                    LinkStroke linkStroke = newStroke as LinkStroke;
                    linkStroke.from = new AnchorPoint();
                    linkStroke.from.SetDefaults();
                    linkStroke.to = new AnchorPoint();
                    linkStroke.to.SetDefaults();
                } else
                {
                    (newStroke as ShapeStroke).linksTo.Clear();
                    (newStroke as ShapeStroke).linksFrom.Clear();
                }

                // TODO : 2 options. 1- Avoir un compteur de Paste qui incremente a chaque Paste, le reinitialiser quand 
                // nouveau OnSelectionChanged. 2- Coller au coin du canvas
                // Voir quoi faire avec le client leger
                Matrix translateMatrix = new Matrix();
                translateMatrix.Translate(20.0, 20.0);
                newStroke.Transform(translateMatrix, false);

                AddStroke(newStroke as CustomStroke);
                newStrokes.Add(newStroke);
            }

            Select(newStrokes);
        }
        #endregion

        #region CutStrokes
        public void CutStrokes()
        {
            StrokeCollection selectedStrokes = GetSelectedStrokes();
            UpdateAnchorPointsAndLinks(selectedStrokes);

            foreach(CustomStroke stroke in selectedStrokes)
            {
                RemoveStroke(stroke);
            }

            // put selection in clipboard to be able to paste it
            clipboard = selectedStrokes;

            // cut selection from canvas
            CutSelection();

            // To delete the adorners
            RefreshChildren();

            // Send message to server that the stroke is deleted
            DrawingService.RemoveShapes(selectedStrokes);
        }
        #endregion


        internal void DeleteStrokes(StrokeCollection selectedStrokes)
        {
            UpdateAnchorPointsAndLinks(selectedStrokes);
            SelectedStrokes.Clear();
            RefreshChildren();
            DrawingService.RemoveShapes(selectedStrokes);
        }

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

            foreach (string strokeId in remoteSelectionIds)
            {
                foreach (CustomStroke stroke in Strokes)
                {
                    if(stroke.guid.ToString().Equals(strokeId))
                    {
                        AddRemoteSelectionAdorner(stroke);
                        break;
                    }
                }
            }
            // Add text boxes (names) to all strokes. And add dotted path if linkStroke is dotted
            foreach (CustomStroke stroke in Strokes)
            {
                AddTextBox(stroke);
                if (stroke.isLinkStroke() && (stroke as LinkStroke).style.type == 1) // dotted linkStroke
                {
                    Path path = new Path();
                    path.Data = stroke.GetGeometry();

                    Children.Add(path);
                    AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
                    myAdornerLayer.Add(new DottedPathAdorner(path, stroke as LinkStroke, this));
                }
            }
            // Select(selectedStrokes);
        }

        private void addAdorners(CustomStroke selectedStroke)
        {
            Path path = new Path();
            path.Data = selectedStroke.GetGeometry();

            Children.Add(path);
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
            myAdornerLayer.Add(new EditionAdorner(path, selectedStroke, this));

            if (!selectedStroke.isLinkStroke())
            {
                if (GetSelectedStrokes().Count == 1)
                {
                    myAdornerLayer.Add(new RotateAdorner(path, selectedStroke, this));
                }
                myAdornerLayer.Add(new AnchorPointAdorner(path, selectedStroke, this));
                if (selectedStroke.strokeType == (int)StrokeTypes.CLASS_SHAPE)
                {
                    myAdornerLayer.Add(new ClassAdorner(path, selectedStroke, this));
                }
            } else
            {
                if(!(selectedStroke as LinkStroke).isAttached() && GetSelectedStrokes().Count == 1)
                {
                    myAdornerLayer.Add(new LinkRotateAdorner(path, selectedStroke as LinkStroke, this));
                }
                myAdornerLayer.Add(new LinkAnchorPointAdorner(path, selectedStroke as LinkStroke, this));
            }
      
        }

        private void AddRemoteSelectionAdorner(CustomStroke stroke)
        {
            Path path = new Path();
            path.Data = stroke.GetGeometry();

            Children.Add(path);
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
            myAdornerLayer.Add(new RemoteSelectionAddorner(path, stroke, this));
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

        
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            SelectedStrokes = GetSelectedStrokes();
            if (EditingMode == InkCanvasEditingMode.Select)
            {
                selectionPath.StartPoint = e.GetPosition(this);
                selectionPath.Segments = new PathSegmentCollection();
                selectionPath.IsClosed = true;
                beingSelected.Clear();
                foreach (CustomStroke stroke in SelectedStrokes)
                {
                    if (stroke.HitTestPointIncludingEdition(e.GetPosition(this)))
                    {
                        base.OnPreviewMouseDown(e);
                        return;
                    }
                }
                selectionPath.Segments.Add(new LineSegment(e.GetPosition(this), true));
                foreach (CustomStroke stroke in Strokes)
                {
                    if (stroke is ShapeStroke && stroke.HitTestPoint(e.GetPosition(this)))
                    {
                        if (beingSelected.Any())
                            beingSelected.Clear();
                        beingSelected.Add(stroke);
                    }
                }
            }
            else if (EditingMode == InkCanvasEditingMode.EraseByStroke)
            {
                beingSelected = new StrokeCollection();
                foreach (CustomStroke stroke in Strokes)
                {
                    if (stroke is ShapeStroke && stroke.HitTestPoint(e.GetPosition(this)))
                    {
                        beingSelected.Add(stroke);
                    }
                }
                if (beingSelected.Any())
                {
                    Strokes.Remove(beingSelected);
                    DrawingService.RemoveShapes(beingSelected);
                    RefreshChildren();
                    beingSelected.Clear();
                }
            } else
            {
                base.OnPreviewMouseDown(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (EditingMode == InkCanvasEditingMode.Select)
            {
                if (selectionPath.Segments.Any())
                    selectionPath.Segments.Add(new LineSegment(e.GetPosition(this), true));
            }
            else if (EditingMode == InkCanvasEditingMode.EraseByStroke)
            {
                beingSelected = new StrokeCollection();
                foreach (CustomStroke stroke in Strokes)
                {
                    if (stroke is ShapeStroke && stroke.HitTestPoint(e.GetPosition(this)))
                    {
                        beingSelected.Add(stroke);
                    }
                }
                if (beingSelected.Any())
                {
                    Strokes.Remove(beingSelected);
                    DrawingService.RemoveShapes(beingSelected);
                    RefreshChildren();
                    beingSelected.Clear();
                }
            }
            else
            {
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (!lastSize.Equals(RenderSize))
            {
                lastSize = RenderSize;
                DrawingService.ResizeCanvas(new Coordinates(Width, Height));
            }
            if (EditingMode == InkCanvasEditingMode.Select)
            {
                if (selectionPath.Segments.Any())
                {
                    if (selectionPath.Segments.Count < 5)
                    {
                        SelectedStrokes.Clear();
                        Select(beingSelected);
                        selectionPath.Segments.Clear();
                    } else
                    {
                        beingSelected.Clear();
                        SelectedStrokes.Clear();
                        PathFigureCollection figures = new PathFigureCollection { selectionPath };
                        PathGeometry geometry = new PathGeometry(figures);
                        foreach (CustomStroke stroke in Strokes)
                        {
                            RectangleGeometry strokeBound = new RectangleGeometry(stroke.GetCustomBound());
                            if (geometry.FillContains(strokeBound))
                            {
                                beingSelected.Add(stroke);
                            }
                        }
                        Select(beingSelected);
                        selectionPath.Segments.Clear();
                    }
                }
            }
            else
            {
                base.OnMouseUp(e);
            }
        }

        public void RefreshSelectedShape(ShapeStroke stroke)
        {
            ShapeStroke strokeCopy = (ShapeStroke)stroke.Clone();
            StrokeCollection strokes = new StrokeCollection() { strokeCopy };
            Strokes.Replace(stroke, strokes);
            Select(strokes);
        }
    }
}
