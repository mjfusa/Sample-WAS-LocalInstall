using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.UI.Composition;

namespace LauncherDepdendencyCheck
{
    public class InstallDepdendencies 
    {
        private static PackageManager _PackageManager;
        public static PackageManager PackageManager
        {
            get
            {
                if (_PackageManager == null)
                {
                    _PackageManager = new PackageManager();
                }
                return _PackageManager;
            }
        }
        public static List<string> FilesToInstall;
        public static Dictionary<string, MsixInfo> PackageInfo = new Dictionary<string, MsixInfo>();
        public void GetInfo()
        {
            new InstallInfo();
        }
        public async Task<bool> Install()
        {
            var installFolder = Package.Current.EffectivePath;
            int cnt = 0;
            bool returnValue = true;
            try
            {
                foreach (var pi in PackageInfo)
                {
                    if (!pi.Value.Installed)
                    {
                        // TODO Display install progress
                        var deploymentOperation = PackageManager.AddPackageAsync(new Uri("file://" + FilesToInstall[cnt]), null, DeploymentOptions.None);
                        // This event is signaled when the operation completes
                        ManualResetEvent opCompletedEvent = new ManualResetEvent(false);

                        // Define the delegate using a statement lambda
                        deploymentOperation.Completed = (depProgress, status) => { opCompletedEvent.Set(); };

                        // Wait until the operation completes
                        opCompletedEvent.WaitOne();

                        // Check the status of the operation
                        if (deploymentOperation.Status == AsyncStatus.Error)
                        {
                            DeploymentResult deploymentResult = deploymentOperation.GetResults();
                            Debug.WriteLine("Error code: {0}\n", deploymentOperation.ErrorCode);
                            Debug.WriteLine("Error text: {0}\n", deploymentResult.ErrorText);
                            returnValue = false;
                        }
                        else if (deploymentOperation.Status == AsyncStatus.Canceled)
                        {
                            Debug.WriteLine("Installation canceled\n");
                        }
                        else if (deploymentOperation.Status == AsyncStatus.Completed)
                        {
                            Debug.WriteLine("Installation succeeded\n");
                        }
                        else
                        {
                            returnValue = false;
                            Debug.WriteLine("Installation status unknown\n");
                        }
                    }
                    cnt++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return returnValue;
        }
    }

    public class InstallInfo
    {
        private List<string> GetFilesToInstall()
        {
            try
            {
                // get MSIX / APPX files to be checked and installed.
                var installFolder = Package.Current.EffectivePath; // NOTE: Packaged Apps Only
                List<string> files = Directory.EnumerateFiles(installFolder + "//Dependencies", "*.???X", SearchOption.AllDirectories).ToList();
                // Determine bitness of running process
                string result = System.Environment.Is64BitProcess switch
                {
                    true => "x64",
                    false => "x86"
                };
                // Get the depdendencies relative to the running machines architecture
                return files.Where(f => f.Contains("Dependencies\\" + result)).ToList();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("App is not packaged. Please run as an MSIX packaged app.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static bool GetPackageInfo(List<string> msixfiles)
        {
            // Get package names and versions of MSIX depencency packages.
            try
            {
                foreach (string file in msixfiles)
                {
                    MemoryStream data = new MemoryStream();
                    var zip = ZipFile.OpenRead(file);
                    var stream = zip.GetEntry("AppxManifest.xml").Open();
                    if (stream != null)
                    {
                        stream.CopyTo(data);
                    }
                    data.Seek(0, SeekOrigin.Begin);
                    XDocument doc = XDocument.Load(data);
                    XNamespace packageNamespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
                    var identityNode = doc.Descendants(packageNamespace + "Identity").First();
                    var mi = new MsixInfo(identityNode.Attribute("Name").Value, identityNode.Attribute("Version").Value);
                    InstallDepdendencies.PackageInfo.Add(Path.GetFileName(file), mi);
                    data.Dispose();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;

        }
        private bool IsInstalled()
        {
            // Compare PackageFamily name in Manifest what is installed on machine
            foreach (var pi in InstallDepdendencies.PackageInfo)
            {
                var result = InstallDepdendencies.PackageManager.FindPackagesForUserWithPackageTypes(string.Empty, PackageTypes.Framework);
                foreach (var fwpkg in result)
                {
                    if (fwpkg.Id.Name.Contains(pi.Value.PackageFamily))
                    {
                        // Depdendency is install, now check the version
                        if (!IsOutOfDate(fwpkg.Id.Version, pi.Value.Version))
                        {
                            pi.Value.Installed = true;
                        }


                        break;
                    }
                }
            }
            return true;
        }

        private bool IsOutOfDate(PackageVersion currentVersion, string versionToInstall)
        {
            var curVersion = $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}.{currentVersion.Revision}";
            int[] verToInstall = versionToInstall.Split('.').Select(int.Parse).ToArray();
            int[] verCurVersion = curVersion.Split('.').Select(int.Parse).ToArray();

            for (uint cnt = 0; cnt < verCurVersion.Length; cnt++)
            {
                if (verToInstall[cnt] > verCurVersion[cnt])
                {
                    return true;
                }
            }
            return false;
        }

        public InstallInfo()
        {
            // Get MSIX files in 'Depdendencies folder
            InstallDepdendencies.FilesToInstall = GetFilesToInstall();
            // Get version and Package Family name info from MSIX files
            GetPackageInfo(InstallDepdendencies.FilesToInstall);
            // Determine if depdencencies are installed. Update PackageInfo class
            IsInstalled();
        }

    }

    public class MsixInfo
    {
        public MsixInfo(string packageFamily, string version)
        {
            Version = version;
            PackageFamily = packageFamily;
        }

        [DefaultValue(false)]
        public bool Installed { get; set; }
        public string Version { get; set; }
        public string PackageFamily { get; set; }
    }

}
