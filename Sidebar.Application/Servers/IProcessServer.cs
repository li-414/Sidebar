using Sidebar.Domain.Models;

namespace Sidebar.Application.Servers
{
    public interface IProcessServer
    {
        void Close();
        void CloseProcess(nint runProcessId);

        List<ProcessInfo> RefreshProcess(bool isDrawBitmap);
    }
}
