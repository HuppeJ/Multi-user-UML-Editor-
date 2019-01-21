using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PolyPaint.Vues
{
    public partial class WindowChat
    {
        public WindowChat()
        {
            InitializeComponent();
        }

        public void OnClick1(object sender, RoutedEventArgs e)
        {
            btn1.Background = Brushes.LightBlue;
        }
    }
}
