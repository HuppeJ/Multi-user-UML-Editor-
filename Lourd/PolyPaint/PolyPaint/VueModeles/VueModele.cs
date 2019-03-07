using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.CustomInk;
using PolyPaint.Modeles;
using PolyPaint.Services;
using PolyPaint.Templates;
using PolyPaint.Utilitaires;

namespace PolyPaint.VueModeles
{

    /// <summary>
    /// Sert d'intermédiaire entre la vue et le modèle.
    /// Expose des commandes et propriétés connectées au modèle aux des éléments de la vue peuvent se lier.
    /// Reçoit des avis de changement du modèle et envoie des avis de changements à la vue.
    /// </summary>
    class VueModele : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Editeur editeur = new Editeur();

        private DrawingService drawingService;

        // Ensemble d'attributs qui définissent l'apparence d'un trait.
        public DrawingAttributes AttributsDessin { get; set; } = new DrawingAttributes();


        public StrokeCollection SelectedStrokes
        {
            get { return editeur.selectedStrokes; }
            set { editeur.selectedStrokes = value; }
        }

        public string SelectedStrokeType
        {
            get { return editeur.SelectedStrokeType; }
            set { ProprieteModifiee(); }
        }


        public string OutilSelectionne
        {
            get { return editeur.OutilSelectionne; }            
            set { ProprieteModifiee(); }
        }        
        
        /* public string CouleurSelectionnee
        {
            get { return editeur.CouleurSelectionnee; }
            set { editeur.CouleurSelectionnee = value; }
        } */

        /*public int TailleTrait
        {
            get { return editeur.TailleTrait; }
            set { editeur.TailleTrait = value; }
        }*/
       
        public StrokeCollection Traits { get; set; }

        #region Commandes pour la vue
        // Commandes sur lesquelles la vue pourra se connecter.
        public RelayCommand<object> Empiler { get; set; }
        public RelayCommand<object> Depiler { get; set; }
        public RelayCommand<string> ChoisirPointe { get; set; }
        public RelayCommand<string> ChoisirOutil { get; set; }
        public RelayCommand<object> Reinitialiser { get; set; }

        public RelayCommand<object> Rotate { get; set; }
        public RelayCommand<string> ChooseStrokeTypeCommand { get; set; }
        #endregion


        /// <summary>
        /// Constructeur de VueModele
        /// On récupère certaines données initiales du modèle et on construit les commandes
        /// sur lesquelles la vue se connectera.
        /// </summary>
        public VueModele()
        {
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, EditeurProprieteModifiee est appelée.
            editeur.PropertyChanged += new PropertyChangedEventHandler(EditeurProprieteModifiee);

            drawingService = new DrawingService();
            drawingService.AddStroke += AddStroke;
            drawingService.UpdateStroke += UpdateStroke;

            // On initialise les attributs de dessin avec les valeurs de départ du modèle.
            AttributsDessin = new DrawingAttributes
            {
                Color = Color.FromRgb(0, 0, 0),
                // AttributsDessin.Color = (Color)ColorConverter.ConvertFromString(editeur.CouleurSelectionnee);
                // AjusterPointe();
                StylusTip = StylusTip.Rectangle,
                Width = 5,
                Height = 5
            };

            Traits = editeur.traits;
            
            // Pour chaque commande, on effectue la liaison avec des méthodes du modèle.            
            Empiler = new RelayCommand<object>(editeur.Empiler, editeur.PeutEmpiler);            
            Depiler = new RelayCommand<object>(editeur.Depiler, editeur.PeutDepiler);
            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChoisirOutil = new RelayCommand<string>(editeur.ChoisirOutil);
            Reinitialiser = new RelayCommand<object>(editeur.Reinitialiser);
            Rotate = new RelayCommand<object>(editeur.Rotate);


            ChooseStrokeTypeCommand = new RelayCommand<string>(editeur.ChooseStrokeTypeCommand);
            

        }
        
        private void AddStroke(Stroke newStroke)
        {
            Console.WriteLine("add de vueModele en provenance du service :) ");
            editeur.traits.Add(newStroke);
        }

        private void UpdateStroke(Stroke newStroke)
        {
            Console.WriteLine("update de vueModele en provenance du service :) ");
            // ne add pas le trait pour vrai..
            editeur.traits.Add(newStroke);
        }


        /// <summary>
        /// Appelee lorsqu'une propriété de VueModele est modifiée.
        /// Un évènement indiquant qu'une propriété a été modifiée est alors émis à partir de VueModèle.
        /// L'évènement qui contient le nom de la propriété modifiée sera attrapé par la vue qui pourra
        /// alors mettre à jour les composants concernés.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected virtual void ProprieteModifiee([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Traite les évènements de modifications de propriétés qui ont été lancés à partir
        /// du modèle.
        /// </summary>
        /// <param name="sender">L'émetteur de l'évènement (le modèle)</param>
        /// <param name="e">Les paramètres de l'évènement. PropertyName est celui qui nous intéresse. 
        /// Il indique quelle propriété a été modifiée dans le modèle.</param>
        private void EditeurProprieteModifiee(object sender, PropertyChangedEventArgs e)
        {     
            if (e.PropertyName == "SelectedStrokeType")
            {
                SelectedStrokeType = editeur.SelectedStrokeType;
            }
            /* else if (e.PropertyName == "CouleurSelectionnee")
            {
                AttributsDessin.Color = (Color)ColorConverter.ConvertFromString(editeur.CouleurSelectionnee);
            }  */
            else //if (e.PropertyName == "OutilSelectionne")
            {
                OutilSelectionne = editeur.OutilSelectionne;
            }  /*              
            else // e.PropertyName == "TailleTrait"
            {               
                AjusterPointe();
            } */               
        }

        /// <summary>
        /// C'est ici qu'est défini la forme de la pointe, mais aussi sa taille (TailleTrait).
        /// Pourquoi deux caractéristiques se retrouvent définies dans une même méthode? Parce que pour créer une pointe 
        /// horizontale ou verticale, on utilise une pointe carrée et on joue avec les tailles pour avoir l'effet désiré.
        /// </summary>
        /* private void AjusterPointe()
        {
            AttributsDessin.StylusTip = StylusTip.Ellipse;
            AttributsDessin.Width = editeur.TailleTrait;
            AttributsDessin.Height = editeur.TailleTrait;
        } */

        #region Initialize DrawingService Command
        private ICommand _initializeDrawingCommand;
        public ICommand InitializeDrawingCommand
        {
            get
            {
                return _initializeDrawingCommand ?? (_initializeDrawingCommand = new RelayCommand<object>(drawingService.Initialize));
            }
        }
        #endregion
    }
}