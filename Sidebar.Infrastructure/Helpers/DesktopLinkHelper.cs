using Shell32;
using Sidebar.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidebar.Infrastructure.Helpers
{
    internal class DesktopLinkHelper
    {
        public List<DesktopIcon> GetDesktopIcons()
        {
            List<DesktopIcon> icons = new List<DesktopIcon>();
            Shell shell = new Shell();
            Shell32.Folder desktopFolder = shell.NameSpace(ShellSpecialFolderConstants.ssfDESKTOP);

            foreach (FolderItem2 item in desktopFolder.Items())
            {
                if (item.IsLink)
                {
                    try
                    {
                        string extension = System.IO.Path.GetExtension(item.Path)?.ToLower();
                        if (extension == ".url")
                            continue; // 跳过 URL 快捷方式

                        string itemPath = GetRegularFilePath(item.Path);
                        if (string.IsNullOrEmpty(itemPath))
                            continue;
                        icons.Add(new DesktopIcon(item.Name, itemPath));
                    }
                    catch (Exception ex)
                    {
                        LogException($"{item.Path} 路径格式错误", ex);
                    }
                   
                }

            }
            return icons;
        }

        private string GetRegularFilePath(string shortcutPath)
        {
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
            // 获取快捷方式的目标路径
            var link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
            return link.TargetPath;
        }

        private void LogException(string context, Exception ex)
        {
            // 日志输出，例如写入文件
            string logPath = "error.log";
            File.AppendAllText(logPath, $"{DateTime.Now} [{context}] {ex.Message}\n{ex.StackTrace}\n\n");
        }
    }
}
