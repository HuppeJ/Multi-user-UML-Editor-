using PolyPaint.VueModeles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PolyPaint.Vues.Popups
{
    /// <summary>
    /// Logique d'interaction pour ChatPopup.xaml
    /// </summary>
    public partial class ChatPopup : Window
    {
        private UserControl chatUserControl;
        private DrawingChatView drawingChatView;

        public ChatPopup()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            grid.Children.Remove(chatUserControl);

            drawingChatView.IntegrateChat(chatUserControl);
            //this.Close();
            //e.Cancel = true;
            //Do whatever you want here..
        }

        private void CloseChatWindow()
        {
            Close();
        }

        public ChatPopup(UserControl control, DrawingChatView drawingChatView, object datacontext)
            : this()
        {
            DataContext = datacontext;
            chatUserControl = control;
            this.drawingChatView = drawingChatView;

            grid.Children.Add(chatUserControl);

            ((MainWindowViewModel)datacontext).CloseChatWindow += CloseChatWindow;
        }

        
    }
}
