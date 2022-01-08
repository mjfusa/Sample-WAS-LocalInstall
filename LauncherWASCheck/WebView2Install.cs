using System;
using System.Threading.Tasks;
using LauncherDepdendencyCheck;
using Microsoft.Web.WebView2.Core;
// Thanks https://github.com/mortenbrudvik/KioskBrowser

namespace LauncherDepdendencyCheck
{
    public class WebView2DepdendencyCheck : IDepdendencyCheck
    {
        public IInstallInfo GetInfo()
        {
            var version = GetVersion();
            return new WebView2InstallInfo(version);
        }

        public string GetVersion()
        {
            try
            {
                return CoreWebView2Environment.GetAvailableBrowserVersionString();
            }
            catch (Exception) 
            { 
                return ""; 
            }
        }

        Task<bool> IDepdendencyCheck.Install()
        {
            throw new NotImplementedException();
        }
    }
    public class WebView2InstallInfo : IInstallInfo
    {
        public WebView2InstallInfo(string version) => (Version) = (version);
        public InstallType InstallType()
        {
            var result = Version switch
            {
                var version when version.Contains("dev") => LauncherDepdendencyCheck.InstallType.EdgeChromiumDev,
                var version when version.Contains("beta") => LauncherDepdendencyCheck.InstallType.EdgeChromiumBeta,
                var version when version.Contains("canary") => LauncherDepdendencyCheck.InstallType.EdgeChromiumCanary,
                var version when !string.IsNullOrEmpty(version) => LauncherDepdendencyCheck.InstallType.WebView2,
                _ => LauncherDepdendencyCheck.InstallType.NotInstalled
            };
            return result;
        }
        public string Version { get; set; }
        public bool Installed { get; set; }
    }
}