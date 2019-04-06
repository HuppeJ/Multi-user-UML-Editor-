using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using PolyPaint.VueModeles;
using PolyPaint;
using System.Windows.Documents;
using PolyPaint.CustomInk;
using System.Windows.Ink;
using PolyPaint.Enums;
using System.Windows.Shapes;
using System.Collections.Generic;
using PolyPaint.CustomInk.Strokes;
using PolyPaint.Templates;
using System.Windows.Threading;
using PolyPaint.Services;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Logique d'interaction pour WindowDrawing.xaml
    /// </summary>
    public partial class WindowDrawing : UserControl
    {
        double width = 32;
        double height = 32;

        public WindowDrawing()
        {
            InitializeComponent();
            DataContext = new VueModele();
            this.Loaded += WindowDrawing_Loaded;
            DrawingService.OnResizeCanvas += OnResizeCanvas;
        }

        void WindowDrawing_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += window_Closing;
        }

        void window_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            DrawingService.LeaveCanvas();
        }

        // Pour gérer les points de contrôles.
        private void GlisserCommence(object sender, DragStartedEventArgs e) => (sender as Thumb).Background = Brushes.Black;
        private void GlisserTermine(object sender, DragCompletedEventArgs e)
        {
            (sender as Thumb).Background = Brushes.White;

            DrawingService.ResizeCanvas(new Coordinates(width * 2.1, height * 2.1));
        }
        private void GlisserMouvementRecu(object sender, DragDeltaEventArgs e)
        {
            String nom = (sender as Thumb).Name;
            if (nom == "horizontal" || nom == "diagonal")
            {
                width = Math.Min(Math.Max(225, colonne.Width.Value + e.HorizontalChange), 800);
                colonne.Width = new GridLength(width);
            }
            if (nom == "vertical" || nom == "diagonal")
            {
                height = Math.Min(Math.Max(225, ligne.Height.Value + e.VerticalChange), 550);
                ligne.Height = new GridLength(height);
            }
        }

        private void OnResizeCanvas(Coordinates dimensions)
        {
            colonne.Width = new GridLength(dimensions.x / 2.1);
            ligne.Height = new GridLength(dimensions.y / 2.1);
        }

        // Pour la gestion de l'affichage de position du pointeur.
        private void surfaceDessin_MouseLeave(object sender, MouseEventArgs e) => textBlockPosition.Text = "";
        private void surfaceDessin_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(surfaceDessin);
            textBlockPosition.Text = Math.Round(p.X) + ", " + Math.Round(p.Y) + "px";
        }

        public void AlignLeft() => surfaceDessin.AlignLeft();

        public void AlignCenter() => surfaceDessin.AlignCenter();

        private void DupliquerSelection(object sender, RoutedEventArgs e) => surfaceDessin.PasteStrokes();

        private void SupprimerSelection(object sender, RoutedEventArgs e) => surfaceDessin.CutStrokes();

        private void RefreshChildren(object sender, RoutedEventArgs e)
        {

            // pcq click et command ne fonctionnent pas ensemble
            var btn = sender as Button;
            btn.Command.Execute(btn.CommandParameter);

            surfaceDessin.RefreshChildren();
        }

        private void Empiler(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.Command.Execute(btn.CommandParameter);

            surfaceDessin.RefreshChildren();
        }

        // Quand une nouvelle nouvelle stroke a ete ajoute
        private void surfaceDessin_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((DataContext as VueModele)?.OutilSelectionne == "crayon" && surfaceDessin.SelectedStrokes.Count > 0)
            {
                CustomStroke newStroke = (DataContext as VueModele).AddStrokeFromView(
                    (CustomStroke)surfaceDessin.SelectedStrokes[0]
                );
                surfaceDessin.Select(new StrokeCollection { newStroke });
                if (newStroke.GetType() != typeof(LinkStroke))
                {
                    Path path = new Path();
                    path.Data = new RectangleGeometry(newStroke.GetBounds());
                    surfaceDessin.Children.Add(path);
                    AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
                    myAdornerLayer.Add(new RotateAdorner(path, newStroke, surfaceDessin));
                    myAdornerLayer.Add(new AnchorPointAdorner(path, newStroke, surfaceDessin));
                    Adorner[] ad = myAdornerLayer.GetAdorners(path);
                    myAdornerLayer.Add(new EditionAdorner(path, newStroke, surfaceDessin));
                }
            }
            else
            {
                foreach (CustomStroke stroke in surfaceDessin.GetSelectedStrokes())
                {
                    if (stroke is LinkStroke)
                    {
                        surfaceDessin.modifyLinkStrokePath(stroke as LinkStroke, e.GetPosition(surfaceDessin));
                    }
                }
            }


            // Pour que les boutons soient de la bonne couleur
            (DataContext as VueModele)?.ChoisirOutil.Execute("lasso");
        }

        // Bouton pour changer le texte de l'élément sélectionné
        public void RenameSelection()
        {
            StrokeCollection strokes = surfaceDessin.GetSelectedStrokes();
            if(strokes.Count == 1)
            {
                if ((strokes[0] as CustomStroke).strokeType == (int)StrokeTypes.CLASS_SHAPE)
                {
                    popUpClassVue.setParameters(strokes[0] as ClassStroke);
                    popUpClass.IsOpen = true;
                }
                else if((strokes[0] as CustomStroke).strokeType == (int)StrokeTypes.LINK)
                {
                    popUpLinkVue.setParameters();
                    popUpLink.IsOpen = true;
                }
                else if (strokes[0] is CommentStroke)
                {
                    popUpCommentVue.setParameters(strokes[0] as CustomStroke);
                    popUpComment.IsOpen = true;
                }
                else if (strokes[0] is FloatingTextStroke)
                {
                    popUpFloatingTextVue.setParameters(strokes[0] as CustomStroke);
                    popUpFloatingText.IsOpen = true;
                }
                else
                {
                    popUpNameVue.setParameters(strokes[0] as CustomStroke);
                    popUpName.IsOpen = true;
                }
                IsEnabled = false;
            }
        }

        public void DeleteSelection()
        {
            StrokeCollection strokes = surfaceDessin.GetSelectedStrokes();
            surfaceDessin.DeleteStrokes(strokes);
        }

        public void Rename(string text, Color borderColor, Color fillColor, int lineStyle )
        {
            popUpName.IsOpen = false;
            ShapeStroke stroke = (ShapeStroke)surfaceDessin.GetSelectedStrokes()[0];
            stroke.name = text;
            stroke.shapeStyle.borderColor = borderColor.ToString();
            stroke.shapeStyle.backgroundColor = fillColor.ToString();
            stroke.shapeStyle.borderStyle = lineStyle;
            StrokeCollection sc = new StrokeCollection();
            sc.Add(stroke);
            DrawingService.UpdateShapes(sc);
            surfaceDessin.RefreshChildren();
            surfaceDessin.RefreshSelectedShape(stroke);
            IsEnabled = true;
        }

        public void CommentEdition(string text, Color borderColor, Color fillColor, int lineStyle)
        {
            popUpComment.IsOpen = false;
            ShapeStroke stroke = (ShapeStroke)surfaceDessin.GetSelectedStrokes()[0];
            stroke.name = text;
            stroke.shapeStyle.borderColor = borderColor.ToString();
            stroke.shapeStyle.backgroundColor = fillColor.ToString();
            stroke.shapeStyle.borderStyle = lineStyle;
            StrokeCollection sc = new StrokeCollection();
            sc.Add(stroke);
            DrawingService.UpdateShapes(sc);
            surfaceDessin.RefreshChildren();
            surfaceDessin.RefreshSelectedShape(stroke);
            IsEnabled = true;
        }


        public void FloatingTextEdition(string text, Color borderColor, Color fillColor, int lineStyle)
        {
            popUpFloatingText.IsOpen = false;
            ShapeStroke stroke = (ShapeStroke)surfaceDessin.GetSelectedStrokes()[0];
            stroke.name = text;
            stroke.shapeStyle.borderColor = borderColor.ToString();
            stroke.shapeStyle.backgroundColor = fillColor.ToString();
            stroke.shapeStyle.borderStyle = lineStyle;
            StrokeCollection sc = new StrokeCollection();
            sc.Add(stroke);
            DrawingService.UpdateShapes(sc);
            surfaceDessin.RefreshChildren();
            surfaceDessin.RefreshSelectedShape(stroke);
            IsEnabled = true;
        }

        public void Rename(string className, string attributes, string methods, Color borderColor, Color fillColor, int lineStyle)
        {
            popUpClass.IsOpen = false;
            ClassStroke stroke = (ClassStroke)surfaceDessin.GetSelectedStrokes()[0];
            stroke.name = className;
            stroke.shapeStyle.borderColor = borderColor.ToString();
            stroke.shapeStyle.backgroundColor = fillColor.ToString();
            stroke.shapeStyle.borderStyle = lineStyle;

            stroke.attributes = new List<string>();
            string[] lines = attributes.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );
            foreach(string line in lines)
            {
                stroke.attributes.Add(line);
            }

            stroke.methods = new List<string>();
            lines = methods.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );
            foreach (string line in lines)
            {
                stroke.methods.Add(line);
            }

            StrokeCollection sc = new StrokeCollection();
            sc.Add(stroke);
            DrawingService.UpdateShapes(sc);

            surfaceDessin.RefreshChildren();
            surfaceDessin.RefreshSelectedShape(stroke);
            IsEnabled = true;
        }

        public void EditLink(string linkName, int linkType, int linkStyle, string selectedColor, int linkThickness, string multiplicityFrom, string multiplicityTo)
        {
            popUpLink.IsOpen = false;
            LinkStroke stroke = (LinkStroke)surfaceDessin.GetSelectedStrokes()[0];
            stroke.name = linkName;
            stroke.style.type = linkStyle;
            stroke.style.color = selectedColor;
            stroke.style.thickness = linkThickness;
            stroke.from.multiplicity = multiplicityFrom;
            stroke.to.multiplicity = multiplicityTo;

            stroke.linkType = linkType;
            stroke.addStylusPointsToLink();
            // dotted
            if(stroke.style.type == 1){
                stroke.DrawingAttributes.Color = Colors.Transparent;
            }
            else // normal line
            {
                stroke.DrawingAttributes.Color = (Color) ColorConverter.ConvertFromString(selectedColor);
            }

            switch (linkThickness)
            {
                case 0:
                    stroke.DrawingAttributes.Width = 4;
                    stroke.DrawingAttributes.Height = 4;
                    break;
                case 1:
                    stroke.DrawingAttributes.Width = 6;
                    stroke.DrawingAttributes.Height = 6;
                    break;
                case 2:
                    stroke.DrawingAttributes.Width = 8;
                    stroke.DrawingAttributes.Height = 8;
                    break;
                default:
                    stroke.DrawingAttributes.Width = 4;
                    stroke.DrawingAttributes.Height = 4;
                    break;
            }

            StrokeCollection sc = new StrokeCollection();
            sc.Add(stroke);
            DrawingService.UpdateLinks(sc);

            surfaceDessin.RefreshChildren();
            IsEnabled = true;
        }

        private void surfaceDessin_SelectionChanged(object sender, EventArgs e)
        {
            // TODO ajouter les appels au service pour "locker" l'objet dans le collaboratif 
            // (avec surfaceDessin.SelectedStrokes)

        }

        private void StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            (DataContext as VueModele)?.OnStrokeCollectedEvent(sender, e);
        }

        private void OpenClassPopup(object sender, EventArgs e)
        {
            IsEnabled = false;
            popUpClassFromCodeVue.Initialize();
            popUpClassFromCodeVue.CodeTextBox.Text = "";
            popUpClassFromCode.IsOpen = true;
        }

        public void ClosePopups()
        {
            IsEnabled = true;
            popUpClassFromCode.IsOpen = false;
        }

        internal void SelectCSFile()
        {
            popUpClassFromCode.IsOpen = false;
            string selectedFile = "";

            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.DefaultExt = "cs";
            openFileDialog.Filter = "cs files (*.cs)|*.cs";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                popUpClassFromCodeVue.CodeTextBox.Text = "";
                selectedFile = openFileDialog.FileName;

                System.IO.StreamReader reader = System.IO.File.OpenText(selectedFile);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    popUpClassFromCodeVue.CodeTextBox.Text += line + Environment.NewLine;
                }
            }

            popUpClassFromCode.IsOpen = true;
        }

        public void DrawClass(string name, List<string> properties, List<string> methods)
        {
            StylusPointCollection stylusPoints = new StylusPointCollection();
            stylusPoints.Add(new StylusPoint(10, 10));
            ClassStroke classStroke = new ClassStroke(stylusPoints);

            classStroke.name = name;
            classStroke.attributes = properties;
            classStroke.methods = methods;

            InkCanvasStrokeCollectedEventArgs eventArgs = new InkCanvasStrokeCollectedEventArgs(classStroke);
            DrawingService.AddClassFromCode(eventArgs);
        }

        private void AdjustToLargestClassWidth(object sender, EventArgs e)
        {
            StrokeCollection sc = surfaceDessin.Strokes;
            StrokeCollection selectedStrokes = surfaceDessin.SelectedStrokes;
            StrokeCollection selectedsc = new StrokeCollection();
            StrokeCollection updatedsc = new StrokeCollection();

            double maxWidth = 0;

            foreach(ShapeStroke stroke in sc)
            {
                if(stroke.strokeType == 0)
                {
                    if(stroke.shapeStyle.width > maxWidth)
                    {
                        maxWidth = stroke.shapeStyle.width;
                    }
                }
            }

            for(int i=0; i<sc.Count; i++)
            {
                if ((sc[i] as ShapeStroke).strokeType == 0)
                {
                    (sc[i] as ShapeStroke).shapeStyle.width = maxWidth;
                    var updatedStroke = sc[i];
                    surfaceDessin.Strokes.RemoveAt(i);
                    surfaceDessin.Strokes.Insert(i, updatedStroke);
                    if (selectedStrokes.Contains(sc[i]))
                    {
                        selectedsc.Add(sc[i]);
                    }
                    // Send modifiation to server
                    updatedsc.Add(sc[i]);
                }
            }

            surfaceDessin.Select(selectedsc);
            DrawingService.UpdateShapes(updatedsc);
        }

        private void ExportCanvas(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image  
            // assigned to Button2.  
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "PNG Image|*.png";
            saveFileDialog1.Title = "Save your canvas";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  
                System.IO.FileStream fs =
                   (System.IO.FileStream)saveFileDialog1.OpenFile();

                surfaceDessin.GetCanvas().Save(fs,
                           System.Drawing.Imaging.ImageFormat.Png);

                fs.Close();
            }
        }

        private void OpenTutorial(object sender, RoutedEventArgs e)
        {

            DrawingService.OpenTutorial();
        }

        private void BackToGallery(object sender, RoutedEventArgs e)
        {
            DrawingService.GoToGallery();
        }

        private void ViewHistoryPopup(object sender, RoutedEventArgs e)
        {
            popUpHistoryVue.Initialize();
            popUpHistory.IsOpen = true;
            IsEnabled = false;
        }

        internal void ClosePopup()
        {
            popUpHistory.IsOpen = false;
            IsEnabled = true;
        }
    }
}
