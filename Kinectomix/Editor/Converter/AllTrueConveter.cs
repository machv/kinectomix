using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Mach.Kinectomix.LevelEditor.Converter
{
    public class AllTrueConveter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = values.Length > 0 && values.All(value => (bool)value);
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
