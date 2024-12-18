namespace Sidebar.Domain.Models
{
    public class DesktopIcon
    {
        public DesktopIcon(string title, string path)
        {
            Title = title;
            Path = path;
        }

        public string Title { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
