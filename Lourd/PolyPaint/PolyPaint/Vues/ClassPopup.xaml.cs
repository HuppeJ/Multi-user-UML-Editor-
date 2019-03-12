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
    public partial class ClassPopup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private WindowDrawing windowDrawing = null;

        public ClassPopup()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region properties
        private string _className = "";
        public string ClassName
        {
            get { return _className; }
            set
            {
                if (_className == value) return;

                _className = value;
                NotifyPropertyChanged("ClassName");
            }
        }

        private string _attributes= "";
        public string Attributes
        {
            get { return _attributes; }
            set
            {
                if (_attributes == value) return;

                _attributes = value;
                NotifyPropertyChanged("Attibutes");
            }
        }

        private string _methods = "";
        public string Methods
        {
            get { return _methods; }
            set
            {
                if (_methods == value) return;

                _methods = value;
                NotifyPropertyChanged("Methods");
            }
        }
        #endregion

        private void Rename(object sender, RoutedEventArgs e)
        {
            var parent = Parent;
            while (!(parent is WindowDrawing))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            windowDrawing = (WindowDrawing)parent;
            if (windowDrawing != null)
            {
                windowDrawing.Rename(_className, _attributes, _methods);
            }
            _className = "";
            _attributes = "";
            _methods = "";

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassName"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Attributes"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Methods"));
        }

        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
