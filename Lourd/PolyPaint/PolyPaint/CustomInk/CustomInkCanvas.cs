using PolyPaint.Enums;
using PolyPaint.Services;
using PolyPaint.Templates;
using PolyPaint.Utilitaires;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public List<Link> links = new List<Link>();

        private CustomDynamicRenderer customRenderer = new CustomDynamicRenderer();

        private StrokeCollection clipboard;
        private Templates.Canvas canvas;

        public StylusPoint firstPoint;
        private int isCreatingLink = 0;

        #region createLink
        public void createLink(CustomStroke stroke, int anchor, Point pointPosition)
        {
            Select(new StrokeCollection { stroke });
            // cheat, pcq RefreshChildren est appele avant createConnectionForm
            if (isCreatingLink == 2)
            {
                Path path = new Path();
                path.Stroke = Brushes.Black;
                path.StrokeThickness = 1;
                Point firstPointToLink = links[links.Count - 1].GetFromPoint(this.Strokes);
                    //stroke.GetAnchorPoint(anchor);
                path.Data = new LineGeometry(firstPointToLink, pointPosition);


                links[links.Count - 1].path.Add(new Coordinates(pointPosition.X, pointPosition.Y));
                links[links.Count - 1].to = new AnchorPoint(stroke.guid.ToString(), anchor, "0");

                Children.Add(path);
                isCreatingLink = 0;
                refreshLinks();
            }
            else
            {
                Link newLink = new Link(pointPosition, stroke.guid.ToString(), anchor);
                links.Add(newLink);
                
                addAnchorPoints();
                isCreatingLink = 1;
            }

        }

        public void refreshLinks()
        {
            if (isCreatingLink != 2)
            {
                foreach (Link link in links)
                {
                    Point fromPoint = link.GetFromPoint(this.Strokes);
                    Point toPoint = link.GetToPoint(this.Strokes);
                    // mettre a jour les positions des points
                    link.path.Clear();
                    link.path.Add(new Coordinates(fromPoint.X, fromPoint.Y));
                    link.path.Add(new Coordinates(toPoint.X, toPoint.Y));

                    // redessiner les liens
                    Path path = new Path();
                    path.Stroke = Brushes.Black;
                    path.StrokeThickness = 1;
                    path.Data = new LineGeometry(fromPoint, toPoint);

                    Children.Add(path);
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

            canvas = new Templates.Canvas(Guid.NewGuid().ToString(), "newCanvas", ConnectionService.username,
                ConnectionService.username, 1, null, new List<BasicShape>(), new List<Link>(), new int[] { 1, 1 });
            DrawingService.CreateCanvas(canvas);
            DrawingService.JoinCanvas("newCanvas");
            DrawingService.AddStroke += OnRemoteStroke;
        }

        #region On.. event handlers
        protected override void OnSelectionChanging(InkCanvasSelectionChangingEventArgs e) {
            SelectedStrokes.Clear();
            foreach (Stroke stroke in e.GetSelectedStrokes())
            {
                SelectedStrokes.Add(stroke);
            }
            base.OnSelectionChanging(e);
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
            base.OnSelectionMoved(e);
            RefreshChildren();
        }

        protected override void OnSelectionResizing(InkCanvasSelectionEditingEventArgs e)
        {
            base.OnSelectionResizing(e);

            // Update selected strokes height and width
            StrokeCollection strokes = GetSelectedStrokes();
            double heightRatio = e.NewRectangle.Height / e.OldRectangle.Height;
            double widthRatio = e.NewRectangle.Width / e.OldRectangle.Width;

            foreach(CustomStroke stroke in strokes)
            {
                stroke.shapeStyle.width *= widthRatio;
                stroke.shapeStyle.height *= heightRatio;
            }
        }

        protected override void OnSelectionResized(EventArgs e)
        {
            base.OnSelectionResized(e);
            RefreshChildren();
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

        private void OnRemoteStroke(InkCanvasStrokeCollectedEventArgs e)
        {
            CustomStroke stroke = (CustomStroke)e.Stroke;
            Strokes.Add(stroke);

            AddTextBox(stroke);
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
                    customStroke = new ClassStroke(pts);
                    break;
                case StrokeTypes.PHASE:
                    customStroke = new ClassStroke(pts);
                    break;
                default:
                    customStroke = new ClassStroke(pts);
                    break;

            }
            return customStroke;
        }

        #region OnStrokeCollected
        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            // Remove the original stroke and add a custom stroke.
            Strokes.Remove(e.Stroke);

            StrokeTypes strokeType = (StrokeTypes) Enum.Parse(typeof(StrokeTypes), StrokeType);

            CustomStroke customStroke = CreateStroke(e.Stroke.StylusPoints, e, strokeType);
            
            Strokes.Add(customStroke);
            DrawingService.CreateShape(customStroke);
            // firstPoint = customStroke.StylusPoints[0];
            SelectedStrokes = new StrokeCollection { Strokes[Strokes.Count - 1] };

            //drawingService.UpdateShape("id", 0, "strokeName", shapeStyle, new List<string>(), new List<string>());

            // Pass the custom stroke to base class' OnStrokeCollected method.
            InkCanvasStrokeCollectedEventArgs args = new InkCanvasStrokeCollectedEventArgs(customStroke);
            base.OnStrokeCollected(args);

            AddTextBox(customStroke);
        }

        private void AddTextBox(CustomStroke stroke)
        {
            if(stroke.type != (int) StrokeTypes.CLASS_SHAPE)
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

        //private void AddStroke(CustomStroke newStroke)
        //{
        //    Console.WriteLine("add de vueModele en provenance du service :) ");

        //    Strokes.Add(newStroke);
        //}

        //private void UpdateStroke(CustomStroke newStroke)
        //{
        //    Console.WriteLine("update de vueModele en provenance du service :) ");
        //    // ne add pas le trait pour vrai..
        //    Strokes.Add(newStroke);
        //}

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

            if (isCreatingLink == 1)
            {
                isCreatingLink++;
            } else
            {
                isCreatingLink = 0;
            }
            
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
            refreshLinks();

            ReadOnlyCollection<UIElement> ahhh = GetSelectedElements();
            Select(selectedStrokes, GetSelectedElements());
        }

        private void addAdorners(CustomStroke selectedStroke)
        {
            Path path = new Path();
            path.Data = selectedStroke.GetGeometry();

            Children.Add(path);
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
            myAdornerLayer.Add(new RotateAdorner(path, selectedStroke, this));
            myAdornerLayer.Add(new AnchorPointAdorner(path, selectedStroke, this));
            myAdornerLayer.Add(new EditionAdorner(path, selectedStroke, this));
            if(selectedStroke.type == (int)StrokeTypes.CLASS_SHAPE)
            {
                myAdornerLayer.Add(new ClassAdorner(path, selectedStroke, this));
            }
        }

        // Tjrs avoir les anchorPoints? Laid.. gi
        private void addAnchorPoints()
        {
            foreach (CustomStroke stroke in Strokes)
            {
                Path path = new Path();
                path.Data = stroke.GetGeometry();

                Children.Add(path);
                AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
                myAdornerLayer.Add(new AnchorPointAdorner(path, stroke, this));
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
    }
}
