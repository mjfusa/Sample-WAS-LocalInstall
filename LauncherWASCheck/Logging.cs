using LauncherWebViewCheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LauncherDepdendencyCheck
{
    public static class AppLogger
    {
        // Task: Provide Debug and Windows Event logging. In Event Viewer: Windows logs | Application. Source == .NET Runtime
        public static ILogger<Program> Logger { get;  set; }

        public static bool Init()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(c => {
                    c.AddConsole(opt => opt.LogToStandardErrorThreshold = LogLevel.Debug);
                    c.AddEventLog();
                }
                )
                .BuildServiceProvider();
            Logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            return true;
        }
    }
}
