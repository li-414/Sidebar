using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Sidebar.Helpers
{
    class IconHelper
    {
        private NotifyIcon _notifyIcon;

        public IconHelper()
        {
            // 初始化 NotifyIcon
            _notifyIcon = new NotifyIcon
            {
                BalloonTipText = "应用程序已最小化到托盘。",
                Text = "Sidebar",
                Icon = GetEmbeddedIcon("Sidebar.Assets.app.ico"),
                Visible = true,
            };

            // 托盘图标双击事件
            _notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            // 托盘右键菜单
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("显示主窗口", null, (s, e) => ShowMainWindow());
            contextMenu.Items.Add("最小化程序", null, (s, e) => MinApplication());
            contextMenu.Items.Add("退出程序", null, (s, e) => ExitApplication());
            _notifyIcon.ContextMenuStrip = contextMenu;
        }


        public Icon? GetEmbeddedIcon(string? resourceName)
        {
            try
            {
                if (resourceName == null) { return null; }
                // 获取当前程序集
                var assembly = Assembly.GetExecutingAssembly();

                // 获取嵌入资源的流
                using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        // 从流中创建并返回图标
                        return new Icon(stream);
                    }
                    else
                    {
                        // 如果资源未找到
                        System.Windows.Forms.MessageBox.Show($"无法找到资源: {resourceName}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"加载图标时发生错误: {ex.Message}");
                return null;
            }
        }

        public ImageSource? IconToImageSource(Icon? icon)
        {
            if (icon == null) return null;

            using (var bitmap = icon.ToBitmap())
            {
                var stream = new MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);

                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze(); // 线程安全

                return image;
            }
        }


        public void Close()
        {
            _notifyIcon.Dispose();
        }

        private void MinApplication()
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void ExitApplication()
        {
            _notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        private void ShowMainWindow()
        {
            App.Current.MainWindow.Show();
            App.Current.MainWindow.WindowState = WindowState.Normal;
            App.Current.MainWindow.Activate();
        }

        private void NotifyIcon_MouseDoubleClick(
            object? sender,
            System.Windows.Forms.MouseEventArgs e
        )
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowMainWindow();
            }
        }
    }
}
