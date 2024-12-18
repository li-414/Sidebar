using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sidebar.Helpers
{
    public static class SendKeys
    {
        const byte VK_F = 0x46; // 'f' 键的虚拟键码
        const byte VK_ENTER = 0x0D; // 'Enter' 键的虚拟键码
        const byte VK_F11 = 0x7A; // F11的虚拟键码

        // 定义常量
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;   // 按下鼠标左键
        const uint MOUSEEVENTF_LEFTUP = 0x0004;     // 松开鼠标左键
        const uint MOUSEEVENTF_ABSOLUTE = 0x8000;   // 指定位置

        // 引入 GetCursorPos API
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        // 引入 SetCursorPos API
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int x, int y);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInfo);



        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);


        public struct POINT
        {
            public int X;
            public int Y;
        }


        public static void Send(string key)
        {
            foreach (char c in key)
            {
                keybd_event((byte)c, 0, 0, 0);
                keybd_event((byte)c, 0, 2, 0);
            }
        }


        public static void PressF11()
        {
            // 模拟按下F11
            keybd_event(VK_F11, 0, 0, 0);

            // 模拟松开F11
            keybd_event(VK_F11, 0, 2, 0);
        }

        public static void PressF()
        {
            // 模拟按下F11
            keybd_event(VK_F, 0, 0, 0);

            // 模拟松开F11
            keybd_event(VK_F, 0, 2, 0);
        }

        public static void PressEnter()
        {
            // 模拟按下F11
            keybd_event(VK_ENTER, 0, 0, 0);

            // 模拟松开F11
            keybd_event(VK_ENTER, 0, 2, 0);
        }


        public static void DoubleClickMouse(int showX = 0, int showY = 0)
        {
            // 保存当前鼠标位置
            GetCursorPos(out POINT currentPos);
            int originalX = currentPos.X;
            int originalY = currentPos.Y;

            uint screenWidth = 3840;   // 例如主显示器和副显示器总宽度
            uint screenHeight = 1080;  // 例如主显示器和副显示器总高度

            uint absX = (((uint)showX * 65535) / screenWidth);
            uint absY = ((uint)showY * 65535) / screenHeight;

            // 使用 SetCursorPos 显式设置鼠标位置
            SetCursorPos(showX, showY);

            // 模拟鼠标左键按下和松开
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, absX, absY, 0, 0); // 按下左键
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP, absX, absY, 0, 0);   // 松开左键

            // 等待一下，模拟双击的间隔
            //System.Threading.Thread.Sleep(100); // 延迟100毫秒

            // 模拟第二次鼠标左键按下和松开
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, absX, absY, 0, 0); // 按下左键
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP, absX, absY, 0, 0);   // 松开左键

            // 恢复鼠标到原位置
            SetCursorPos(originalX, originalY);
        }
    }
}
