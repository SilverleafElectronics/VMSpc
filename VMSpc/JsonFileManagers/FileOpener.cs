using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;

namespace VMSpc.JsonFileManagers
{
    public enum FilePathType
    {
        Relative,
        Absolute,
    }

    public class FileOpener
    {
        protected string filepath;
        public string absoluteFilepath => BaseDirectory + filepath;
        private static string GetBaseDirectory()
        {
            var exeLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
            var indexOf = exeLocation.IndexOf("VMSpc.exe") - 1;//-1 to remove the \\
            var withoutExeName = exeLocation.Substring(0, indexOf);
            return withoutExeName;
        }

        public static string BaseDirectory => GetBaseDirectory();
        /// <summary>
        /// Wrapper class to enforce consistency of file/directory access across the application. Should be used
        /// to read/write to all files in VMSpc with relative paths. 
        /// </summary>
        /// This class is required because using './{path}' does not always guarantee that the executable's
        /// directory will be used as the base directory of the path. Add additional wrapper methods as needed
        /// <param name="filePath"></param>
        public FileOpener(string filepath)
        {
            this.filepath = filepath;
        }

        public static string GetAbsoluteFilePath(string filePath, FilePathType filePathType = FilePathType.Relative)
        {
            switch (filePathType)
            {
                case FilePathType.Absolute:
                    return filePath;
                case FilePathType.Relative:
                    return BaseDirectory + filePath;
            }
            return null;
        }

        public bool Exists()
        {
            return File.Exists(absoluteFilepath);
        }

        public static bool FileExists(string filePath, FilePathType filePathType = FilePathType.Relative)
        {
            return File.Exists(GetAbsoluteFilePath(filePath, filePathType));
        }

        public static bool IsFileLocked(string filePath, FilePathType filePathType = FilePathType.Relative)
        {
            try
            {
                var file = new FileInfo(GetAbsoluteFilePath(filePath, filePathType));
                using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the name of the file without the preceding path. Example: "C:\VMSpc\rawlog.vms" becomes "rawlog.vms"
        /// </summary>
        public static string GetFileName(string filePath)
        {
            var fileNameStartIndex = filePath.LastIndexOf('\\') + 1;
            if (fileNameStartIndex == 1)    //if the filepath doesn't have Windows-style directory-dividing characters, try using '/'
                fileNameStartIndex = filePath.LastIndexOf('/');
            if (fileNameStartIndex == 1)    //if the filpath doesn't have any directory-dividing characters, the filepath is already the filename
                return filePath;
            return filePath.Substring(fileNameStartIndex);
        }

        public static bool IsFileEmpty(string filePath, FilePathType filePathType = FilePathType.Relative)
        {
            return (new FileInfo(GetAbsoluteFilePath(filePath, filePathType)).Length == 0);
        }

        /// <summary>
        /// Returns true if the file is "Null" (doesn't exist) or "Empty" (Length == 0)
        /// </summary>
        public static bool IsNullOrEmpty(string filePath, FilePathType filePathType = FilePathType.Relative)
        {
            return (!FileExists(filePath, filePathType) || IsFileEmpty(filePath, filePathType));
        }

        public static void CopyFile(string absoluteFilePathSource, string relativeFilePathDestination)
        {
            File.Copy(absoluteFilePathSource, BaseDirectory + relativeFilePathDestination);
        }

        public static bool DirectoryExists(string relativeFilePath)
        {
            return Directory.Exists(BaseDirectory + relativeFilePath);
        }

        public static bool IsDirectoryEmpty(string relativeDirectoryPath)
        {
            return (Directory.GetFiles(BaseDirectory + relativeDirectoryPath).Length < 1);
        }

        public StreamReader GetStreamReader()
        {
            return new StreamReader(absoluteFilepath);
        }

        public StreamWriter GetStreamWriter()
        {
            return new StreamWriter(absoluteFilepath);
        }

        /// <summary>
        /// Wrapper for File.WriteAllText. Creates a file, write Content to the file, and then closes the file. 
        /// If the file already exists, it will be overwritten with the specified Content.
        /// </summary>
        public static void WriteAllText(string relativePath, string Content)
        {
            File.WriteAllText(BaseDirectory + relativePath, Content);
        }

        /// <summary>
        /// Creates a directory relative to the executable's base directory. Usage: FileOpener.CreateDirectory("\\{Directory}[\\{SubDirectory}...]")
        /// </summary>
        /// <param name="relativeDirectoryPath"></param>
        public static void CreateDirectory(string relativeDirectoryPath)
        {
            var dir = Directory.CreateDirectory(BaseDirectory + relativeDirectoryPath);
            var dirSecurity = new DirectorySecurity();
        }

        public static string[] GetDirectoryFiles(string relativeDirectoryPath)
        {
            return Directory.GetFiles(BaseDirectory + relativeDirectoryPath);
        }

        public static void DeleteFile(string relativeFilePath)
        {
            File.Delete(BaseDirectory + relativeFilePath);
        }

        public static void DeleteDirectory(string relativeDirectoryLocation)
        {
            Directory.Delete(BaseDirectory + relativeDirectoryLocation);
        }

        public static string GetAbsoluteDirectoryPath(string directoryPath, FilePathType filePathType = FilePathType.Relative)
        {
            switch (filePathType)
            {
                case FilePathType.Absolute:
                    return directoryPath;
                case FilePathType.Relative:
                    return BaseDirectory + directoryPath;
            }
            return null;
        }
    }
}
