using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Sidebar.Application.Servers;
using Sidebar.Domain.Models;
using Sidebar.Domain.Recipients;
using Sidebar.Helpers;
using Sidebar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Sidebar.ViewModels
{

    internal partial class ProcessPageViewModel : ObservableRecipient, 
        IRecipient<CommandRecipient>, 
        IRecipient<ColorRecipient>, 
        IRecipient<KeyRecipient>
    {
        [ObservableProperty]
        private System.Windows.Media.Brush foregroundBrush = System.Windows.Media.Brushes.White;

        [ObservableProperty]
        private LinearGradientBrush sidebarBackground = new LinearGradientBrush(
            Colors.White,
            System.Windows.Media.Colors.Black,
            new System.Windows.Point(0, 0.5),
            new System.Windows.Point(1, 0.5)
        );

        [ObservableProperty]
        private bool isDrawBitmap = true;

        public bool IsRefreshProcess = true;
        private readonly ILogger logger;
        private readonly IProcessServer processServer;

        [ObservableProperty]
        private ObservableCollection<ProcessInfo> openWindows =
           new ObservableCollection<ProcessInfo>();

        public ProcessPageViewModel()
        {
            this.IsActive = true;

            this.IsDrawBitmap = true;

            this.logger = App.ServiceProvider!.GetRequiredService<ILogger>(); ;
            this.processServer = App.ServiceProvider!.GetRequiredService<IProcessServer>();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => LoadOpenWindows();
            timer.Start();
            LoadOpenWindows();
        }

        /// <summary>
        /// 刷新线程
        /// </summary>
        /// <param name="isImageRefresh">是只刷新图片还是整个刷新</param>
        private void LoadOpenWindows(bool isImageRefresh = false)
        {
            //线程刷新
            if (IsRefreshProcess)
            {
                List<ProcessInfo>? processInfos = processServer?.RefreshProcess(IsDrawBitmap);
                if (processInfos != null)
                {
                    if (isImageRefresh == false)
                    {
                        var bitmapList = OpenWindows?.Where(x => x.Bitmap != null).ToList();
                        OpenWindows?.Clear();
                        foreach (var item in processInfos)
                        {
                            var temp = bitmapList?.FirstOrDefault(x => x.Handle == item.Handle && x.Handle != WindowHelper.GetForegroundWindow());
                            if (temp != null)
                            {
                                item.Bitmap = temp.Bitmap;
                            }
                            OpenWindows?.Add(item);
                        }
                    }
                    else
                    {
                        var bitmapList = OpenWindows.ToList();
                        OpenWindows?.Clear();
                        foreach (var item in bitmapList)
                        {
                            var temp = processInfos?.FirstOrDefault(x => x.Handle == item.Handle);
                            if (temp != null && item.Bitmap is null)
                            {
                                item.Bitmap = temp.Bitmap;
                            }
                            OpenWindows?.Add(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将窗口移位最前
        /// </summary>
        /// <param name="windowInfo"></param>
        [RelayCommand]
        private void ProcessSelectionChanged(ProcessInfo windowInfo)
        {
            if (windowInfo == null)
                return;
            // 显示并切换到选中的窗口
            WindowHelper.ShowRestoreWindow(windowInfo.Handle);
            WindowHelper.SetForegroundWindow(windowInfo.Handle);

            WeakReferenceMessenger.Default.Send(new MoveRecipient(windowInfo.Handle, Domain.Enums.MoveEnum.Error));

            if (windowInfo.Bitmap != null) return;
            IsRefreshProcess = true;
            LoadOpenWindows(true); //只刷新图片
            IsRefreshProcess = false;
        }


        [RelayCommand]
        private void HiddenWindow(nint handle)
        {
            WindowHelper.ShowMinimizedWindow(handle);
        }

        [RelayCommand]
        private void NormalWindow(nint handle)
        {
            ProcessInfo? windowInfo = OpenWindows.FirstOrDefault(x => x.Handle == handle);
            if (windowInfo == null)
                return;

            //WindowHelper.ExitFullScreenWindow(windowInfo.Handle);
            ProcessSelectionChangedCommand.Execute(windowInfo);

            WeakReferenceMessenger.Default.Send(new MoveRecipient(handle, Domain.Enums.MoveEnum.Normal));
        }

        [RelayCommand]
        private void MaxWindow(nint handle)
        {
            ProcessInfo? windowInfo = OpenWindows.FirstOrDefault(x => x.Handle == handle);
            if (windowInfo == null)
                return;

            //WindowHelper.ExitFullScreenWindow(windowInfo.Handle);
            ProcessSelectionChangedCommand.Execute(windowInfo);

            WeakReferenceMessenger.Default.Send(new MoveRecipient(handle, Domain.Enums.MoveEnum.Max));
        }

        [RelayCommand]
        private void SecondScreenLeft(nint handle)
        {
            ProcessInfo? windowInfo = OpenWindows.FirstOrDefault(x => x.Handle == handle);
            if (windowInfo == null)
                return;

            ProcessSelectionChangedCommand.Execute(windowInfo);

            new ScreenHelper().ScreenShowLeft(handle);
        }

        [RelayCommand]
        private void SecondScreenRight(nint handle)
        {

            ProcessInfo? windowInfo = OpenWindows.FirstOrDefault(x => x.Handle == handle);
            if (windowInfo == null)
                return;

            ProcessSelectionChangedCommand.Execute(windowInfo);

            new ScreenHelper().ScreenShowRight(handle);
        }

        [RelayCommand]
        private void SecondScreen(nint handle)
        {
            ProcessInfo? windowInfo = OpenWindows.FirstOrDefault(x => x.Handle == handle);
            if (windowInfo == null)
                return;
            ProcessSelectionChangedCommand.Execute(windowInfo);

            new ScreenHelper().ScreenShow(handle);

            //Helpers.SendKeys.PressF();
            //Helpers.SendKeys.PressEnter();
            //Helpers.SendKeys.PressF11();


        }

        [RelayCommand]
        private void CloseWindow(nint handle)
        {
            processServer.CloseProcess(handle);

            var pro = OpenWindows.FirstOrDefault(x => x.Handle == handle);
            if (pro != null)
            {
                OpenWindows.Remove(pro);
            }
        }

        public void Receive(CommandRecipient message)
        {
            switch (message.CommandEnum)
            {
                case Domain.Enums.CommandEnum.ProcessSelectionChanged:
                    ProcessInfo? windowInfo = OpenWindows.FirstOrDefault(x => x.Title == message.Title);
                    ProcessSelectionChangedCommand.Execute(windowInfo);
                    break;
                default:
                    break;
            }
        }

        public void Receive(ColorRecipient message)
        {
            ForegroundBrush = message.ForegroundBrush;
            SidebarBackground = message.SidebarBackground;
        }

        public void Receive(KeyRecipient message)
        {
            switch (message.KeyEnum)
            {
                case Domain.Enums.KeyEnum.F:
                    Helpers.SendKeys.PressF();
                    break;
                case Domain.Enums.KeyEnum.Enter:
                    Helpers.SendKeys.PressEnter();
                    break;
                case Domain.Enums.KeyEnum.F11:
                    Helpers.SendKeys.PressF11();
                    break;
                default:
                    break;
            }
        }

        internal void SelectItemMouseDoubleClick(ProcessInfo selectedItem)
        {
            var (errorLeft, errorTop, errorWidth, errorHeight) = WindowHelper.GetForegroundWindowSize(selectedItem.Handle);

            var showX = errorLeft + errorWidth / 2;
            var showY = errorTop + errorHeight / 2;

            Thread.Sleep(500);

            Helpers.SendKeys.DoubleClickMouse((int)showX, (int)showY);
        }
    }
}
