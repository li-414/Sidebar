using Sidebar.Domain.Models;
using Sidebar.Helpers;
using Sidebar.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Image = System.Windows.Controls.Image;
using ListBox = System.Windows.Controls.ListBox;

namespace Sidebar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IconHelper iconHelper = new IconHelper();

        public MainWindow()
        {
            this.WindowState = WindowState.Normal;

            InitializeComponent();

            this.Icon = iconHelper.IconToImageSource(iconHelper.GetEmbeddedIcon("Sidebar.Assets.app.ico"));


            Rect workArea = SystemParameters.WorkArea;
            this.Width = workArea.Width;
            this.Height = workArea.Height;
            this.Left = 0;
            this.Top = 0;
        }


       


        protected override void OnClosed(EventArgs e)
        {
            iconHelper.Close();
            base.OnClosed(e);
        }

       
        private void DesktopBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DesktopPop.IsOpen = !this.DesktopPop.IsOpen;
        }
    }
}
