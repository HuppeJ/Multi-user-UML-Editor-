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
                int lineType;
                switch (_lineStyle)
                {
                    case "Full":
                        lineType = 0;
                        break;
                    case "Dashed":
                        lineType = 1;
                        break;
                    case "Dotted":
                        lineType = 2;
                        break;
                    default:
                        lineType = 0;
                        break;
                }
                windowDrawing.Rename(_className, _attributes, _methods, _borderColor, _fillColor, lineType);
            }
        }

        public void setParameters(ClassStroke stroke)
        {
            _className = stroke.name;
            _attributes = ListToString(stroke.attributes);
            _methods = ListToString(stroke.methods);
            _borderColor = (Color)ColorConverter.ConvertFromString(stroke.shapeStyle.borderColor);
            _fillColor = (Color)ColorConverter.ConvertFromString(stroke.shapeStyle.backgroundColor);
            switch ((stroke as ShapeStroke).shapeStyle.borderStyle)
            {
                case (int)LineStyles.FULL:
                    _lineStyle = "Full";
                    break;
                case (int)LineStyles.DASHED:
                    _lineStyle = "Dashed";
                    break;
                case (int)LineStyles.DOTTED:
                    _lineStyle = "Dotted";
                    break;
                default:
                    _lineStyle = "Full";
                    break;
            }
            _lineStylesList = new List<string> { "Full", "Dashed"};
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClassName"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Attributes"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Methods"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderColor"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FillColor"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LineStyle"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LineStylesList"));
        }

        private string ListToString(List<string> strings)
        {
            StringBuilder sb = new StringBuilder();
            foreach(string str in strings)
            {
                sb.Append(str);
                sb.AppendLine();
            }
            if(sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
