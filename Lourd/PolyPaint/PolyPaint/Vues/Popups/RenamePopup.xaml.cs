using PolyPaint.CustomInk;
using PolyPaint.Enums;
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
    public partial class RenamePopup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private WindowDrawing windowDrawing = null;

        public RenamePopup()
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

        private Color _borderColor;
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (_borderColor == value) return;

                _borderColor = value;
                NotifyPropertyChanged("BorderColor");
            }
        }

        private Color _fillColor;
        public Color FillColor
        {
            get { return _fillColor; }
            set
            {
                if (_fillColor == value) return;

                _fillColor = value;
                NotifyPropertyChanged("FillColor");
            }
        }

        private string _lineStyle;
        public string LineStyle
        {
            get { return _lineStyle; }
            set
            {
                if (_lineStyle == value) return;

                _lineStyle = value;
                NotifyPropertyChanged("LineStyle");
            }
        }

        private List<string> _lineStylesList;
        public List<string> LineStylesList
        {
            get { return _lineStylesList; }
            set
            {
                if (_lineStylesList == value) return;

                _lineStylesList = value;
                NotifyPropertyChanged("LineStylesList");
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
                int lineType;
                switch (_lineStyle)
                {
                    case "Full":
                        lineType = 0;         
                        break;
                    case "Dashed":
                        lineType = 1;
                        break;
                    default:
                        lineType = 0;
                        break;
                }
                windowDrawing.Rename(_label, _borderColor, _fillColor, lineType);
            }
        }

        public void setParameters(CustomStroke stroke)
        {
            _label = stroke.name;
            _borderColor = (Color)ColorConverter.ConvertFromString((stroke as ShapeStroke).shapeStyle.borderColor);
            _fillColor = (Color)ColorConverter.ConvertFromString((stroke as ShapeStroke).shapeStyle.backgroundColor);
            switch((stroke as ShapeStroke).shapeStyle.borderStyle)
            {
                case (int)LineStyles.FULL:
                    _lineStyle = "Full";
                    break;
                case (int)LineStyles.DASHED:
                    _lineStyle = "Dashed";
                    break;
                default:
                    _lineStyle = "Full";
                    break;
            }
            _lineStylesList = new List<string> { "Full", "Dashed" };
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderColor"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FillColor"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LineStyle"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LineStylesList"));
        }

        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
