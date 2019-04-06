using PolyPaint.VueModeles;
using PolyPaint.Vues.Popups;
using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for DrawingChatView.xaml
    /// </summary>
    public partial class DrawingChatView : UserControl
    {
        MainWindowViewModel dataContext = null;

        public DrawingChatView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DrawingChat_Loaded);
        }

        void DrawingChat_Loaded(object sender, RoutedEventArgs e)
        {
            dataContext = DataContext as MainWindowViewModel;
            if (dataContext.IsChatWindowOpened)
            {
                grid.Children.Remove(ChatView);
                chatButton.Visibility = Visibility.Hidden;
                chatButton.IsHitTestVisible = false;

                var wind = new ChatPopup(ChatView, this, dataContext);
                wind.Show();
            }
        }

        private void ChangeChatMode(object sender, RoutedEventArgs e)
        {
            grid.Children.Remove(ChatView);
            chatButton.Visibility = Visibility.Hidden;
            chatButton.IsHitTestVisible = false;

            var wind = new ChatPopup(ChatView, this, ((Button)sender).Tag);
            wind.Show();
        }

        public void IntegrateChat(UserControl chatUserControl)
        {
            chatButton.Visibility = Visibility.Visible;
            chatButton.IsHitTestVisible = true;

            grid.Children.Add(chatUserControl);
        }
    }
}
