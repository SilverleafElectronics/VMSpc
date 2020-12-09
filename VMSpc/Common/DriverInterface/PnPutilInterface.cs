using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace VMSpc.Common.DriverInterface
{
    public class DriverDetail
    {
        public string PublishedName { get; set; }
        public string PackageProvider { get; set; }
        public string Class { get; set; }
        public string Date { get; set; }
        public string SignerName { get; set; }

        public DriverDetail(string entry)
        {
            var lines = entry.Split('\n');
            if (!IsValidEntry(lines))
            {
                PublishedName = PackageProvider = Class = Date = SignerName = string.Empty;
                return;
            }
            PublishedName = lines[0].Split(':')[1].Trim();
            PackageProvider = lines[1].Split(':')[1].Trim();
            Class = lines[2].Split(':')[1].Trim();
            Date = lines[3].Split(':')[1].Trim();
            SignerName = lines[4].Split(':')[1].Trim();
        }

        private bool IsValidEntry(string[] lines)
        {
            if (lines.Length < 4)
                return false;
            foreach (var line in lines)
            {
                if (!line.Contains(":"))
                    return false;
            }
            return true;
        }
    }

    public static class PnPutilInterface
    {
        public static List<DriverDetail> GetVMSpcDrivers()
        {
            List<DriverDetail> silverleafDriverDetails = new List<DriverDetail>();
            var driverDetails = GetDrivers();
            if (driverDetails != null)
            {
                foreach (var detail in driverDetails)
                {
                    if (detail.SignerName.Contains("SilverLeaf"))
                    {
                        silverleafDriverDetails.Add(detail);
                    }
                }
            }
            return silverleafDriverDetails;
        }

        public static void InstallDrivers()
        {
            DriverInstaller.InstallDrivers();
        }

        public static void DeleteDrivers()
        {
            var silverleafDrivers = GetVMSpcDrivers();
            if (silverleafDrivers != null && silverleafDrivers.Count < 3)
            {
                var message = "VMSpc will uninstall the drivers by their oem files: ";
                foreach (var driver in silverleafDrivers)
                {
                    message += $"{driver.PublishedName}, ";
                }
                message += "are you sure you want to proceed?";
                if (MessageBox.Show(message, "Delete Drivers", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (var driver in silverleafDrivers)
                    {
                        var result = BatchExecutor.ExecuteCommand($"pnputil -f -d {driver.PublishedName}", true);
                        MessageBox.Show(result.StandardOutput);
                        MessageBox.Show(result.StandardError);
                    }
                }
            }
        }

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool IsWow64Process(IntPtr hProcess, out bool Wow64Process);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool Wow64DisableWow64FsRedirection(out IntPtr OldValue);
        public static List<DriverDetail> GetDrivers()
        {
            bool bWow64 = false;
            IsWow64Process(Process.GetCurrentProcess().Handle, out bWow64);
            if (bWow64)
            {
                IntPtr OldValue = IntPtr.Zero;
                bool bRet = Wow64DisableWow64FsRedirection(out OldValue);
            }
            var output = BatchExecutor.ExecuteCommand("pnputil -e");
            var entries = new List<DriverDetail>();
            var splitOutput = output.StandardOutput.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var outputSegment in splitOutput)
            {
                if (outputSegment.StartsWith("Published name"))
                {
                    var entry = new DriverDetail(outputSegment);
                    entries.Add(entry);
                }
            }
            return entries;
        }
    }
}
