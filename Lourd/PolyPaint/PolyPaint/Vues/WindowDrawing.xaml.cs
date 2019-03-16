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
        
        private void RotateSelection(object sender, RoutedEventArgs e) => surfaceDessin.RotateStrokes();

        private void RefreshChildren(object sender, RoutedEventArgs e)
        {
            // pcq click et command ne fonctionnent pas ensemble
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
                    path.Data = newStroke.GetGeometry();
                    surfaceDessin.Children.Add(path);
                    AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
                    myAdornerLayer.Add(new RotateAdorner(path, newStroke, surfaceDessin));
                    myAdornerLayer.Add(new AnchorPointAdorner(path, newStroke, surfaceDessin));
                    Adorner[] ad = myAdornerLayer.GetAdorners(path);
                    myAdornerLayer.Add(new EditionAdorner(path, newStroke, surfaceDessin));
                }
            }

            // when is creating a link from customStroke to canvas point (a link is selected && an achor point has been clicked before)
            if (surfaceDessin.isCreatingLink > 0)
            {
                // what to put in when no guid? i.e not linked to shapeStroke? gi
                surfaceDessin.linkBeingCreated.addToPointToLink(e.GetPosition(surfaceDessin), null, 0);
                surfaceDessin.Strokes.Add(surfaceDessin.linkBeingCreated);

                surfaceDessin.isCreatingLink = 0;
            }
            
            // Pour que les boutons est la bonne couleur
            (DataContext as VueModele)?.ChoisirOutil.Execute("lasso");
        }

        // Bouton pour changer le texte de l'élément sélectionné
        public void RenameSelection()
        {
            StrokeCollection strokes = surfaceDessin.GetSelectedStrokes();
            if(strokes.Count == 1)
            {
                if (((CustomStroke)strokes[0]).type == (int)StrokeTypes.CLASS_SHAPE)
                {
                    popUpClass.IsOpen = true;
                }
                else
                {
                    popUpName.IsOpen = true;
                }
            }
        }

        public void Rename(string text)
        {
            popUpName.IsOpen = false;
            CustomStroke stroke = (CustomStroke)surfaceDessin.GetSelectedStrokes()[0];
            stroke.name = text;
            surfaceDessin.RefreshChildren();
        }

        public void Rename(string className, string attributes, string methods)
        {
            popUpClass.IsOpen = false;
            ClassStroke stroke = (ClassStroke)surfaceDessin.GetSelectedStrokes()[0];
            stroke.name = className;
            stroke.attributes = new List<string>();
            stroke.attributes.Add(attributes);
            stroke.methods = new List<string>();
            stroke.methods.Add(methods);

            surfaceDessin.RefreshChildren();
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
