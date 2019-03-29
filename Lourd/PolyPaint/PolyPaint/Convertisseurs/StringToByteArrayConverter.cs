using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PolyPaint.Convertisseurs
{
    class StringToByteArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                string image = value as string;
                return System.Convert.FromBase64String(image);
            }

            return "Hello";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }
}
