using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VMSpc.JsonFileManagers;
using System.Diagnostics;

namespace VMSpc
{
    public static class DriverInstaller
    {
        public static bool InstallDrivers()
        {
            try
            {
                if (!FileOpener.DirectoryExists("\\installer"))
                {
                    var installerZipFile = new FileOpener("\\InstallerArchive.zip").absoluteFilepath;
                    using (var client = new WebClient())
                    {
                        client.DownloadFile("https://www.silverleafelectronics.com/silverleafelectronics.com/sites/default/files/2019-04/Signed_Drivers_Only_04-16-2015.zip", installerZipFile);
                        if (FileOpener.FileExists("\\InstallerArchive.zip"))
                        {
                            UnzipInstallerArchive();
                            FileOpener.DeleteFile("\\InstallerArchive.zip");
                        }
                    }
                }
                var process = Process.Start(new FileOpener("\\installer").absoluteFilepath + "\\Signed_Drivers_Only_04-16-2015\\Install_Drivers.exe");
                process.WaitForExit();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static void UnzipInstallerArchive()
        {
            var SourceZipFilePath = new FileOpener("\\InstallerArchive.zip").absoluteFilepath;
            var RelativeDeploymentPath = new FileOpener("\\installer").absoluteFilepath;
            ZipFile.ExtractToDirectory(SourceZipFilePath, RelativeDeploymentPath);
        }
    }
}
