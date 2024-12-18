using System.Drawing;

namespace Sidebar.Infrastructure.Helpers
{
    public class ColorHelper
    {
        /// <summary>
        /// 获取指定区域的颜色
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public (byte r, byte g, byte b) GetDominantColorAsync(int x, int y, int width, int height)
        {
            // 截取屏幕指定区域
            using (Bitmap bitmap = new Bitmap(width, height))
            {

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(x, y, 0, 0, bitmap.Size);
                }


                // 提取主色调
                var colorData = new int[bitmap.Width * bitmap.Height];
                int index = 0;

                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        var pixelColor = bitmap.GetPixel(i, j);
                        colorData[index++] = pixelColor.ToArgb();
                    }
                }

                // 分析颜色，计算最常见的颜色
                var dominantColor = colorData
                    .GroupBy(c => c)
                    .OrderByDescending(g => g.Count())
                    .Select(g => System.Drawing.Color.FromArgb(g.Key))
                    .First();

                // 停止计时
                //stopwatch.Stop();

                // 记录执行时间
                //LogHelper.LogExecutionTime(stopwatch.ElapsedMilliseconds);

                // 转换为 WPF 的颜色类型
                return (dominantColor.R, dominantColor.G, dominantColor.B);
            }
        }

        //public static LinearGradientBrush CreateGradientBrush(
        //    System.Windows.Media.Color dominantColor
        //)
        //{
        //    // 创建明亮和暗色调
        //    var lightColor = System.Windows.Media.Color.FromArgb(
        //        dominantColor.A,
        //        (byte)Math.Min(dominantColor.R + 30, 255),
        //        (byte)Math.Min(dominantColor.G + 30, 255),
        //        (byte)Math.Min(dominantColor.B + 30, 255)
        //    );

        //    var darkColor = System.Windows.Media.Color.FromArgb(
        //        dominantColor.A,
        //        (byte)Math.Max(dominantColor.R - 30, 0),
        //        (byte)Math.Max(dominantColor.G - 30, 0),
        //        (byte)Math.Max(dominantColor.B - 30, 0)
        //    );

        //    // 创建线性渐变 Brush
        //    var gradientBrush = new LinearGradientBrush();
        //    gradientBrush.StartPoint = new System.Windows.Point(0, 1);
        //    gradientBrush.EndPoint = new System.Windows.Point(1, 1);
        //    gradientBrush.GradientStops.Add(new GradientStop(lightColor, 0.0));
        //    gradientBrush.GradientStops.Add(new GradientStop(dominantColor, 0.5));
        //    gradientBrush.GradientStops.Add(new GradientStop(darkColor, 1.0));

        //    return gradientBrush;
        //}
    }
}
