namespace Sidebar.Application.Servers
{
    public interface IUpdateService
    {
        Task UpdateVersionAsync();
        Task<bool> CheckForUpdatesAsync(Func<string, string, bool> func);
    }
}
