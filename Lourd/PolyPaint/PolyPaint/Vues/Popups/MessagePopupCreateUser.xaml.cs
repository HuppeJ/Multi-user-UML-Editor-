using PolyPaint.CustomInk;
using PolyPaint.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for RenamePopup.xaml
    /// </summary>
    public partial class MessagePopupCreateUser : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CreateUserView windowDrawing = null;

        public MessagePopupCreateUser()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string _label = "";
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label == value) return;

                _label = value;
                NotifyPropertyChanged("Label");
            }
        }

        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            var parent = Parent;
            while (!(parent is CreateUserView))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            windowDrawing = (CreateUserView)parent;
            if (windowDrawing != null)
            {
                windowDrawing.CloseMessagePopup();
            }
        }

        public void setParameters(string message)
        {
            _label = message;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
