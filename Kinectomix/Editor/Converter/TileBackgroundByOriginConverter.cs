//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Data;

//namespace Mach.Kinectomix.LevelEditor.Converter
//{
//    class TileBackgroundByOriginConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (negate)
//                return value == null ? Visibility.Visible : Visibility.Collapsed;
//            else
//                return value != null ? Visibility.Visible : Visibility.Collapsed;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            return value;
//        }
//    }
//}
