using LauncherDepdendencyCheck;
using Xunit;

namespace LauncherDependencyCheck.Tests
{
    public class LauncherDependencyCheckUnitTest
    {
        [Fact]
        public void VerifyMSIXDependenciesAreCorrectlyDeployed()
        {
            var installInfo = new InstallInfo();
            var filesToInstall = installInfo.GetFilesToInstall();

            Assert.Empty(filesToInstall);   
        }
    }
}