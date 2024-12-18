using Sidebar.Domain.Models;

namespace Sidebar.Application.Servers
{
    public interface IDesktopLinkServer
    {
        List<DesktopIcon> RefreshDesktopLink();
    }
}
