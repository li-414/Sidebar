using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Sidebar.Application.Servers;
using Sidebar.ViewModels;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Sidebar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static ServiceProvider? ServiceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // 捕获非 UI 线程未处理的异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 捕获任务线程未处理的异常
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            try
            {
                //注册服务
                var services = new ServiceCollection();

                string infrastructurePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sidebar.Infrastructure.dll");
                if (!File.Exists(infrastructurePath))
                    return;

                // 加载程序集
                var infrastructureAssembly = Assembly.LoadFrom(infrastructurePath);
                Console.WriteLine($"Loaded assembly: {infrastructureAssembly.FullName}");

                // 查找扩展方法 ConfigurationServer
                var method = infrastructureAssembly
                    .GetTypes()
                    .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
                    .FirstOrDefault(m => m.Name == "ConfigurationServer" && m.IsStatic);

                if (method != null)
                {
                    // 调用 ConfigurationServer 方法，传递 ServiceCollection 实例
                    method.Invoke(null, new object[] { services });
                }

                ServiceProvider = services.BuildServiceProvider();


                //判断是否已有线程
                IProcessServer processServer = ServiceProvider.GetRequiredService<IProcessServer>();
                processServer.Close();

                //更新检测
                IUpdateService updateService = ServiceProvider.GetRequiredService<IUpdateService>();
                if (e.Args.Count() != 0) { await updateService.UpdateVersionAsync(); }
                bool isUpdate = await updateService.CheckForUpdatesAsync((a, b) =>
                {
                    // 有新版本可用，提示用户是否更新
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"发现新版本 {a}。旧版本 {b}。 是否立即更新？", "更新提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.Yes)
                        return true;
                    else
                        return false;
                });
                if (!isUpdate)
                {
                    var mainWindow = new MainWindow();
                    App.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                }
                else
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.StackTrace);
                System.Windows.MessageBox.Show(ex.Message);
            }

            

        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogException("UI 线程异常", e.Exception);
            e.Handled = true; // 防止应用程序立即退出
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogException("非 UI 线程异常", ex);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException("任务线程异常", e.Exception);
            e.SetObserved(); // 避免程序崩溃
        }

        private void LogException(string context, Exception ex)
        {
            // 日志输出，例如写入文件
            string logPath = "error.log";
            File.AppendAllText(logPath, $"{DateTime.Now} [{context}] {ex.Message}\n{ex.StackTrace}\n\n");
        }
    }
}
