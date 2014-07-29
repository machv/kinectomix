using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Mach.Kinectomix.LevelEditor
{
    public class NotNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool negate = parameter != null ? (bool)parameter : false;

            if (negate)
                return value == null ? Visibility.Visible : Visibility.Collapsed;
            else
                return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
