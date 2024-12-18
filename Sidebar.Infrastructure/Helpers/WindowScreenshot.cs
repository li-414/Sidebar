using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sidebar.Infrastructure.Helpers
{
    public class WindowScreenshot
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static Bitmap CaptureWindow(IntPtr hWnd)
        {
            if (GetWindowRect(hWnd, out RECT rect))
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                if (width == 0 || height == 0)
                {
                    return null;
                }

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    IntPtr hdc = graphics.GetHdc();
                    bool success = PrintWindow(hWnd, hdc, 0);
                    graphics.ReleaseHdc(hdc);

                    if (!success)
                    {
                        bitmap.Dispose();
                        return null; // PrintWindow failed
                    }
                }
                return bitmap;
            }
            return null;
        }


        public static bool IsBitmapBlackOptimized(Bitmap bitmap, int skipEdgePixels = 100)
        {
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int byteCount = bmpData.Stride * bitmap.Height;
                byte[] pixels = new byte[byteCount];

                IntPtr ptr = bmpData.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, byteCount);

                int effectiveWidth = bitmap.Width - skipEdgePixels; // 忽略右边的像素
                int effectiveHeight = bitmap.Height - skipEdgePixels; // 忽略下边的像素

                for (int y = 0; y < effectiveHeight; y++)
                {
                    for (int x = 0; x < effectiveWidth; x++)
                    {
                        int index = y * bmpData.Stride + x * bytesPerPixel;

                        // 判断是否为非黑像素
                        if (pixels[index] != 0 || pixels[index + 1] != 0 || pixels[index + 2] != 0)
                        {
                            return false; // 找到非黑像素
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }

            return true; // 全黑
        }


        public static bool IsBitmapBlackOptimized(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int byteCount = bmpData.Stride * bitmap.Height;
                byte[] pixels = new byte[byteCount];

                IntPtr ptr = bmpData.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, byteCount);

                for (int i = 0; i < byteCount; i += bytesPerPixel)
                {
                    if (pixels[i] != 0 || pixels[i + 1] != 0 || pixels[i + 2] != 0)
                    {
                        return false; // 找到非黑像素
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }

            return true; // 全黑
        }


        public static bool IsBitmapMostlyBlack(Bitmap bitmap)
        {
            int blackPixelCount = 0;
            int totalPixelCount = bitmap.Width * bitmap.Height;

            for (int y = 0; y < bitmap.Height; y += 5) // 每隔5行取样
            {
                for (int x = 0; x < bitmap.Width; x += 5) // 每隔5列取样
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0)
                    {
                        blackPixelCount++;
                    }
                }
            }

            // 如果超过一定比例是黑像素，认为图像是黑的
            return blackPixelCount > (totalPixelCount * 0.95); // 95%黑像素
        }


        public static bool IsBitmapUniformOptimized(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int byteCount = bmpData.Stride * bitmap.Height;
                byte[] pixels = new byte[byteCount];

                IntPtr ptr = bmpData.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, byteCount);

                // 获取第一个像素的颜色
                byte baseBlue = pixels[0];
                byte baseGreen = pixels[1];
                byte baseRed = pixels[2];
                byte baseAlpha = bytesPerPixel == 4 ? pixels[3] : (byte)255;

                // 遍历所有像素
                for (int i = 0; i < byteCount; i += bytesPerPixel)
                {
                    byte blue = pixels[i];
                    byte green = pixels[i + 1];
                    byte red = pixels[i + 2];
                    byte alpha = bytesPerPixel == 4 ? pixels[i + 3] : (byte)255;

                    if (blue != baseBlue || green != baseGreen || red != baseRed || alpha != baseAlpha)
                    {
                        return false; // 发现不同像素
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }

            return true; // 所有像素相同
        }



        public static bool HasTwoOrFewerColorsOptimized(Bitmap bitmap)
        {
            HashSet<(byte R, byte G, byte B)> uniqueColors = new HashSet<(byte R, byte G, byte B)>();

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int byteCount = bmpData.Stride * bitmap.Height;
                byte[] pixels = new byte[byteCount];

                Marshal.Copy(bmpData.Scan0, pixels, 0, byteCount);

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        int index = y * bmpData.Stride + x * bytesPerPixel;
                        byte blue = pixels[index];
                        byte green = pixels[index + 1];
                        byte red = pixels[index + 2];

                        uniqueColors.Add((red, green, blue));

                        // 如果颜色种类超过两种，直接返回 false
                        if (uniqueColors.Count > 2)
                        {
                            return false;
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }

            return true; // 仅包含两种或更少颜色
        }

    }
}
