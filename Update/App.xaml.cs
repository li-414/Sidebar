using System.Data;
using System.Diagnostics;
using System.Windows;

namespace Update
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CloseApp("Sidebar");
        }

        private void CloseApp(string currentProcessName)
        {
            // 当前程序的进程名（不包含扩展名）
            //string currentProcessName = Process.GetCurrentProcess().ProcessName;

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
                    // 处理异常，例如没有权限终止进程等
                    MessageBox.Show($"关闭线程错误：{ex.Message}");
                }
            }
        }
    }

}
