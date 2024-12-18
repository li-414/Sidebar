using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Sidebar.Application.Servers;
using Sidebar.Infrastructure.Helpers;
using Sidebar.Infrastructure.Servers;

namespace Sidebar.Infrastructure.Extensions
{
    public static class ServerExtensions
    {
        public static ServiceCollection ConfigurationServer(this ServiceCollection services)
        {
            // 配置 Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddSingleton(Log.Logger);
            services.AddSingleton<LacalVersionHelper>();
            services.AddSingleton<ColorHelper>();
            services.AddSingleton<DesktopLinkHelper>();

            services.AddSingleton<IProcessServer, ProcessServer>();
            services.AddSingleton<IUpdateService, UpdateService>();
            services.AddSingleton<IDesktopLinkServer, DesktopLinkServer>();
            services.AddSingleton<IColorServer, ColorServer>();

            return services;
        }
    }
}
