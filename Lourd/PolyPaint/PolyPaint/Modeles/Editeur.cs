using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Threading;
using PolyPaint.CustomInk;
using PolyPaint.CustomInk.Strokes;
using PolyPaint.Enums;
using PolyPaint.Services;

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
        private StrokeCollection traitsRetires = new StrokeCollection();
        public StrokeCollection selectedStrokes = new StrokeCollection();

        // TODO Remove fix
        private bool isStackUpToDate = true;
        private CustomStroke strokeEmpilable;

        public event EventHandler<CustomStroke> AddStrokeFromModel;


        // StrokeType selected
        private string selectedStrokeType = "";
        public string SelectedStrokeType
        {
            get { return selectedStrokeType; }
            set { selectedStrokeType = value; ProprieteModifiee(); }
        }

        // Outil actif dans l'éditeur
        private string outilSelectionne = "lasso";
        public string OutilSelectionne
        {
            get { return outilSelectionne; }
            set { outilSelectionne = value; ProprieteModifiee(); }
        }

        internal CustomStroke AddStrokeFromView(CustomStroke selectedStroke/*StylusPoint firstPoint, StrokeTypes strokeType*/)
        {
            // Deja fait par le binding
            //traits.Add(selectedStroke);

            StrokeAdded(selectedStroke);

            StrokeCollection selectedStrokes = new StrokeCollection { selectedStroke };
            OutilSelectionne = "lasso";

            return selectedStroke;
        }

        private bool isInTraits(CustomStroke selectedStroke)
        {
            foreach(CustomStroke stroke in traits)
            {
                if(stroke.guid == selectedStroke.guid)
                {
                    return true;
                }
            }

            return false;
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
        public bool PeutEmpiler(object o)
        {
            if (!isStackUpToDate)
            {
                isStackUpToDate = true;
                return false;
            }
            else if (traits.Count > 0)
            {
                bool isFound = false;
                foreach (CustomStroke stroke in traits)
                {
                    if (DrawingService.localAddedStrokes.Contains(stroke.guid.ToString()))
                    {
                        strokeEmpilable = stroke;
                        isFound = true;
                    }
                }
                return isFound && !DrawingService.remoteSelectedStrokes.Contains(strokeEmpilable.guid.ToString());
            }
            else
                return false;
        }
        // On retire le trait le plus récent de la surface de dessin et on le place sur une pile.
        public void Empiler(object o)
        {
            try
            {
                (o as CustomInkCanvas).UpdateAnchorPointsAndLinks(new StrokeCollection { strokeEmpilable });
                isStackUpToDate = false;
                traitsRetires.Add(strokeEmpilable);
                traits.Remove(strokeEmpilable);
                StrokeCollection strokes = new StrokeCollection { strokeEmpilable };
                DrawingService.RemoveShapes(strokes);
            }
            catch { }

        }

        // S'il y a au moins 1 trait sur la pile de traits retirés, il est possible d'exécuter Depiler.
        public bool PeutDepiler(object o)
        {
            if (!isStackUpToDate)
            {
                isStackUpToDate = true;
                return false;
            }
            else
            {
                return (traitsRetires.Count > 0);
            }
        }
        // On retire le trait du dessus de la pile de traits retirés et on le place sur la surface de dessin.
        public void Depiler(object o)
        {
            try
            {
                isStackUpToDate = false;
                CustomStroke trait = (CustomStroke)traitsRetires.Last();

                if (trait.isLinkStroke())
                {
                    LinkStroke linkStroke = trait as LinkStroke;

                    linkStroke.from.SetDefaults();
                    linkStroke.to.SetDefaults();
                }
                else
                {
                    ShapeStroke shapeStroke = trait as ShapeStroke;
                    shapeStroke.linksTo = new List<string> { };
                    shapeStroke.linksFrom = new List<string> { };
                }

                traits.Add(trait);
                if(trait.isLinkStroke())
                {
                    DrawingService.CreateLink(trait as LinkStroke);
                } else
                {
                    DrawingService.CreateShape(trait as ShapeStroke);
                }
                traitsRetires.Remove(trait);
            }
            catch { }
        }

        // L'outil actif devient celui passé en paramètre.
        public void ChoisirOutil(string outil) {
            OutilSelectionne = outil;
            if (outil.Equals("efface_trait") || outil.Equals("lasso"))
            {
                SelectedStrokeType = "nothing";
            }
        }

        public bool PeutReinitialiser(object o) => (true);
        // On vide la surface de dessin de tous ses traits.
        public void Reinitialiser(object o)
        {
            if (traits.Count > 0)
            {
                DrawingService.Reset();
                traits.Clear();
            }
        }

        public void ChooseStrokeTypeCommand(string strokeType) {
            // Automatically select crayon
            ChoisirOutil("crayon");
            SelectedStrokeType = strokeType;
        }
    }
}