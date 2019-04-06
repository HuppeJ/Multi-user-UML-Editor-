using System;
using System.Globalization;
using System.Windows.Data;

namespace PolyPaint.Convertisseurs
{
    public class ConvertisseurDate: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string timeString = value as string;
            long time = long.Parse(timeString);

            return value = DateTimeOffset.FromUnixTimeMilliseconds(time).DateTime;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value;
        }

    }
}
