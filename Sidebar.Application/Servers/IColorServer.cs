namespace Sidebar.Application.Servers
{
    public interface IColorServer
    {
        Task<(byte r, byte g, byte b)> ColorRefreshAsync(double x, double y, double width, double height);
    }
}
