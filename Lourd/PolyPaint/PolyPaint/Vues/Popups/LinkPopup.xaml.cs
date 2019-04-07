using PolyPaint.CustomInk;
using PolyPaint.CustomInk.Strokes;
using PolyPaint.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

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

        #region properties
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

        private List<string> _linkStylesList;
        public List<string> LinkStylesList
        {
            get { return _linkStylesList; }
            set
            {
                if (_linkStylesList == value) return;

                _linkStylesList = value;
                NotifyPropertyChanged("LinkStylesList");
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

        private List<string> _linkTypesList;
        public List<string> LinkTypesList
        {
            get { return _linkTypesList; }
            set
            {
                if (_linkTypesList == value) return;

                _linkTypesList = value;
                NotifyPropertyChanged("LinkTypesList");
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

        private string _linkThickness = "";
        public string LinkThickness
        {
            get { return _linkThickness; }
            set
            {
                if (_linkThickness == value) return;

                _linkThickness = value;
                NotifyPropertyChanged("LinkThickness");
            }
        }

        private List<string> _linkThicknessesList;
        public List<string> LinkThicknessesList
        {
            get { return _linkThicknessesList; }
            set
            {
                if (_linkThicknessesList == value) return;

                _linkThicknessesList = value;
                NotifyPropertyChanged("LinkThicknessesList");
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
        #endregion

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
                int linkType;
                switch (_linkType)
                {
                    case "Line":
                        linkType = 0;
                        break;
                    case "One way association":
                        linkType = 1;
                        break;
                    case "Two way association":
                        linkType = 2;
                        break;
                    case "Heritage":
                        linkType = 3;
                        break;
                    case "Aggregation":
                        linkType = 4;
                        break;
                    case "Composition":
                        linkType = 5;
                        break;
                    default:
                        linkType = 0;
                        break;
                }

                int linkStyle;
                switch (_linkStyle)
                {
                    case "Full":
                        linkStyle = 0;
                        break;
                    case "Dotted":
                        linkStyle = 1;
                        break;
                    default:
                        linkStyle = 0;
                        break;
                }

                int linkThickness;
                switch (_linkThickness)
                {
                    case "Thin":
                        linkThickness = 0;
                        break;
                    case "Normal":
                        linkThickness = 1;
                        break;
                    case "Thick":
                        linkThickness = 2;
                        break;
                    default:
                        linkThickness = 0;
                        break;
                }

                windowDrawing.EditLink(_label, linkType, linkStyle, _selectedColor, linkThickness, _multiplicityFrom, _multiplicityTo);
            }
        }

        public void setParameters(LinkStroke linkStroke, CustomInkCanvas canvas)
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
                _selectedColor = stroke.style.color;
                _multiplicityFrom = stroke.from.multiplicity;
                _multiplicityTo = stroke.to.multiplicity;

                switch (stroke.linkType)
                {
                    case (int)LinkTypes.LINE:
                        _linkType = "Line";
                        break;
                    case (int)LinkTypes.HERITAGE:
                        _linkType = "Heritage";
                        break;
                    case (int)LinkTypes.ONE_WAY_ASSOCIATION:
                        _linkType = "One way association";
                        break;
                    case (int)LinkTypes.TWO_WAY_ASSOCIATION:
                        _linkType = "Two way association";
                        break;
                    case (int)LinkTypes.COMPOSITION:
                        _linkType = "Composition";
                        break;
                    case (int)LinkTypes.AGGREGATION:
                        _linkType = "Aggregation";
                        break;
                    default:
                        _linkType = "Line";
                        break;
                }

                switch (stroke.style.type)
                {
                    case (int)LinkStyles.FULL:
                        _linkStyle = "Full";
                        break;
                    case (int)LinkStyles.DOTTED:
                        _linkStyle = "Dotted";
                        break;
                    default:
                        _linkStyle = "Full";
                        break;
                }

                switch (stroke.style.thickness)
                {
                    case (int)LinkThicknesses.THIN:
                        _linkThickness = "Thin";
                        break;
                    case (int)LinkThicknesses.NORMAL:
                        _linkThickness = "Normal";
                        break;
                    case (int)LinkThicknesses.THICK:
                        _linkThickness = "Thick";
                        break;
                    default:
                        _linkThickness = "Thin";
                        break;
                }
            }

            if (isBetweenTwoClasses(linkStroke, canvas))
            {
                _linkTypesList = new List<string> { "Line", "One way association", "Two way association", "Heritage", "Aggregation", "Composition" };
            }
            else if (isLinkedToOneClass(linkStroke, canvas))
            {
                _linkTypesList = new List<string> { "Line", "One way association", "Two way association" };
            }
            else if (isLinkedToProcess(linkStroke, canvas))
            {
                _linkTypesList = new List<string> { "One way association" };
            }
            else
            {
                _linkTypesList = new List<string> { "Line", "One way association", "Two way association"};
            }
            _linkStylesList = new List<string> { "Full", "Dotted" };
            _linkThicknessesList = new List<string> { "Thin", "Normal", "Thick" };

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkStyle"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkType"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedColor"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkThickness"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MultiplicityFrom"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MultiplicityTo"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkStylesList"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkTypesList"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LinkThicknessesList"));
        }

        private bool isLinkedToOneClass(LinkStroke linkStroke, CustomInkCanvas canvas)
        {
            CustomStroke fromStroke = new Stroke(new StylusPointCollection { new StylusPoint(0,0)}) as CustomStroke;
            CustomStroke toStroke = new Stroke(new StylusPointCollection { new StylusPoint(0, 0) }) as CustomStroke;

            string from = "";
            string to = "";
            from = linkStroke.from?.formId;
            to = linkStroke.to?.formId;
            if (from != null)
            {
                canvas.StrokesDictionary.TryGetValue(from, out fromStroke);
            }
            if (to != null)
            {
                canvas.StrokesDictionary.TryGetValue(to, out toStroke);
            }

            return fromStroke is ClassStroke || toStroke is ClassStroke;
        }

        private bool isLinkedToProcess(LinkStroke linkStroke, CustomInkCanvas canvas)
        {
            CustomStroke fromStroke = new Stroke(new StylusPointCollection { new StylusPoint(0, 0) }) as CustomStroke;
            CustomStroke toStroke = new Stroke(new StylusPointCollection { new StylusPoint(0, 0) }) as CustomStroke;

            string from = "";
            string to = "";
            from = linkStroke.from?.formId;
            to = linkStroke.to?.formId;
            if (from != null)
            {
                canvas.StrokesDictionary.TryGetValue(from, out fromStroke);
            }
            if (to != null)
            {
                canvas.StrokesDictionary.TryGetValue(to, out toStroke);
            }

            return fromStroke != null && fromStroke.isProccessStroke() || toStroke != null && toStroke.isProccessStroke();
        }

        private bool isBetweenTwoClasses(LinkStroke linkStroke, CustomInkCanvas canvas)
        {
            CustomStroke fromStroke = new Stroke(new StylusPointCollection { new StylusPoint(0, 0) }) as CustomStroke;
            CustomStroke toStroke = new Stroke(new StylusPointCollection { new StylusPoint(0, 0) }) as CustomStroke;

            string from = "";
            string to = "";
            from = linkStroke.from?.formId;
            to = linkStroke.to?.formId;
            if (from != null)
            {
                canvas.StrokesDictionary.TryGetValue(from, out fromStroke);
            }
            if (to != null)
            {
                canvas.StrokesDictionary.TryGetValue(to, out toStroke);
            }

            return fromStroke is ClassStroke && toStroke is ClassStroke;
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
