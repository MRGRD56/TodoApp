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
    public class CompletedTodoForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isDone = (bool)value;
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(isDone ? "#8d8d8d" : "#000000"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
