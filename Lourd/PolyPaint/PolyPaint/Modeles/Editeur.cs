using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Ink;
using System.Windows.Input;
using PolyPaint.CustomInk;
using PolyPaint.Enums;

namespace PolyPaint.Modeles
{
    /// <summary>
    /// Modélisation de l'éditeur.
    /// Contient ses différents états et propriétés ainsi que la logique
    /// qui régis son fonctionnement.
    /// </summary>
    class Editeur : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public StrokeCollection traits = new StrokeCollection();
        public StrokeCollection selectedStrokes = new StrokeCollection();
        private StrokeCollection traitsRetires = new StrokeCollection();


        public event EventHandler<CustomStroke> AddStrokeFromModel;


        // StrokeType selected
        private string selectedStrokeType = "CLASS_SHAPE";
        public string SelectedStrokeType
        {
            get { return selectedStrokeType; }
            set { selectedStrokeType = value; ProprieteModifiee(); }
        }

        // Outil actif dans l'éditeur
        private string outilSelectionne = "crayon";
        public string OutilSelectionne
        {
            get { return outilSelectionne; }
            set { outilSelectionne = value; ProprieteModifiee(); }
        }

        // Couleur des traits tracés par le crayon.
        private string couleurSelectionnee = "Black";
        public string CouleurSelectionnee
        {
            get { return couleurSelectionnee; }
            // Lorsqu'on sélectionne une couleur c'est généralement pour ensuite dessiner un trait.
            // C'est pourquoi lorsque la couleur est changée, l'outil est automatiquement changé pour le crayon.
            set
            {
                couleurSelectionnee = value;
                OutilSelectionne = "crayon";
                ProprieteModifiee();
            }
        }

        // Grosseur des traits tracés par le crayon.
        private int tailleTrait = 11;
        public int TailleTrait
        {
            get { return tailleTrait; }
            // Lorsqu'on sélectionne une taille de trait c'est généralement pour ensuite dessiner un trait.
            // C'est pourquoi lorsque la taille est changée, l'outil est automatiquement changé pour le crayon.
            set
            {
                tailleTrait = value;
                OutilSelectionne = "crayon";
                ProprieteModifiee();
            }
        }

        internal StrokeCollection AddStrokeFromView(CustomStroke selectedStroke/*StylusPoint firstPoint, StrokeTypes strokeType*/)
        {
            // Deja fait par le binding
            //traits.Add(selectedStroke);

            StrokeAdded(selectedStroke);

            StrokeCollection selectedStrokes = new StrokeCollection { selectedStroke };
            OutilSelectionne = "lasso";

            return selectedStrokes;
        }

        internal void AddStrokeFromService(CustomStroke selectedStroke/*StylusPoint firstPoint, StrokeTypes strokeType*/)
        {
            // Il faudrait pouvoir verifier que le trait n'existe pas, ou pouvoir le modifier... on a acces au guid?
            if (!traits.Contains(selectedStroke))
            {
                traits.Add(selectedStroke);
            }
        }

        private void StrokeAdded(CustomStroke stroke)
        {
            AddStrokeFromModel?.Invoke(this, stroke);
        }

        public static object StrokeType { get; private set; }

        /// <summary>
        /// Appelee lorsqu'une propriété d'Editeur est modifiée.
        /// Un évènement indiquant qu'une propriété a été modifiée est alors émis à partir d'Editeur.
        /// L'évènement qui contient le nom de la propriété modifiée sera attrapé par VueModele qui pourra
        /// alors prendre action en conséquence.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected void ProprieteModifiee([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // S'il y a au moins 1 trait sur la surface, il est possible d'exécuter Empiler.
        public bool PeutEmpiler(object o) => (traits.Count > 0); 
        // On retire le trait le plus récent de la surface de dessin et on le place sur une pile.
        public void Empiler(object o)
        {
            try
            {
                Stroke trait = traits.Last();
                traitsRetires.Add(trait);
                traits.Remove(trait);               
            }
            catch { }

        }

        // S'il y a au moins 1 trait sur la pile de traits retirés, il est possible d'exécuter Depiler.
        public bool PeutDepiler(object o) => (traitsRetires.Count > 0);
        // On retire le trait du dessus de la pile de traits retirés et on le place sur la surface de dessin.
        public void Depiler(object o)
        {
            try
            {
                Stroke trait = traitsRetires.Last();
                traits.Add(trait);
                traitsRetires.Remove(trait);
            }
            catch { }         
        }

        // L'outil actif devient celui passé en paramètre.
        public void ChoisirOutil(string outil) => OutilSelectionne = outil;

        // On vide la surface de dessin de tous ses traits.
        public void Reinitialiser(object o) => traits.Clear();

        public void ChooseStrokeTypeCommand(string strokeType) {
            // Automatically select crayon
            OutilSelectionne = "crayon";
            SelectedStrokeType = strokeType;
            ProprieteModifiee();
        }
        
        // Rotate selected strokes of 90 degrees
        public void Rotate(object o)
        {
            foreach (CustomStroke stroke in selectedStrokes)
            {
                stroke.Rotate();
            }
        }
    }
}