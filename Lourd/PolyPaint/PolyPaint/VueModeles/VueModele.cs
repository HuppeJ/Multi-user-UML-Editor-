using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.CustomInk;
using PolyPaint.Enums;
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

        private AsyncObservableCollection<HistoryData> _historyLogs = new AsyncObservableCollection<HistoryData>();
        public AsyncObservableCollection<HistoryData> historyLogs
        {
            get { return _historyLogs; }
            set
            {
                _historyLogs = value;
                ProprieteModifiee();
            }
        }

        private HistoryData _selectedHistoryLog;
        public HistoryData selectedHistoryLog
        {
            get { return _selectedHistoryLog; }
            set
            {
                _selectedHistoryLog = value;
                ProprieteModifiee();
            }
        }

        private string _thumbnail;
        public string thumbnail
        {
            get { return _thumbnail; }
            set
            {
                _thumbnail = value;
                ProprieteModifiee();
            }
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

        internal CustomStroke AddStrokeFromView(CustomStroke selectedStroke/*StylusPoint firstPoint, StrokeTypes strokeType*/)
        {
            return editeur.AddStrokeFromView(selectedStroke);
        }


        public RelayCommand<string> ChooseStrokeTypeCommand { get; set; }
        //Command for sending editor actions to server
        public RelayCommand<CustomStroke> SendNewStrokeCommand { get; set; }
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
            
            // DrawingService.AddStroke += AddStroke;
            // drawingService.UpdateStroke += UpdateStroke;

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
            Reinitialiser = new RelayCommand<object>(editeur.Reinitialiser, editeur.PeutReinitialiser);

            ChooseStrokeTypeCommand = new RelayCommand<string>(editeur.ChooseStrokeTypeCommand);


            //Command for sending editor actions to server
            SendNewStrokeCommand = new RelayCommand<CustomStroke>(SendNewStroke);
            editeur.AddStrokeFromModel += OnStrokeCollectedEvent;

            DrawingService.UpdateHistory += UpdateHistory;
        }

        /// <summary>
        ///     Handler for InkCanvas event
        /// </summary>
        public void OnStrokeCollectedEvent(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            SendNewStrokeCommand.Execute(e.Stroke);
        }

        /// <summary>
        ///     Handler for Editor event (unstack)
        /// </summary>
        public void OnStrokeCollectedEvent(object sender, Stroke stroke)
        {
            SendNewStrokeCommand.Execute(stroke);
        }

        public void SendNewStroke(CustomStroke stroke)
        {
            //drawingService.createStroke();
            Coordinates coordinates = new Coordinates(stroke.StylusPoints[0].X, stroke.StylusPoints[0].Y);
            ShapeStyle shapeStyle = new ShapeStyle();
            shapeStyle.coordinates = coordinates;
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

        /*
        private void AddStroke(Stroke stroke)
        {
            editeur.AddStrokeFromService((CustomStroke)stroke);
        }
        */

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
        #region GetCanvasHistoryLogCommand
        private ICommand _getCanvasHistoryLogCommand;
        public ICommand GetCanvasHistoryLogCommand
        {
            get
            {
                return _getCanvasHistoryLogCommand ?? (_getCanvasHistoryLogCommand = new RelayCommand<object>(DrawingService.GetHistoryLog));
            }
        }
        #endregion

        #region UpdateThumbnailCommand
        private ICommand _updateThumbnailCommand;
        public ICommand UpdateThumbnailCommand
        {
            get
            {
                return _updateThumbnailCommand ?? (_updateThumbnailCommand = new RelayCommand<object>(UpdateThumbnail));
            }
        }

        private void UpdateThumbnail(object o)
        {
            _thumbnail = o as string;
        }
        #endregion

        private void UpdateHistory(History history)
        {
            historyLogs = new AsyncObservableCollection<HistoryData>(history.history);
        }

        #region Initialize DrawingService Command
        private ICommand _initializeDrawingCommand;
        public ICommand InitializeDrawingCommand
        {
            get
            {
                return _initializeDrawingCommand ?? (_initializeDrawingCommand = new RelayCommand<object>(DrawingService.Initialize));
            }
        }
        #endregion
    }
}