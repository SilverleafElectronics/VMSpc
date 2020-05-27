using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VMSpc.JsonFileManagers;
using System.IO.Compression;
using System.IO;
using VMSpc.Exceptions;
using VMSpc.Loggers;

namespace VMSpc
{
    public static class EngineDownloader
    {
        public static bool DownloadEngines()
        {
            try
            {
                var engineZipFile = new FileOpener("\\EngineArchive.zip").absoluteFilepath;
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://silverleafelectronics.com/silverleafelectronics.com/sites/default/files/2019-04/Additional_Engines_%2810-09-2018%29.zip", engineZipFile);
                    if (FileOpener.FileExists("\\EngineArchive.zip"))
                    {
                        return UnzipEngineArchive();
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        private static bool UnzipEngineArchive()
        {
            try
            {
                var SourceZipFilePath = new FileOpener("\\EngineArchive.zip").absoluteFilepath;
                var RelativeDeploymentPath = new FileOpener("\\engines").absoluteFilepath;
                ZipFile.ExtractToDirectory(SourceZipFilePath, RelativeDeploymentPath);
                MoveToBaseEnginePath();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void MoveToBaseEnginePath()
        {
            var absoluteDir = new FileOpener("\\engines\\Additional_Engines_(10-09-2018)").absoluteFilepath;
            var absoluteSourceDir = new FileOpener("\\engines").absoluteFilepath;
            var dirFiles = FileOpener.GetDirectoryFiles("\\engines\\Additional_Engines_(10-09-2018)");
            foreach (var file in dirFiles)
            {
                FileInfo mFile = new FileInfo(file);
                mFile.MoveTo(absoluteSourceDir + "\\" + mFile.Name);
            }
            try
            {
                Cleanup();
            }
            catch (Exception ex)
            {
                ErrorLogger.GenerateErrorRecord(ex);
            }
        }

        private static void Cleanup()
        {
            FileOpener.DeleteFile("\\EngineArchive.zip");
            FileOpener.DeleteDirectory("\\engines\\Additional_Engines_(10-09-2018)");
        }
    }
}
