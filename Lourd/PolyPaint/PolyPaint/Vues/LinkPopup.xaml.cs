using PolyPaint.CustomInk;
using PolyPaint.CustomInk.Strokes;
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

        private string _linkType = "";
        public string LinkType
        {
            get { return _linkType; }
            set
            {
                if (_linkType == value) return;

                _linkType = value;
                NotifyPropertyChanged("LinkType");
            }
        }

        private string _selectedColor;
        public string SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor == value || value == null) return;

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

        private string _multiplicityFrom = "";
        public string MultiplicityFrom
        {
            get { return _multiplicityFrom; }
            set
            {
                if (MultiplicityFrom == value) return;

                _multiplicityFrom = value;
                NotifyPropertyChanged("MultiplicityFrom");
            }
        }

        private string _multiplicityTo = "";
        public string MultiplicityTo
        {
            get { return _multiplicityTo; }
            set
            {
                if (_multiplicityTo == value) return;

                _multiplicityTo = value;
                NotifyPropertyChanged("MultiplicityTo");
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
                windowDrawing.EditLink(_label, ConvertTypeToInt(_linkType),ConvertStyleToInt(_linkStyle), _selectedColor, _linkThickness, _multiplicityFrom, _multiplicityTo);
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
                _linkType = ConvertTypeToString(stroke.linkType);
                _selectedColor = stroke.style.color;
                _linkThickness = stroke.style.thickness;
                _multiplicityFrom = stroke.from.multiplicity;
                _multiplicityTo = stroke.to.multiplicity;
            }
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkStyle"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkType"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedColor"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkThickness"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MultiplicityFrom"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MultiplicityTo"));
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

        private int ConvertTypeToInt(string style)
        {
            switch (style)
            {
                case "Line":
                    return 0;
                case "One way association":
                    return 1;
                case "Two way association":
                    return 2;
                case "Heritage":
                    return 3;
                case "Aggregation":
                    return 4;
                case "Composition":
                    return 5;
                default:
                    return 0;
            }
        }

        private string ConvertTypeToString(int style)
        {
            switch (style)
            {
                case 0:
                    return "Line";
                case 1:
                    return "One way association";
                case 2:
                    return "Two way association";
                case 3:
                    return "Heritage";
                case 4:
                    return "Aggregation";
                case 5:
                    return "Composition";
                default:
                    return "Line";
            }
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
