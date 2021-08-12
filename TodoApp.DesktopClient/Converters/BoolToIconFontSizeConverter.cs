using System;
using System.Globalization;
using System.Windows.Data;

namespace TodoApp.DesktopClient.Converters
{
    public class BoolToIconFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;
            return boolValue ? 14D : 16D;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
