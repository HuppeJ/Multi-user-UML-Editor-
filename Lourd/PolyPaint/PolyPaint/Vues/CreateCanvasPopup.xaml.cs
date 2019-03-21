using PolyPaint.CustomInk;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for RenamePopup.xaml
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
