using LauncherDepdendencyCheck;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net.WebSockets;
using Windows.System;

namespace LauncherWebViewCheck
{
    class Program
    {
        static public bool IsNotInstalled => !IsInstalled;

        private static string[] packageFamilyNames = {
                "Microsoft.WindowsAppRuntime",
                "UWPDesktop",
                "Microsoft.VCLibs.140.00"
            };
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
            var id = new InstallDepdendencies();
            id.GetInfo();
            id.Install();

            Process.Start(new ProcessStartInfo("wasmainapp:") { UseShellExecute = true });

        }
    }
}
