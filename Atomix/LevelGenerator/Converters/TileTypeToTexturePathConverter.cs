using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Kinectomix.LevelGenerator
{
    public class TileTypeToTexturePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object texturesPathParameter, CultureInfo culture)
        {
            if (!(texturesPathParameter is string))
                return null;

            return string.Format("{0}{1}.png", (string)texturesPathParameter, value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
