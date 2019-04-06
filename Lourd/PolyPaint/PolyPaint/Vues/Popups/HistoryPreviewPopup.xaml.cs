using PolyPaint.Modeles;
using PolyPaint.Utilitaires;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for HistoryPreviewPopup.xaml
    /// </summary>
    public partial class HistoryPreviewPopup
    {
        private HistoryPopup historyPopup = null;

        public HistoryPreviewPopup()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            var parent = Parent;
            while (!(parent is HistoryPopup))
            {
                parent = LogicalTreeHelper.GetParent(parent);
            }

            historyPopup = (HistoryPopup)parent;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            historyPopup.ClosePopup();
        }
    }
}
