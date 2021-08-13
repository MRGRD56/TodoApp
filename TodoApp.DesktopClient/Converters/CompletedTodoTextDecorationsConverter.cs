using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TodoApp.DesktopClient.Converters
{
    public class CompletedTodoTextDecorationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isDone = (bool)value;
            return isDone ? TextDecorations.Strikethrough : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
