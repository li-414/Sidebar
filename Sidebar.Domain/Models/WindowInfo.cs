using System.Drawing;

namespace Sidebar.Domain.Models
{
    public class ProcessInfo
    {
        public IntPtr Handle { get; set; }

        public string Title { get; set; } = string.Empty;

        public Bitmap Bitmap { get; set; }

        public int BitmapPosite { get; set; } = 1;
    }
}
