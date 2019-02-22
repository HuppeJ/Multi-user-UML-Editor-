using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Ink;
using System.Windows.Media;
using PolyPaint.Modeles;
using PolyPaint.Services;
using PolyPaint.Utilitaires;
using Quobject.SocketIoClientDotNet.Client;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;

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

        private ChatService chat = new ChatService();

        // Ensemble d'attributs qui définissent l'apparence d'un trait.
        public DrawingAttributes AttributsDessin { get; set; } = new DrawingAttributes();

        public string OutilSelectionne
        {
            get { return editeur.OutilSelectionne; }            
            set { ProprieteModifiee(); }
        }        
        
        public string CouleurSelectionnee
        {
            get { return editeur.CouleurSelectionnee; }
            set { editeur.CouleurSelectionnee = value; }
        }

        public string PointeSelectionnee
        {
            get { return editeur.PointeSelectionnee; }
            set { ProprieteModifiee(); }
        }

        public int TailleTrait
        {
            get { return editeur.TailleTrait; }
            set { editeur.TailleTrait = value; }
        }
       
        public Collection<CustomStroke> Traits { get; set; }
        
        // Commandes sur lesquels la vue pourra se connecter.
        public RelayCommand<object> Empiler { get; set; }
        public RelayCommand<object> Depiler { get; set; }
        public RelayCommand<string> ChoisirPointe { get; set; }
        public RelayCommand<string> ChoisirOutil { get; set; }
        public RelayCommand<object> Reinitialiser { get; set; }

        public RelayCommand<object> ConnectCommand { get; set; }
        public RelayCommand<object> CreateUserCommand { get; set; }
        public RelayCommand<object> LoginUserCommand { get; set; }


        private void UserCreated(User user)
        {
            Console.WriteLine("test");
        }

        /// <summary>
        /// Constructeur de VueModele
        /// On récupère certaines données initiales du modèle et on construit les commandes
        /// sur lesquelles la vue se connectera.
        /// </summary>
        public VueModele()
        {
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, EditeurProprieteModifiee est appelée.
            editeur.PropertyChanged += new PropertyChangedEventHandler(EditeurProprieteModifiee);

            //chat.UserCreated += UserCreated;

            // On initialise les attributs de dessin avec les valeurs de départ du modèle.
            AttributsDessin = new DrawingAttributes();            
            AttributsDessin.Color = (Color)ColorConverter.ConvertFromString(editeur.CouleurSelectionnee);
            AjusterPointe();

            Traits = editeur.traits;
            
            // Pour chaque commande, on effectue la liaison avec des méthodes du modèle.            
            Empiler = new RelayCommand<object>(editeur.Empiler, editeur.PeutEmpiler);            
            Depiler = new RelayCommand<object>(editeur.Depiler, editeur.PeutDepiler);
            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChoisirPointe = new RelayCommand<string>(editeur.ChoisirPointe);
            ChoisirOutil = new RelayCommand<string>(editeur.ChoisirOutil);
            Reinitialiser = new RelayCommand<object>(editeur.Reinitialiser);

            ConnectCommand = new RelayCommand<object>(chat.Connect);
            //CreateUserCommand = new RelayCommand<object>(chat.CreateUser);
            //LoginUserCommand = new RelayCommand<object>(chat.LoginUser);

        }

        private void ConnectCommand2(string outil) 
        {

            Socket socket = IO.Socket("https://projet-3-228722.appspot.com");

            socket.On(Socket.EVENT_CONNECT, () => {
                Console.WriteLine("fasdf");
            });

            socket.On("hello", () => {
                Console.WriteLine("fasdf");
            });

            // Connect to server
            socket.Connect();

            socket.Emit("test");

            // disconnect from the server
            socket.Close();

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
            if (e.PropertyName == "CouleurSelectionnee")
            {
                AttributsDessin.Color = (Color)ColorConverter.ConvertFromString(editeur.CouleurSelectionnee);
            }                
            else if (e.PropertyName == "OutilSelectionne")
            {
                OutilSelectionne = editeur.OutilSelectionne;
            }                
            else if (e.PropertyName == "PointeSelectionnee")
            {
                PointeSelectionnee = editeur.PointeSelectionnee;
                AjusterPointe();
            }
            else // e.PropertyName == "TailleTrait"
            {               
                AjusterPointe();
            }                
        }

        /// <summary>
        /// C'est ici qu'est défini la forme de la pointe, mais aussi sa taille (TailleTrait).
        /// Pourquoi deux caractéristiques se retrouvent définies dans une même méthode? Parce que pour créer une pointe 
        /// horizontale ou verticale, on utilise une pointe carrée et on joue avec les tailles pour avoir l'effet désiré.
        /// </summary>
        private void AjusterPointe()
        {
            AttributsDessin.StylusTip = (editeur.PointeSelectionnee == "ronde") ? StylusTip.Ellipse : StylusTip.Rectangle;
            AttributsDessin.Width = (editeur.PointeSelectionnee == "verticale") ? 1 : editeur.TailleTrait;
            AttributsDessin.Height = (editeur.PointeSelectionnee == "horizontale") ? 1 : editeur.TailleTrait;
        }
    }
}

public class CustomStroke : Stroke
{
    public CustomStroke(StylusPointCollection pts)
     : base(pts)
    {
        StylusPoints = pts;
    }

    protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
    {
        if (drawingContext == null)
        {
            throw new ArgumentNullException("drawingContext");
        }
        if (null == drawingAttributes)
        {
            throw new ArgumentNullException("drawingAttributes");
        }
        DrawingAttributes originalDa = drawingAttributes.Clone();
        SolidColorBrush brush2 = new SolidColorBrush(drawingAttributes.Color);
        brush2.Freeze();
        drawingContext.DrawRectangle(brush2, null, new Rect(GetTheLeftTopPoint(), GetTheRightBottomPoint()));
    }

    Point GetTheLeftTopPoint()
    {
        if (this.StylusPoints == null)
            throw new ArgumentNullException("StylusPoints");
        StylusPoint tmpPoint = new StylusPoint(double.MaxValue, double.MaxValue);
        foreach (StylusPoint point in this.StylusPoints)
        {
            if ((point.X < tmpPoint.X) || (point.Y < tmpPoint.Y))
                tmpPoint = point;
        }
        return tmpPoint.ToPoint();
    }

    Point GetTheRightBottomPoint()
    {
        if (this.StylusPoints == null)
            throw new ArgumentNullException("StylusPoints");
        StylusPoint tmpPoint = new StylusPoint(0, 0);
        foreach (StylusPoint point in this.StylusPoints)
        {
            if ((point.X > tmpPoint.X) || (point.Y > tmpPoint.Y))
                tmpPoint = point;
        }
        return tmpPoint.ToPoint();
    }
}