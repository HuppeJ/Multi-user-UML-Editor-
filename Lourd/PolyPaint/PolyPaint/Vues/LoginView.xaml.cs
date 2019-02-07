using PolyPaint.VueModeles;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        //private void ConnectCommand(object sender, RoutedEventArgs e)
        //{

        //    Socket socket = IO.Socket("https://projet-3-228722.appspot.com");

        //    socket.On(Socket.EVENT_CONNECT, () => {
        //        Console.WriteLine("fasdf");
        //    });

        //    socket.On("hello", () => {
        //        Console.WriteLine("fasdf");
        //    });

        //    // Connect to server
        //    socket.Connect();

        //    socket.Emit("test");

        //    // disconnect from the server
        //    socket.Close();

        //}
    }
}
