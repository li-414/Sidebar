using Sidebar.Domain.Models;
using Sidebar.ViewModels;
using System;
using System.Collections.Generic;
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
    /// DesktopPage.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopPage : Page
    {
        public DesktopPage()
        {
            InitializeComponent();
        }


        //桌面快捷键点击
        private void ListBox_DsekTopSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (
                sender is System.Windows.Controls.ListBox listBox
                && listBox.DataContext is DesktopPageViewModel model
                && e.AddedItems.Count > 0
                && e.AddedItems[0] is DesktopIcon desktop
            )
            {
                model.DockerTopCLick(desktop);
                listBox.SelectedIndex = -1;
            }
        }
    }
}
