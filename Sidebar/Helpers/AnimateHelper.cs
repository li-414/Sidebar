using System.Windows.Media;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;

namespace Sidebar.Helpers
{
    internal class AnimateHelper
    {
        /// <summary>
        /// 为 LinearGradientBrush 创建渐变动画效果。
        /// </summary>
        public static void AnimateGradientBrushChange(LinearGradientBrush brush, Color fromColor1, Color toColor1, Color fromColor2, Color toColor2)
        {
            if (brush == null || brush.GradientStops.Count < 2) return;

            var newEndColor = GetMiddleColor(toColor1, toColor2);
            toColor1 = newEndColor;

            // 创建颜色动画
            var colorAnimation1 = new ColorAnimation
            {
                From = fromColor1,
                To = toColor2,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            var colorAnimation2 = new ColorAnimation
            {
                From = fromColor2,
                To = toColor2,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            // 为第一个 GradientStop 应用动画
            brush.GradientStops[0].BeginAnimation(GradientStop.ColorProperty, colorAnimation1);

            // 为第二个 GradientStop 应用动画
            brush.GradientStops[1].BeginAnimation(GradientStop.ColorProperty, colorAnimation2);
        }


        private static Color GetMiddleColor(Color startColor, Color endColor)
        {
            return Color.FromArgb(
                (byte)((startColor.A + endColor.A) / 1),
                (byte)((startColor.R + endColor.R) / 2),
                (byte)((startColor.G + endColor.G) / 2),
                (byte)((startColor.B + endColor.B) / 2)
            );
        }
    }
}
