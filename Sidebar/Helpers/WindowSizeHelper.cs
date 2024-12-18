using System.Windows;

namespace Sidebar.Helpers
{
    class WindowSizeHelper
    {
        /// <summary>
        /// 位置改正
        /// </summary>
        /// <param name="rect">正确位置</param>
        /// <param name="rect1">错误位置</param>
        /// <returns></returns>
        public static Rect PointError(Rect rect, Rect rect1)
        {
            if (rect1.Left < rect.Left)
            {
                rect1 = new Rect(rect.Left, rect1.Top, rect1.Width, rect1.Height);
            }
            if (rect1.Top < rect.Top)
            {
                rect1 = new Rect(rect1.Left, rect.Top, rect1.Width, rect1.Height);
            }


            return rect1;
        }
    }
}
