using System;
using System.Windows;
using System.Windows.Controls;
using PolyPaint.Modeles;
using PolyPaint.Utilitaires;
using PolyPaint.VueModeles;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        MainWindowViewModel dataContext = null;

        public ChatView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ChatView_Loaded);
        }

        void ChatView_Loaded(object sender, RoutedEventArgs e)
        {
            dataContext = DataContext as MainWindowViewModel;
            if(dataContext != null)
            dataContext.CreateChatRoomFailed += CreateChatRoomFailed;
        }

        public void CreateChatRoomFailed()
        {
            OpenMessagePopup("Chat room already exists.");
        }

        public void CloseMessagePopup()
        {
            IsEnabled = true;
            popUpMessage.IsOpen = false;
        }

        public void OpenMessagePopup(string message)
        {
            popUpMessageVue.setParameters(message);
            popUpMessage.IsOpen = true;
            IsEnabled = false;
        }
   
        private void CreateRoomPopup(object sender, EventArgs e)
        {
            popUpCreateRoomVue.Initialize();
            popUpCreateRoomVue.RoomNameTextBox.Text = "";
            popUpCreateRoom.IsOpen = true;
            IsEnabled = false;
        }

        private void JoinRoomPopup(object sender, EventArgs e)
        {
            popUpJoinRoomVue.Initialize();
            popUpJoinRoomVue.joinableRooms = ((Button)sender).Tag as AsyncObservableCollection<Room>;
            popUpJoinRoom.IsOpen = true;
            IsEnabled = false;
        }

        public void ClosePopup()
        {
            popUpCreateRoom.IsOpen = false;
            popUpJoinRoom.IsOpen = false;
            IsEnabled = true;
        }
    }
}
