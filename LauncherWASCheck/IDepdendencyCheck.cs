using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LauncherDepdendencyCheck
{
    public interface IDepdendencyCheck
    {
        public IInstallInfo GetInfo();
        public Task<bool> Install();
    }

    public interface IInstallInfo
    {
        //public InstallType InstallType();// { get;  }
        //public string Version { get; set; }
        //public bool Installed { get; set; }
    }


    public enum InstallType
    {
        WebView2, EdgeChromiumBeta, EdgeChromiumCanary, EdgeChromiumDev, NotInstalled, DeploymentStatus, Ok,
        PackageInstalledUpdateRequired,
        PackageInstalled,
        Unknown
    }

    public static class FindInstallType
    {
        public static InstallType InstallType(string itEnum)
        {
            int cnt = 0;
            foreach(string str in Enum.GetNames(typeof(LauncherDepdendencyCheck.InstallType)))
            {
                if (str == itEnum)
                {
                    return (InstallType)cnt;
                }
                cnt++;

            }
            return (InstallType)cnt;
        }
    }
}