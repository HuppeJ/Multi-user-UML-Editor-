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

        public WindowDrawing()
        {
            InitializeComponent();
            DataContext = new VueModele();
        }

        // Pour gérer les points de contrôles.
        private void GlisserCommence(object sender, DragStartedEventArgs e) => (sender as Thumb).Background = Brushes.Black;
        private void GlisserTermine(object sender, DragCompletedEventArgs e) => (sender as Thumb).Background = Brushes.White;
        private void GlisserMouvementRecu(object sender, DragDeltaEventArgs e)
        {
            String nom = (sender as Thumb).Name;
            if (nom == "horizontal" || nom == "diagonal") colonne.Width = new GridLength(Math.Max(32, colonne.Width.Value + e.HorizontalChange));
            if (nom == "vertical" || nom == "diagonal") ligne.Height = new GridLength(Math.Max(32, ligne.Height.Value + e.VerticalChange));
        }

        // Pour la gestion de l'affichage de position du pointeur.
        private void surfaceDessin_MouseLeave(object sender, MouseEventArgs e) => textBlockPosition.Text = "";
        private void surfaceDessin_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(surfaceDessin);
            textBlockPosition.Text = Math.Round(p.X) + ", " + Math.Round(p.Y) + "px";

        }

        private void DupliquerSelection(object sender, RoutedEventArgs e) => surfaceDessin.PasteStrokes();

        private void SupprimerSelection(object sender, RoutedEventArgs e) => surfaceDessin.CutStrokes();

        private void RefreshChildren(object sender, RoutedEventArgs e)
        {
            // pcq click et command ne fonctionnent pas ensemble
            var btn = sender as Button;
            btn.Command.Execute(btn.CommandParameter);

            surfaceDessin.RefreshChildren();
        }

        private void surfaceDessin_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (surfaceDessin.SelectedStrokes.Count > 0)
                {
                    surfaceDessin.DeleteStrokes(surfaceDessin.SelectedStrokes);
                }
            }
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
                    path.Data = newStroke.GetGeometry();
                    surfaceDessin.Children.Add(path);
                    AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
                    myAdornerLayer.Add(new RotateAdorner(path, newStroke, surfaceDessin));
                    myAdornerLayer.Add(new AnchorPointAdorner(path, newStroke, surfaceDessin));
                    Adorner[] ad = myAdornerLayer.GetAdorners(path);
                    myAdornerLayer.Add(new EditionAdorner(path, newStroke, surfaceDessin));
                }
            }
            else if (surfaceDessin.SelectedStrokes.Count == 1 && surfaceDessin.SelectedStrokes[0].GetType() == typeof(LinkStroke))
            {
                surfaceDessin.modifyLinkStrokePath(surfaceDessin.SelectedStrokes[0] as LinkStroke, e.GetPosition(surfaceDessin));

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
                else
                {
                    popUpNameVue.setParameters(strokes[0] as CustomStroke);
                    popUpName.IsOpen = true;
                }
                IsEnabled = false;
            }
        }

        public void Rename(string text)
        {
            popUpName.IsOpen = false;
            CustomStroke stroke = (CustomStroke)surfaceDessin.GetSelectedStrokes()[0];
            stroke.name = text;
            surfaceDessin.RefreshChildren();
            IsEnabled = true;
        }

        public void Rename(string className, string attributes, string methods)
        {
            popUpClass.IsOpen = false;
            ClassStroke stroke = (ClassStroke)surfaceDessin.GetSelectedStrokes()[0];
            stroke.name = className;

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

            surfaceDessin.RefreshChildren();
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
                stroke.DrawingAttributes.Color = Colors.White;
            }
            else // normal line
            {
                stroke.DrawingAttributes.Color = (Color) ColorConverter.ConvertFromString(selectedColor);
            }
            stroke.DrawingAttributes.Width = linkThickness;
            stroke.DrawingAttributes.Height = linkThickness;

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
    }
}
