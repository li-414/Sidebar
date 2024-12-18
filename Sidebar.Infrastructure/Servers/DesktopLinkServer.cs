using Sidebar.Application.Servers;
using Sidebar.Domain.Models;
using Sidebar.Infrastructure.Helpers;

namespace Sidebar.Infrastructure.Servers
{
    internal class DesktopLinkServer : IDesktopLinkServer
    {
        private readonly DesktopLinkHelper desktopLinkHelper;

        public DesktopLinkServer(DesktopLinkHelper desktopLinkHelper)
        {
            this.desktopLinkHelper = desktopLinkHelper;
        }

        public List<DesktopIcon> RefreshDesktopLink()
        {
            return desktopLinkHelper.GetDesktopIcons();
        }
    }
}
