using System.ComponentModel;
using System.Windows;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for CreateCanvasPopup.xaml
    /// </summary>
    public partial class CreateRoomPopup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ChatView chatview = null;

        public CreateRoomPopup()
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
