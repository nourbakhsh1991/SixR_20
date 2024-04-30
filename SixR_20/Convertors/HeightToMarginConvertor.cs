using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SixR_20.Convertors
{
    class HeightToMarginConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is double))
                return null;
            var height = (double)values[0];
            var thickness = (Thickness)values[1];
            return new Thickness(thickness.Left, thickness.Top + 10, thickness.Right, thickness.Bottom + 10);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
