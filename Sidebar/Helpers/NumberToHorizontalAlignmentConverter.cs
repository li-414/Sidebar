using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sidebar.Helpers
{
    public class NumberToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int alignmentValue)
            {
                return alignmentValue switch
                {
                    0 => System.Windows.HorizontalAlignment.Left,
                    1 => System.Windows.HorizontalAlignment.Center,
                    2 => System.Windows.HorizontalAlignment.Right,
                    _ => System.Windows.HorizontalAlignment.Stretch,
                };
            }
            return HorizontalAlignment.Center; // 默认值
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HorizontalAlignment alignment)
            {
                return alignment switch
                {
                    HorizontalAlignment.Left => 0,
                    HorizontalAlignment.Center => 1,
                    HorizontalAlignment.Right => 2,
                    _ => -1, // 默认值
                };
            }
            return -1; // 默认值
        }
    }
}
