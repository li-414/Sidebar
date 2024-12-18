using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace Sidebar.Helpers
{
    class WebDownLoadHelper
    {
        // 下载并安装更新的异步方法
        public static async Task<bool> DownloadAndUpdateAsync(string url, string filePath)
        {
            try
            {
                // 使用 HttpClient 下载文件
                using (HttpClient client = new HttpClient())
                {
                    Console.WriteLine("开始下载更新包...");
                    byte[] fileBytes = await client.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(filePath, fileBytes);
                    Console.WriteLine("下载完成，文件已保存到：" + filePath);
                }

                // 执行 .msi 安装包
                Console.WriteLine("正在安装更新...");
                Process process = Process.Start("msiexec", $"/i \"{filePath}\" /passive /norestart");

                // 等待安装完成
                process.WaitForExit();

                // 检查安装结果
                if (process.ExitCode == 0)
                {
                    Console.WriteLine("安装成功！");
                    File.Delete(filePath);
                    return true;
                }
                else
                {
                    Console.WriteLine($"安装失败，退出代码：{process.ExitCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下载或安装过程中发生错误：{ex.Message}");
                return false;
            }
        }
    }
}
