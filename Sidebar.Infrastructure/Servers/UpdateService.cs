using Serilog;
using Sidebar.Application.Servers;
using Sidebar.Infrastructure.Helpers;
using System.Diagnostics;

namespace Sidebar.Infrastructure.Servers
{
    public class UpdateService : IUpdateService
    {
        private readonly ILogger logger;
        private readonly LacalVersionHelper lacalVersionHelper;
        private string remoteXml =
            "http://101.43.204.170:8080/userContent/Version/SidebarVersion.xml";

        public UpdateService(ILogger logger, LacalVersionHelper lacalVersionHelper)
        {
            this.logger = logger;
            this.lacalVersionHelper = lacalVersionHelper;
        }

        public async Task<bool> CheckForUpdatesAsync(Func<string, string, bool> func)
        {
            var LocalVersion = lacalVersionHelper.GetCurrentVersion();
            string? RemoteVersion = await WebXmlHelper.GetVersionFromXml(remoteXml);

            if (string.IsNullOrEmpty(RemoteVersion))
            {
                return false;
            }


            if (!string.IsNullOrEmpty(LocalVersion))
            {
                // 将版本号字符串转换为 Version 对象
                Version currentVersion = new Version(LocalVersion);
                Version newVersion = new Version(RemoteVersion);

                // 比较版本号
                int result = newVersion.CompareTo(currentVersion);

                if (result > 0)
                {
                    bool? isUpdate = (bool?)(func?.Invoke(RemoteVersion, LocalVersion));
                    if (isUpdate == true)
                    {
                        try
                        {
                            string localAppDataPath = System.IO.Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                "Sidebar",
                                "Update.exe"
                            );
                            var processStartInfo = new ProcessStartInfo
                            {
                                FileName = localAppDataPath,
                                Verb = "runas", // 以管理员身份启动
                                UseShellExecute = true, // 使用 Windows Shell 启动程序
                                CreateNoWindow =
                                    true // 不显示命令行窗口
                                ,
                            };
                            Process.Start(processStartInfo);
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"打开更新窗口错误：{ex.Message}");
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public async Task UpdateVersionAsync()
        {
            string? RemoteVersion = await WebXmlHelper.GetVersionFromXml(remoteXml);
            lacalVersionHelper.SetCurrentVersion(RemoteVersion);
        }
    }
}
