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
        public DrawingChatView()
        {
            InitializeComponent();
        }

        // ne garde pas encore le meme chat qu'avant
        private void ChangeChatMode(object sender, RoutedEventArgs e)
        {
            grid.Children.Remove(this.ChatView);
            chatButton.Visibility = Visibility.Hidden;
            chatButton.IsHitTestVisible = false;

            var wind = new ChatPopup(this.ChatView, this);

            wind.Show();
        }

        public void IntegrateChat(UserControl chatUserControl)
        {
            chatButton.Visibility = Visibility.Visible;
            chatButton.IsHitTestVisible = true;
            //ColumnDefinition col = new ColumnDefinition();
            //col.Width = GridLength.Auto;
            //this.grid.ColumnDefinitions.Add(col);

            grid.Children.Add(chatUserControl);
        }


    }
}
