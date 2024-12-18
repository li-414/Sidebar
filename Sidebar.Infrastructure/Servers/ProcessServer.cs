using Serilog;
using Sidebar.Application.Servers;
using Sidebar.Domain.Helpers;
using Sidebar.Domain.Models;
using Sidebar.Infrastructure.Helpers;
using System.Diagnostics;
using System.Drawing;

namespace Sidebar.Infrastructure.Servers
{
    public class ProcessServer : IProcessServer
    {
        public bool IsRefreshProcess = true;

        private readonly ILogger logger;

        public ProcessServer(ILogger logger)
        {
            this.logger = logger;
        }

        public void Close()
        {
            // 当前程序的进程名（不包含扩展名）
            string currentProcessName = Process.GetCurrentProcess().ProcessName;

            // 获取所有正在运行的同名进程
            var runningProcesses = Process.GetProcessesByName(currentProcessName)
                                          .Where(p => p.Id != Process.GetCurrentProcess().Id);

            foreach (var process in runningProcesses)
            {
                try
                {
                    // 关闭找到的进程
                    process.Kill();
                    process.WaitForExit(); // 等待进程退出
                }
                catch (Exception ex)
                {
                    logger.Error($"关闭线程错误：{ex.Message}");
                }
            }
        }


        public void CloseProcess(nint runProcessId)
        {
            try
            {
                WindowHelper.GetWindowThreadProcessId(runProcessId, out uint processId);
                if (processId == 0)
                {
                    logger.Error($"无法获取进程 ID。", "错误");
                    return;
                }

                // 根据 ProcessId 查找并关闭进程
                var process = Process.GetProcessById((int)processId);
                process.Kill();
                //MessageBox.Show($"进程 {processInfo.Title} 已关闭。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                logger.Error($"关闭进程失败：{ex.Message}", "错误");
            }
        }


        public List<ProcessInfo> RefreshProcess(bool isDrawBitmap)
        {
            List<ProcessInfo> OpenWindows = new List<ProcessInfo>();

            try
            {
                WindowHelper.EnumWindows((hWnd, lParam) =>
                {
                    if (WindowHelper.IsWindowVisible(hWnd))
                    {
                        string title = WindowHelper.GetWindowTitle(hWnd);
                        Bitmap bitmap = null;
                        //if (bitmap == null || WindowScreenshot.IsBitmapBlackOptimized(bitmap))
                        if (WindowHelper.GetForegroundWindow() == hWnd && isDrawBitmap == true)
                            bitmap = ScreenshotHelper.CaptureWindow(hWnd);

                        if (!string.IsNullOrWhiteSpace(title))
                        {
                            if (!SidebarExcludeHelper.Exclude(title))
                            {
                                int BitmapPosite = 1;
                                (double Left, double Top, double Width, double Height) = WindowHelper.GetForegroundWindowSize(hWnd);
                                if (Math.Abs(Left - 5 - 1920) < 1) BitmapPosite = 0;
                                else if (Math.Abs(Left - (1920 / 2 + 1920 + 5)) < 1) BitmapPosite = 2;
                                else BitmapPosite = 1;


                                OpenWindows.Add(new ProcessInfo { Handle = hWnd, Title = title, Bitmap = bitmap, BitmapPosite = BitmapPosite });
                            }
                        }
                    }
                    return true;
                },
                IntPtr.Zero
            );
            }
            catch (Exception ex)
            {
                logger.Error($"获取进程失败：{ex.Message}", "错误");
            }
            

            return OpenWindows;
        }
    }
}
