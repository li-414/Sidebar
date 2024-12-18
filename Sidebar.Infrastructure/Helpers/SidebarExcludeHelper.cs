namespace Sidebar.Infrastructure.Helpers
{
    public class SidebarExcludeHelper
    {
        public static List<string> Titles = new List<string>();

        public static bool Exclude(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                Titles = GetExclude();
                if (Titles.Contains(title))
                {
                    return true;
                }
            }
            return false;
        }

        private static List<string> GetExclude()
        {
            return Titles = new List<string>
            {
                "Sidebar",
                "PlayerPluginWnd",
                "playerShadow",
                "Windows 输入体验",
                "Program Manager",
                "电影和电视",
                "小火箭托盘态",
                "Windows Shell Experience 主机",
                "MainWindow",
                "ToolBarHiddenWindow",
                "SOUI_DUMMY_WND",
                "TFT_SIDE_BTN",
                "BUG_REPORT_BTN",
                "主机弹出窗口",
                "SIDE_WEB_VIEW",
                "HELPER_BTN"
            };
        }
    }
}
