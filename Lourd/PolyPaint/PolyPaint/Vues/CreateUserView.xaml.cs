using PolyPaint.VueModeles;
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
using System.Windows.Shapes;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for CreateUserView.xaml
    /// </summary>
    public partial class CreateUserView : UserControl
    {
        MainWindowViewModel dataContext = null;

        public CreateUserView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(CreateUserView_Loaded);
        }
        
        void CreateUserView_Loaded(object sender, RoutedEventArgs e)
        {
            dataContext = DataContext as MainWindowViewModel;
            dataContext.CreateUserFailed += CreateUserFailed;
        }

        public void CreateUserFailed()
        {
            OpenMessagePopup("Create user failed.");
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
