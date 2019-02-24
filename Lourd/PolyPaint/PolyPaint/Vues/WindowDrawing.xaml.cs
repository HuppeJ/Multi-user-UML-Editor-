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

        //private void surfaceDessin_MouseLeftButtonUp(object sender, MouseEventArgs e)
        //{
        //    AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(visual);
        //    //myAdornerLayer.Add(new AnchorPointAdorner(visual));
        //    foreach (UIElement toAdorn in surfaceDessin.Children)
        //        myAdornerLayer.Add(new AnchorPointAdorner(toAdorn));
        //}

        private void DupliquerSelection(object sender, RoutedEventArgs e)
        {          
            surfaceDessin.CopySelection();
            surfaceDessin.Paste();
        }

        private void SupprimerSelection(object sender, RoutedEventArgs e) => surfaceDessin.CutSelection();

    }
}
