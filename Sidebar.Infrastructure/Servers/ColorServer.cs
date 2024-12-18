using Sidebar.Application.Servers;
using Sidebar.Infrastructure.Helpers;

namespace Sidebar.Infrastructure.Servers
{
    public class ColorServer : IColorServer
    {
        private readonly ColorHelper colorHelper;

        public ColorServer(ColorHelper colorHelper)
        {
            this.colorHelper = colorHelper;
        }

        public async Task<(byte r, byte g, byte b)> ColorRefreshAsync(double x, double y, double width, double height)
        {
            var dominantColor = await Task.Run(() => colorHelper.GetDominantColorAsync((int)x, (int)y, (int)width, (int)height));
            return dominantColor;
        }
    }
}
