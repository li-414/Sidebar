using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sidebar.Views
{
    /// <summary>
    /// AddressControl.xaml 的交互逻辑
    /// </summary>
    public partial class AddressControl : System.Windows.Controls.UserControl
    {
        public AddressControl()
        {
            InitializeComponent();
        }

        WebView2 myWebView2;
        private async void WebView2_Initialized(object sender, EventArgs e)
        {
            if (sender is WebView2 webView2)
            {
                await webView2.EnsureCoreWebView2Async(null);

                //webView2.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                webView2.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

                 myWebView2 = webView2;
            }
        }

        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            // 获取目标 URL
            string url = e.Uri;

            // 阻止 WebView2 打开新窗口
            e.Handled = true;

            // 使用默认浏览器打开链接
            OpenInDefaultBrowser(url);
        }

        private void OpenInDefaultBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // 使用系统默认浏览器
                });
            }
            catch
            {
                // 错误处理：例如记录日志或通知用户
            }
        }
    }
}
