using LauncherDepdendencyCheck;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Diagnostics;

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
                id.Install().Wait();
            } catch (Exception ex)
            {
                AppLogger.Logger.LogCritical(ex.Message);
                return;
            }

            try
            {
                // Task: Start process in container using protocol. See package manifest for protocol definition
                var psResult = Process.Start(new ProcessStartInfo("wasmainapp:") { UseShellExecute = true });
                if (psResult == null)
                {
                    AppLogger.Logger.LogCritical($"Problem with starting {psResult.ProcessName}");
                }
                else
                {
                    AppLogger.Logger.LogInformation($"Process {psResult.ProcessName} successfully started.");
                }
            } catch (Win32Exception ex)
            {
                AppLogger.Logger.LogCritical(ex.Message);
            }

        }

    }
}
