using System.ComponentModel;
using System.Windows;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for CreateCanvasPopup.xaml
    /// </summary>
    public partial class CreateCanvasPopup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private GalleryView galleryview = null;

        public CreateCanvasPopup()
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
