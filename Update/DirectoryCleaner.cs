using System.IO;

namespace Update
{


    public class DirectoryCleaner
    {
        public bool IsHasFile(string directoryPath)
        {
            try
            {
                // 检查目录是否存在
                if (!Directory.Exists(directoryPath))
                {
                    Console.WriteLine("目录不存在。");
                    return false;
                }

                // 获取目录中的所有文件
                var files = Directory.GetFiles(directoryPath);

                // 如果文件列表长度大于 0，说明有文件
                return files.Length > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查目录时出错：{ex.Message}");
                return false;
            }
        }

        // 删除指定目录中的所有内容（文件和子目录）
        public void CleanDirectory(string directoryPath)
        {
            try
            {
                // 检查目录是否存在
                if (Directory.Exists(directoryPath))
                {
                    // 获取所有文件
                    var files = Directory.GetFiles(directoryPath);
                    foreach (var file in files)
                    {
                        File.Delete(file);  // 删除文件
                    }

                    // 获取所有子目录
                    var directories = Directory.GetDirectories(directoryPath);
                    foreach (var dir in directories)
                    {
                        Directory.Delete(dir, true);  // 删除子目录及其内容
                    }

                    Console.WriteLine($"已清空目录: {directoryPath}");
                }
                else
                {
                    Console.WriteLine($"目录不存在: {directoryPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除目录内容时出错: {ex.Message}");
            }
        }

        public void DeleteDirectory(string directoryPath)
        {
            // 检查目录是否存在
            if (Directory.Exists(directoryPath))
            {
                // 删除目录及其内容（递归删除）
                Directory.Delete(directoryPath, true);
                Console.WriteLine($"已删除目录及其内容: {directoryPath}");
            }
            else
            {
                Console.WriteLine($"目录不存在: {directoryPath}");
            }
        }
    }

}
