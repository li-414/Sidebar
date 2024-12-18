using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows;
using System.Xml.Linq;

namespace Update
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string RemoteXml = "http://101.43.204.170:8080/userContent/Version/SidebarVersion.xml";
        string updatePack =
            "http://101.43.204.170:8080/job/Sidebar/lastSuccessfulBuild/artifact/Sidebar.zip";

        // 获取用户目录路径存储下载文件
        string installerPathTemp = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Sidebar"
        );
        string localpath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Sidebar",
            "Sidebar.zip"
        );

        string startupApp = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Sidebar",
            "Sidebar.exe"
        );

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Tuple<string, string> xmlValue = await GetFromXml(RemoteXml);
            if (xmlValue != null)
            {
                VersionText.Text = xmlValue.Item1;
                UpdateLogText.Text = xmlValue.Item2;
            }

            await Task.Run(async () =>
            {
                bool isDown = await DownLoadInstallPack(
                    updatePack,
                    localpath,
                    (a, b) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var num = Math.Round((double)a / (double)b * 98);
                            progressBar.Value = num;
                        });
                    }
                );
                if (isDown == false)
                    return;
            });

            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    progressBar.Value = 99;
                });
                ExtractZipSkipExistingFiles(localpath, installerPathTemp);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    progressBar.Value = 100;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解压失败: {ex.Message}");
                return;
            }

            try
            {
                File.Delete(localpath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除安装包失败: {ex.Message}");
            }

            try
            {
                Process.Start(startupApp, "Succes");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动程序失败: {ex.Message}");
            }

            Application.Current.Shutdown();
        }

        private void ExtractZipSkipExistingFiles(string zipPath, string extractPath)
        {
            try
            {
                // 确保目标目录存在
                Directory.CreateDirectory(extractPath);

                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        string destinationPath = System.IO.Path.Combine(
                            extractPath,
                            entry.FullName
                        );

                        if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                        {
                            // 创建文件夹路径
                            Directory.CreateDirectory(destinationPath);
                        }
                        else
                        {
                            // 如果文件已存在，跳过该文件
                            if (File.Exists(destinationPath))
                            {
                                File.Delete(destinationPath);
                            }

                            // 创建目录（如果不存在）
                            string directoryPath = System.IO.Path.GetDirectoryName(destinationPath);
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            // 解压文件
                            entry.ExtractToFile(destinationPath);
                        }
                    }
                }

                Console.WriteLine("解压完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解压失败: {ex.Message}");
            }
        }

        private async Task<bool> DownLoadInstallPack(
            string downLoadUrl,
            string installPath,
            Action<long, long> progressActive
        )
        {
            try
            {
                // 使用 HttpClient 下载文件
                using (HttpClient client = new HttpClient())
                {
                    Console.WriteLine("开始下载更新包...");

                    var byteArray = System.Text.Encoding.ASCII.GetBytes("admin:admin");
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue(
                            "Basic",
                            Convert.ToBase64String(byteArray)
                        );

                    // 获取文件大小
                    HttpResponseMessage response = await client.GetAsync(
                        downLoadUrl,
                        HttpCompletionOption.ResponseHeadersRead
                    );
                    response.EnsureSuccessStatusCode();
                    long totalFileSize = response.Content.Headers.ContentLength ?? 0;

                    // 创建文件流
                    using (
                        var fileStream = new FileStream(
                            installPath,
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.None
                        )
                    )
                    {
                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            byte[] buffer = new byte[8192]; // 8KB 缓冲区
                            long totalBytesRead = 0;
                            int bytesRead;

                            // 下载并写入文件，同时更新进度条
                            while (
                                (
                                    bytesRead = await contentStream.ReadAsync(
                                        buffer,
                                        0,
                                        buffer.Length
                                    )
                                ) > 0
                            )
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalBytesRead += bytesRead;

                                progressActive.Invoke(totalBytesRead, totalFileSize);
                                // 计算并显示进度条
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"错误记录：{DateTime.Now}\n {ex.Message}");
                return false;
            }
        }

        private async Task<Tuple<string, string>> GetFromXml(string url)
        {
            try
            {
                // 使用 HttpClient 获取 XML 内容
                using (HttpClient client = new HttpClient())
                {
                    var byteArray = System.Text.Encoding.ASCII.GetBytes("admin:admin");
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue(
                            "Basic",
                            Convert.ToBase64String(byteArray)
                        );

                    string xmlContent = await client.GetStringAsync(url);

                    // 解析 XML 内容
                    XDocument xmlDoc = XDocument.Parse(xmlContent);

                    // 假设 XML 中的 version 位于 <version> 标签中
                    XElement versionElement = xmlDoc.Descendants("version").FirstOrDefault();
                    XElement updatelogElement = xmlDoc.Descendants("updatelog").FirstOrDefault();

                    if (versionElement != null && updatelogElement != null)
                    {
                        return new Tuple<string, string>(
                            versionElement.Value,
                            updatelogElement.Value
                        );
                    }
                    else
                    {
                        Console.WriteLine("Version element not found in the XML.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching or parsing XML: {ex.Message}");
                return null;
            }
        }
    }
}
