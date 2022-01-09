using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using System.Diagnostics;
using LauncherWebViewCheck;

namespace LauncherDepdendencyCheck
{
    public static class AppLogger
    {
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
