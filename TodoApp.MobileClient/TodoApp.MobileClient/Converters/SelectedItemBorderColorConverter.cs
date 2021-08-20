using System;
using System.Drawing;
using System.Globalization;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;

namespace TodoApp.MobileClient.Converters
{
    public class SelectedItemBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = (bool) (value ?? false);
            var borderColor = isSelected ? "#007dba" : "#c5c5c5"; //#cfd8dc 
            return Color.FromHex(borderColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
