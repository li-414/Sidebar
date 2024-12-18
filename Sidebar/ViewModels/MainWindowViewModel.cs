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
using Sidebar.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Sidebar.ViewModels
{
    public partial class MainWindowViewModel : ObservableRecipient, IRecipient<MoveRecipient>
    {
        private string addressUrl = "http://101.43.204.170:8083/Address";


        [ObservableProperty]
        private Uri? navigationUrl = new Uri("/Sidebar;component/Views/ProcessPage.xaml", UriKind.Relative);


        [ObservableProperty]
        private double embedAreaLeft;

        [ObservableProperty]
        private double embedAreaTop;

        [ObservableProperty]
        private double embedAreaWidth;

        [ObservableProperty]
        private double embedAreaHeight;

        [ObservableProperty]
        private double embedAreaNormalLeft;

        [ObservableProperty]
        private double embedAreaNormalTop;

        [ObservableProperty]
        private double embedAreaNormalWidth;

        [ObservableProperty]
        private double embedAreaNormalHeight;

        [ObservableProperty]
        private string navigationSoure = "about:blank";

        [ObservableProperty]
        private Visibility webVisibility = Visibility.Collapsed;


        [ObservableProperty]
        private LinearGradientBrush sidebarBackground = new LinearGradientBrush(
            Colors.White,
            System.Windows.Media.Colors.Black,
            new System.Windows.Point(0, 0.5),
            new System.Windows.Point(1, 0.5)
        );

        [ObservableProperty]
        private System.Windows.Media.Brush foregroundBrush = System.Windows.Media.Brushes.White;


        public Action<Rect> UpdateEmbedAreaPositionAction { get; }
        public Action<Rect> UpdateEmbedAreaNormalPositionAction { get; }


        private readonly IColorServer colorServer;

        public MainWindowViewModel()
        {
            this.IsActive = true;


            UpdateEmbedAreaPositionAction = rect => UpdateEmbedAreaPosition(rect);
            UpdateEmbedAreaNormalPositionAction = rect => UpdateEmbedAreaNormalPosition(rect);

            this.colorServer = App.ServiceProvider!.GetRequiredService<IColorServer>();

            DispatcherTimer timerColor = new DispatcherTimer();
            timerColor.Interval = TimeSpan.FromSeconds(10);
            timerColor.Tick += (s, e) => LoadColorWindows();
            timerColor.Start();
            LoadColorWindows();
        }


        [RelayCommand]
        private void NavigationAddress(string tag)
        {

            switch (tag)
            {
                case "1":
                    NavigationSoure = "about:blank";
                    WebVisibility = Visibility.Collapsed;
                    break;
                case "2":
                    NavigationSoure = "http://101.43.204.170:8083/Address";
                    WebVisibility = Visibility.Visible;
                    break;
                case "3":
                    NavigationSoure = "http://101.43.204.170:8082/Addresss";
                    WebVisibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 颜色刷新
        /// </summary>
        private void LoadColorWindows()
        {
            //颜色刷新
            App.Current.Dispatcher.Invoke(async () =>
            {
                Rect workArea = SystemParameters.WorkArea;
                (byte r, byte g, byte b) = await colorServer.ColorRefreshAsync(workArea.X, workArea.Y, workArea.Width, workArea.Height);
                var areaColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b));
                if (areaColor is SolidColorBrush solidColorBrush)
                {
                    double brightness = (0.299 * solidColorBrush.Color.R) + (0.587 * solidColorBrush.Color.G) + (0.114 * solidColorBrush.Color.B);
                    ForegroundBrush = brightness > 128 ? System.Windows.Media.Brushes.Black : System.Windows.Media.Brushes.White;


                    if (ForegroundBrush is SolidColorBrush colorBrush)
                    {
                        AnimateHelper.AnimateGradientBrushChange(SidebarBackground,
                                                                 SidebarBackground.GradientStops[0].Color,
                                                                 colorBrush.Color,
                                                                 SidebarBackground.GradientStops[1].Color,
                                                                 System.Windows.Media.Color.FromRgb(r, g, b));


                        WeakReferenceMessenger.Default.Send(new ColorRecipient(ForegroundBrush, SidebarBackground));
                    }
                }
            });
        }


        private void UpdateEmbedAreaNormalPosition(Rect rect)
        {
            (EmbedAreaNormalLeft, EmbedAreaNormalTop, EmbedAreaNormalWidth, EmbedAreaNormalHeight) =
                (rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void UpdateEmbedAreaPosition(Rect rect)
        {
            (EmbedAreaLeft, EmbedAreaTop, EmbedAreaWidth, EmbedAreaHeight) = (
                rect.Left,
                rect.Top,
                rect.Width,
                rect.Height
            );
        }

        public void Receive(MoveRecipient message)
        {
            switch (message.MoveEnum)
            {
                case Domain.Enums.MoveEnum.Error:
                    //更改错误位置不让它影响操作
                    var (errorLeft, errorTop, errorWidth, errorHeight) = WindowHelper.GetForegroundWindowSize(message.handle);
                    Rect changeRect = WindowSizeHelper.PointError(new System.Windows.Rect(EmbedAreaLeft, EmbedAreaTop, EmbedAreaWidth, EmbedAreaHeight),
                        new System.Windows.Rect(errorLeft, errorTop, errorWidth, errorHeight)
                    );
                    WindowHelper.MoveWindow(message.handle, (int)changeRect.Left, (int)changeRect.Top, (int)changeRect.Width, (int)changeRect.Height, true);
                    break;
                case Domain.Enums.MoveEnum.Normal:
                    WindowHelper.MoveWindow(message.handle, (int)EmbedAreaNormalLeft, (int)EmbedAreaNormalTop, (int)EmbedAreaNormalWidth, (int)EmbedAreaNormalHeight, true);
                    break;
                case Domain.Enums.MoveEnum.Max:
                    WindowHelper.MoveWindow(message.handle, (int)EmbedAreaLeft, (int)EmbedAreaTop, (int)EmbedAreaWidth, (int)EmbedAreaHeight, true);
                    break;
                default:
                    break;
            }
        }

    }
}
