using System.ComponentModel;
using System.Windows;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for ChangeCanvasProtectionPopup.xaml
    /// </summary>
    public partial class ChangeCanvasProtectionPopup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private GalleryView galleryview = null;

        public ChangeCanvasProtectionPopup()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            var parent = Parent;
            while (!(parent is GalleryView))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            galleryview = (GalleryView)parent;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            IsEnabled = true;
            galleryview.ClosePopUp();
        }
    }
}
