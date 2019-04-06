using PolyPaint.Modeles;
using PolyPaint.Utilitaires;
using System.ComponentModel;
using System.Windows;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for JoinRoomPopup.xaml
    /// </summary>
    public partial class HistoryPopup
    {
        private WindowDrawing drawingview = null;

        public HistoryPopup()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            var parent = Parent;
            while (!(parent is WindowDrawing))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            drawingview = (WindowDrawing)parent;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            drawingview.ClosePopup();
        }
    }
}
