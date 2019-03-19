using PolyPaint.CustomInk;
using PolyPaint.CustomInk.Strokes;
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
    public partial class LinkPopup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private WindowDrawing windowDrawing = null;

        public LinkPopup()
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

        private string _linkStyle = "";
        public string LinkStyle
        {
            get { return _linkStyle; }
            set
            {
                if (_linkStyle == value) return;

                _linkStyle = value;
                NotifyPropertyChanged("LinkStyle");
            }
        }

        private string _selectedColor = "";
        public string SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor == value) return;

                _selectedColor = value;
                NotifyPropertyChanged("SelectedColor");
            }
        }

        private int _linkThickness = 1;
        public int LinkThickness
        {
            get { return _linkThickness; }
            set
            {
                if (_linkThickness == value) return;

                _linkThickness = value;
                NotifyPropertyChanged("LinkThickness");
            }
        }

        private void Rename(object sender, RoutedEventArgs e)
        {
            var parent = Parent;
            while (!(parent is WindowDrawing))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            windowDrawing = (WindowDrawing) parent;
            if (windowDrawing != null)
            {
                windowDrawing.Rename(_label, ConvertStyleToInt(_linkStyle), _selectedColor, _linkThickness);
            }
        }

        public void setParameters()
        {
            var parent = Parent;
            while (!(parent is WindowDrawing))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            windowDrawing = (WindowDrawing)parent;
            if (windowDrawing != null)
            {
                LinkStroke stroke = windowDrawing.surfaceDessin.GetSelectedStrokes()[0] as LinkStroke;
                _label = stroke.name;
                _linkStyle = ConvertStyleToString(stroke.style.type);
                _selectedColor = stroke.style.color;
                _linkThickness = stroke.style.thickness;
            }
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkStyle"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedColor"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkThickness"));
        }

        private int ConvertStyleToInt(string style)
        {
            switch (style)
            {
                case "Full":
                    return 0;
                case "Dotted":
                    return 1;
                default:
                    return 0;
            }
        }

        private string ConvertStyleToString(int style)
        {
            switch (style)
            {
                case 0:
                    return "Full";
                case 1:
                    return "Dotted";
                default:
                    return "Full";
            }
        }

        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
