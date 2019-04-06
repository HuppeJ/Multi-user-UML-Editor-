using PolyPaint.VueModeles;

namespace PolyPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ((MainWindowViewModel) DataContext).LoginFailed += LoginFailed;
        }

        public void LoginFailed()
        {
            OpenMessagePopup("Login failed.");
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
    }
}
