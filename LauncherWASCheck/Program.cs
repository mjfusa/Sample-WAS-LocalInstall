using LauncherDepdendencyCheck;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using WinRT;

namespace LauncherWebViewCheck
{
    public class Program
    {

        static public bool IsNotInstalled => !IsInstalled;

        public static bool _isInstalled;
        public static bool IsInstalled
        {
            get
            {
                return _isInstalled;
            }
            set
            {
                _isInstalled = value;
            }
        }

        public static object DeploymentManager { get; private set; }

        static bool DoWindowsAppRuntimeInstall()
        {

            return true;
        }

        static void Main(string[] args)
        {
            AppLogger.Init();
            try
            {
                var id = new InstallDepdendencies();
                id.GetInfo();
                id.Install();
            } catch (Exception ex)
            {
                AppLogger.Logger.LogCritical(ex.Message);
            }

            Process.Start(new ProcessStartInfo("wasmainapp:") { UseShellExecute = true });

        }

    }
}
