using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using ColorMine.ColorSpaces;

namespace SixR_20.Convertors
{
    class ColorOpacityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float opacity = 1;
            if (!(float.TryParse(parameter.ToString(), out opacity)) || opacity>1 || opacity<0)
                throw new Exception("Invalid Opacity Value.");
            Color color = ((SolidColorBrush)value).Color;
            color.A = (byte)(opacity*255);
            return color; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
