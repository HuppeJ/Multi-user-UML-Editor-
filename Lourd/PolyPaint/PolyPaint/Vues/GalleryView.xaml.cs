using PolyPaint.Services;
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
    /// Interaction logic for GalleryView.xaml
    /// </summary>
    public partial class GalleryView : UserControl
    {

        MainWindowViewModel dataContext = null;

        public GalleryView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(GalleryView_Loaded);
        }

        void GalleryView_Loaded(object sender, RoutedEventArgs e)
        {
            dataContext = DataContext as MainWindowViewModel;
            if (dataContext != null)
                dataContext.PopupMessageOnGalleryView += OpenMessagePopup;
        }
        
        //public void CloseDisconnectPopup()
        //{
        //    IsEnabled = true;
        //    popUpMessage.IsOpen = false;
        //}

        public void OpenMessagePopup(string message)
        {
            popUpMessageGalleryViewVue.setParameters(message);
            popUpMessageGalleryView.IsOpen = true;
            IsEnabled = false;
        }

        private void CreateCanvas(object sender, RoutedEventArgs e)
        {
            popUpCreateCanvas.IsOpen = true;
            popUpCreateCanvasVue.Initialize();
            IsEnabled = false;
        }

        private void JoinProtectedCanvas(object sender, RoutedEventArgs e)
        {
            popUpJoinProtectedCanvas.IsOpen = true;
            popUpJoinProtectedCanvasVue.Initialize();
            IsEnabled = false;
        }

        private void ChangeCanvasProtection(object sender, RoutedEventArgs e)
        {
            popUpChangeCanvasProtection.IsOpen = true;
            popUpChangeCanvasProtectionVue.Initialize();
            IsEnabled = false;
        }

        public void ClosePopUp()
        {
            popUpCreateCanvas.IsOpen = false;
            popUpJoinProtectedCanvas.IsOpen = false;
            popUpChangeCanvasProtection.IsOpen = false;
            popUpMessageGalleryView.IsOpen = false;
            IsEnabled = true;
        }
    }
}
