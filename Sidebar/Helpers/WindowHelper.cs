using System.Runtime.InteropServices;
using System.Text;

namespace Sidebar.Helpers
{
    public class WindowHelper
    {
        

        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static string GetWindowTitle(nint hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// 获取窗口大小
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static (int Width, int Height) GetWindowSize(nint hWnd)
        {
            if (GetWindowRect(hWnd, out RECT rect))
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                return (width, height);
            }

            return (0, 0);
        }

        /// <summary>
        /// 获取最前方的窗口大小
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static (double Left, double Top, double Width, double Height) GetForegroundWindowSize(nint hWnd)
        {
            if (GetWindowRect(hWnd, out RECT rect))
            {
                double left = rect.Left;
                double top = rect.Top;
                double width = rect.Right - rect.Left;
                double height = rect.Bottom - rect.Top;
                return (left, top, width, height);
            }

            return (0, 0, 10, 10);
        }

        /// <summary>
        /// 恢复窗口大小
        /// </summary>
        /// <param name="hWnd"></param>
        public static void ShowRestoreWindow(nint hWnd)
        {
            ShowWindow(hWnd, SW_RESTORE);
        }

        /// <summary>
        /// 最大化窗口
        /// </summary>
        /// <param name="hWnd"></param>
        public static void ShowMaximizedWindow(nint hWnd)
        {
            ShowWindow(hWnd, SW_SHOWMAXIMIZED);
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="hWnd"></param>
        public static void ShowMinimizedWindow(nint hWnd)
        {
            ShowWindow(hWnd, SW_SHOWMINIMIZED);
        }

        /// <summary>
        /// 正常显示的窗口
        /// </summary>
        /// <param name="hWnd"></param>
        public static void ShowNormalWindow(nint hWnd)
        {
            ShowWindow(hWnd, SW_SHOWNORMAL);
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        /// <param name="hWnd"></param>
        public static void HideWindow(nint hWnd)
        {
            ShowWindow(hWnd, SW_HIDE);
        }

        /// <summary>
        /// 获取窗口样式
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static int GetWindowStyle(nint hWnd)
        {
            return GetWindowLong(hWnd, GWL_STYLE);
        }

        /// <summary>
        /// 无边框样式窗口
        /// </summary>
        /// <param name="handle"></param>
        internal static void SetWindowNoStyle(nint handle)
        {
            int style = WindowHelper.GetWindowStyle(handle);
            SetWindowLong(handle, GWL_STYLE, (int)(style | WS_POPUP | WS_VISIBLE));
        }
        internal static void ExitFullScreenWindow(nint hWnd)
        {
            const uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
            int style = GetWindowLong(hWnd, GWL_STYLE);
            SetWindowLong(hWnd, GWL_STYLE, (int)(style | WS_OVERLAPPEDWINDOW));
        }

        /// <summary>
        /// 窗口是否是最大化
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static bool IsWindowMaximized(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.Length = Marshal.SizeOf(placement);

            if (GetWindowPlacement(hWnd, ref placement))
            {
                return placement.ShowCmd == SW_SHOWMAXIMIZED;
            }

            return false;
        }




        // 枚举窗口的委托
        public delegate bool EnumWindowsProc(nint hWnd, nint lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, nint lParam);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(nint hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);



        [DllImport("user32.dll")]
        private static extern bool ShowWindow(nint hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern nint GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(nint hWnd, out uint processId);



        /// <summary>
        /// 获取窗口样式
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// 获取窗口大小
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

        /// <summary>
        /// 将窗口切换到前台。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(nint hWnd);
        /// <summary>
        /// 将窗口置于最前方，确保窗口不被其他窗口覆盖
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(nint hWnd);

        /// <summary>
        /// 调整窗口位置
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="bRepaint"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(nint hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// 窗口层级
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);


        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);



        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int Length;
            public int Flags;
            public int ShowCmd;
            public Point MinPosition;
            public Point MaxPosition;
            public RECT NormalPosition;
        }

        public static readonly nint HWND_BOTTOM = new nint(1);

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_SHOWWINDOW = 0x0040;

        private const int SW_HIDE = 0;          // 隐藏窗口
        private const int SW_SHOWNORMAL = 1;    // 正常显示窗口
        private const int SW_SHOWMINIMIZED = 2; // 最小化窗口
        private const int SW_SHOWMAXIMIZED = 3; // 最大化窗口        
        private const int SW_RESTORE = 9;// 恢复窗口大小

        private const int GWL_STYLE = -16; //窗口样式
        private const uint WS_POPUP = 0x80000000;
        private const uint WS_VISIBLE = 0x10000000;
    }

}
