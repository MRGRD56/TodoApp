using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TodoApp.DesktopClient.Converters
{
    public class SelectedItemBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = (bool) (value ?? false);
            var borderColor = isSelected ? "#007dba" : "#c5c5c5";
            return new SolidColorBrush((Color) ColorConverter.ConvertFromString(borderColor));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
