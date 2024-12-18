using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Sidebar.Application.Servers;
using Sidebar.Domain.Models;
using Sidebar.Domain.Recipients;
using Sidebar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace Sidebar.ViewModels
{
    internal partial class DesktopPageViewModel : ObservableRecipient, IRecipient<CommandRecipient>, IRecipient<ColorRecipient>
    {
        [ObservableProperty]
        private System.Windows.Media.Brush foregroundBrush = System.Windows.Media.Brushes.White;

        private readonly ILogger logger;
        private readonly IDesktopLinkServer desktopLinkServer;

        [ObservableProperty]
        private ObservableCollection<DesktopIcon> desktopLinks =
           new ObservableCollection<DesktopIcon>();

        public DesktopPageViewModel()
        {
            this.IsActive = true;

            try
            {
                this.logger = App.ServiceProvider!.GetRequiredService<ILogger>(); ;
                this.desktopLinkServer = App.ServiceProvider!.GetRequiredService<IDesktopLinkServer>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += (s, e) => LoadOpenWindows();
            timer.Start();

            try
            {
                LoadOpenWindows();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 桌面快捷键和线程
        /// </summary>
        private void LoadOpenWindows()
        {
            //桌面刷新
            List<DesktopIcon>? desktopIcons = desktopLinkServer?.RefreshDesktopLink();
            desktopIcons = desktopIcons?.OrderBy(x => x.Title).ToList();
            DesktopLinks.Clear();
            desktopIcons?.ForEach(DesktopLinks.Add);
        }


        /// <summary>
        /// 启动软件
        /// </summary>
        /// <param name="desktopIcon"></param>
        [RelayCommand]
        private void DeskTopLinkSelectionChanged(DesktopIcon desktopIcon)
        {
            if (desktopIcon == null)
                return;

            //ProcessInfo? windowInfo = OpenWindows.FirstOrDefault(x => x.Title == desktopIcon.Title);
            //if (windowInfo == null)
            {
                try
                {
                    var process = Process.Start(
                        new ProcessStartInfo(desktopIcon.Path) { UseShellExecute = true }
                    );
                    if (process == null)
                        return;
                    process.WaitForInputIdle();

                    WeakReferenceMessenger.Default.Send(new CommandRecipient(desktopIcon.Title, Domain.Enums.CommandEnum.ProcessSelectionChanged));
                }
                catch (Exception ex)
                {
                    logger.Error($"启动软件错误：{ex.Message}");
                    //System.Windows.MessageBox.Show($"启动软件错误：{ex.Message}");
                }
            }
        }

        internal void DockerTopCLick(DesktopIcon desktop)
        {
            DesktopIcon? desktopIcon = DesktopLinks.FirstOrDefault(x => x.Title == desktop.Title);
            if (desktopIcon == null) return;
            DeskTopLinkSelectionChangedCommand.Execute(desktopIcon);

            //WeakReferenceMessenger.Default.Send(new CommandRecipient(desktop.Title, Domain.Enums.CommandEnum.DeskTopLinkSelectionChanged));
        }


        public void Receive(CommandRecipient message)
        {
            switch (message.CommandEnum)
            {
                case Domain.Enums.CommandEnum.ProcessSelectionChanged:
                    break;
                case Domain.Enums.CommandEnum.DeskTopLinkSelectionChanged:
                    DesktopIcon? desktopIcon = DesktopLinks.FirstOrDefault(x => x.Title == message.Title);
                    if (desktopIcon == null) return;
                    DeskTopLinkSelectionChangedCommand.Execute(desktopIcon);
                    break;
                default:
                    break;
            }

        }

        public void Receive(ColorRecipient message)
        {
            ForegroundBrush = message.ForegroundBrush;
        }
    }
}
