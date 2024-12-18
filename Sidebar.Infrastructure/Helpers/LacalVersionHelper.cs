using Serilog;

namespace Sidebar.Infrastructure.Helpers
{
    public class LacalVersionHelper
    {
        private readonly ILogger logger;

        public LacalVersionHelper(ILogger logger)
        {
            this.logger = logger;
        }

        // 获取用户目录路径
        private string GetVersionFilePath()
        {
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!Directory.Exists(Path.Combine(localAppDataPath, "Sidebar")))
            {
                Directory.CreateDirectory(Path.Combine(localAppDataPath, "Sidebar"));
            }
            return Path.Combine(localAppDataPath, "Sidebar", "Version.txt");
        }


        // 读取当前版本号
        public string? GetCurrentVersion()
        {
            try
            {
                string versionFilePath = GetVersionFilePath();

                if (!File.Exists(versionFilePath))
                {
                    SetCurrentVersion("1.0.0.0");
                }
                var localver = File.ReadAllText(versionFilePath);
                if (string.IsNullOrEmpty(localver))
                {
                    SetCurrentVersion("1.0.0.0");
                }
                return File.ReadAllText(versionFilePath);

            }
            catch (Exception ex)
            {
                logger.Error($"读取版本号失败: {ex.Message}");
            }

            return null;
        }

        // 设置当前版本号
        public void SetCurrentVersion(string? version)
        {
            if (version == null) {return; }
            try
            {
                string versionFilePath = GetVersionFilePath();
                File.WriteAllText(versionFilePath, version);
            }
            catch (Exception ex)
            {
                logger.Error($"保存版本号失败: {ex.Message}");
            }
        }
    }
}
