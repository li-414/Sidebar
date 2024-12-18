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
using ListBox = System.Windows.Controls.ListBox;

namespace Sidebar.Views
{
    /// <summary>
    /// ProcessPage.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessPage : Page
    {
        public ProcessPage()
        {
            InitializeComponent();
        }


        private void ListBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.DataContext is ProcessPageViewModel model)
            {
                model.IsRefreshProcess = false;
            }
        }

        private void ListBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.DataContext is ProcessPageViewModel model && sender is ListBox listBox && !listBox.IsMouseOver)
            {
                model.IsRefreshProcess = true;
            }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is ProcessPageViewModel model)
            {
                // 获取当前选中的项
                var listBox = sender as ListBox;
                if (listBox != null)
                {
                    var selectedItem = listBox.SelectedItem as ProcessInfo;
                    if (selectedItem != null)
                    {
                        model.SelectItemMouseDoubleClick(selectedItem);
                        // 执行双击后的操作
                        //MessageBox.Show($"双击了进程: {selectedItem.Title}");
                    }
                }
            }
               
        }
    }
}
