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

        // Alex pour savoir quand un stroke a ete ajoute
        private void surfaceDessin_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((DataContext as VueModele)?.OutilSelectionne == "crayon")
            {
                CustomStroke newStroke = (DataContext as VueModele).AddStrokeFromView(
                    (CustomStroke) surfaceDessin.SelectedStrokes[surfaceDessin.SelectedStrokes.Count - 1]
                );
                surfaceDessin.Select(new StrokeCollection{ newStroke });

                Path path = new Path();
                path.Data = newStroke.GetGeometry();
                surfaceDessin.Children.Add(path);
                //newStroke.GetWidenedPathGeometry();
                AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(path);
                //myAdornerLayer.Add(new AnchorPointAdorner(path));
                myAdornerLayer.Add(new RotateAdorner(path, newStroke));

            }
        }

        private void OnStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            (DataContext as VueModele)?.OnStrokeCollectedEvent(sender, e);
        }

    }
}
