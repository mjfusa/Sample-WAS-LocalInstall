using System;
using System.Threading.Tasks;
using Windows.Management.Deployment;

namespace LauncherDepdendencyCheck
{
    public class WASDepdendencyCheck : IDepdendencyCheck
    {
        public WASDepdendencyCheck(string packageFamily)
        {
            PackageFamily = packageFamily;
        }

        public string PackageFamily { get; private set; }

        public IInstallInfo GetInfo()
        {
            return new WASInstallInfo(PackageFamily);
        }

        public string GetVersion()
        {
            return "";
        }

        Task<bool> IDepdendencyCheck.Install()
        {
            throw new NotImplementedException();
        }
    }

    //public class InstallInfo
    //{
    //    public InstallInfo(string version) => (Version) = (version);
    //    public string Version { get; }
    //    internal InstallType InstallType => Version switch
    //    {
    //        var version when version.Contains("dev") => InstallType.EdgeChromiumDev,
    //        var version when version.Contains("beta") => InstallType.EdgeChromiumBeta,
    //        var version when version.Contains("canary") => InstallType.EdgeChromiumCanary,
    //        var version when !string.IsNullOrEmpty(version) => InstallType.WebView2,
    //        _ => InstallType.NotInstalled
    //    };
    //}
    public class WASInstallInfo : IInstallInfo
    {
        public WASInstallInfo(string packageFamily) => (PackageFamily) = (packageFamily);
        public InstallType InstallType()
        {
            var pm = new PackageManager();
                var result = pm.FindPackagesForUserWithPackageTypes(string.Empty, PackageTypes.Framework);
                foreach (var fwpkg in result) { 
                    if (fwpkg.Id.Name.Contains(PackageFamily))
                    {
                        Version = $"{fwpkg.Id.Version.Major}.{fwpkg.Id.Version.Minor}.{fwpkg.Id.Version.Build}.{fwpkg.Id.Version.Revision}";
                        break;
                    }
                }
            //    if (string.IsNullOrEmpty(Version))
            //        break;
            ////}
            return string.IsNullOrEmpty(Version) ? LauncherDepdendencyCheck.InstallType.NotInstalled : LauncherDepdendencyCheck.InstallType.PackageInstalled;
        }
        public string Version { get; set; }
        public string PackageFamily { get; set; }
        public bool Installed { get; set; }
    }

}