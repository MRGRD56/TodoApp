using System;
using System.Globalization;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;

namespace TodoApp.MobileClient.Converters
{
    public class CompletedTodoTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isDone = (bool)value;
            return Color.FromHex(isDone ? "#8d8d8d" : "#000000");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
