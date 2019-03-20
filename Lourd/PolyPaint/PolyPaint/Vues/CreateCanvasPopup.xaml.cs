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
            DataContext = this;
        }

        private string _canvasName = "";
        public string CanvasName
        {
            get { return _canvasName; }
            set
            {
                if (_canvasName == value) return;

                _canvasName = value;
                NotifyPropertyChanged("CanvasName");
            }
        }

        private string _canvasPrivacy = "Public";
        public string CanvasPrivacy
        {
            get { return _canvasPrivacy; }
            set
            {
                if (_canvasPrivacy == value) return;

                _canvasPrivacy = value;
                NotifyPropertyChanged("CanvasPrivacy");
            }
        }

        private string _canvasProtection = "Unprotected";
        public string CanvasProtection
        {
            get { return _canvasProtection; }
            set
            {
                if (_canvasProtection == value) return;

                _canvasProtection = value;
                NotifyPropertyChanged("CanvasProtection");
            }
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            if (galleryview != null)
            {
                // galleryview.Rename(_canvasName);
            }
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
            _canvasName = "";
            _canvasPrivacy = "Public";
            _canvasProtection = "Unprotected";
            PasswordTextBox.Password = "";
            NotifyPropertyChanged("CanvasName");
            NotifyPropertyChanged("CanvasPrivacy");
            NotifyPropertyChanged("CanvasProtection");

            galleryview.ClosePopUp();
        }

        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
