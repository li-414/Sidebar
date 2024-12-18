using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Sidebar.Helpers
{
    public class ConverterHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double originalSize && double.TryParse(parameter?.ToString(), out double scale))
            {
                return originalSize * scale;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class BorderPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FrameworkElement element && element.Parent is Window window)
            {
                // 获取 Border 相对于窗口的位置
                var borderRect = element.TransformToAncestor(window).TransformBounds(new Rect(element.RenderSize));
                return borderRect; // 返回 Rect，可以根据需要返回位置或大小信息
            }
            return Rect.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
