using PolyPaint.Modeles;
using PolyPaint.Utilitaires;
using System.ComponentModel;
using System.Windows;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for JoinRoomPopup.xaml
    /// </summary>
    public partial class JoinRoomPopup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ChatView chatview = null;
        public AsyncObservableCollection<Room> joinableRooms = new AsyncObservableCollection<Room>();
        public Room selectedRoom;

        public JoinRoomPopup()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            var parent = Parent;
            while (!(parent is ChatView))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            chatview = (ChatView)parent;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            chatview.ClosePopup();
        }
    }
}
