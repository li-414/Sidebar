namespace Sidebar.Domain.Helpers
{
    public class SidebarExclude
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
                "小火箭托盘态"
            };
        }
    }
}
