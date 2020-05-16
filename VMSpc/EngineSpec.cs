using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using VMSpc.Exceptions;

namespace VMSpc
{
    public static class EngineSpec
    {
        private static ushort[] RPMMap = new ushort[12];
        private static ushort[] TorqueMap     = new ushort[12];
        private static ushort[] HorsepowerMap = new ushort[12];

        private static bool IsNewEngine = false;

        private static double
            currentRPMs,
            currentLoadPercent,
            currentTorque,
            currentHorsepower;

        private const uint GenericRead = 0x80000000;
        private const uint FileShareRead = 1;
        private const uint FileShareWrite = 2;
        private const uint OpenExisting = 3;
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(IntPtr hFile, [Out] ushort[] lpBuffer,
           uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        public static void SetEngineFile(string EngineFilePath)
        {
            IsNewEngine = true;
            IntPtr ptr = CreateFile(EngineFilePath,
                GenericRead, FileShareRead | FileShareWrite, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero);

            SafeFileHandle handleValue = new SafeFileHandle(ptr, true);
            FileStream fileStream = new FileStream(handleValue, FileAccess.Read);

            const uint numberOfBytesToRead = 72;

            ushort[] buffer = new ushort[numberOfBytesToRead / 2];
            if (!ReadFile(handleValue.DangerousGetHandle(), buffer, numberOfBytesToRead, out uint bytesRead, IntPtr.Zero))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            if (bytesRead < 72)
            {
                throw new CannotReadEngineFileException(EngineFilePath, buffer, bytesRead);
            }
            for (int i = 0; i < 12; i++)
            {
                RPMMap[i] = buffer[i];
                TorqueMap[i] = buffer[i + 12];
                HorsepowerMap[i] = buffer[i + 24];
            }
        }

        private static bool IsValidRPM(double rpm)
        {
            return ((rpm >= RPMMap[0]) || (rpm <= RPMMap[11]));
        }

        public static double CalculateTorque(double rpm, double loadPercent)
        {
            CalculateNewValues(rpm, loadPercent);
            return currentTorque;
        }

        public static double CalculateHorsepower(double rpm, double loadPercent)
        {
            CalculateNewValues(rpm, loadPercent);
            return currentHorsepower;
        }

        private static void CalculateNewValues(double rpm, double loadPercent)
        {
            if (!IsNewValues(rpm, loadPercent))
            {
                return;
            }
            else
            {
                currentRPMs = rpm;
                currentLoadPercent = loadPercent;
                var mapIndex = GetHighRpmIndex(rpm);
                ushort highRPM = RPMMap[mapIndex];
                ushort lowRPM = RPMMap[mapIndex - 1];
                ushort highTorque = TorqueMap[mapIndex];
                ushort lowTorque = TorqueMap[mapIndex - 1];
                ushort highHP = HorsepowerMap[mapIndex];
                ushort lowHP = HorsepowerMap[mapIndex - 1];
                currentTorque = InferFromMappedValues(lowTorque, highTorque, lowRPM, highRPM);
                currentHorsepower = InferFromMappedValues(lowHP, highHP, lowRPM, highRPM);
            }
        }

        private static double InferFromMappedValues(ushort low, ushort high, ushort lowRPM, ushort highRPM)
        {
            return currentLoadPercent * 0.01d * (low + ((currentRPMs - lowRPM) * (high - low) / (highRPM - lowRPM)));
        }

        private static bool IsNewValues(double rpm, double loadPercent)
        {
            if (IsNewEngine)
            {
                IsNewEngine = false;
                return true;
            }
            return (rpm != currentRPMs) && (loadPercent != currentLoadPercent);
        }

        private static int GetHighRpmIndex(double rpm)
        {
            if (rpm < RPMMap[0] || rpm > RPMMap[11])
            {
                return 1;
            }
            for (int i = 1; i < 12; i++)
            {
                if (RPMMap[i] <= rpm)
                    return i;
            }
            return 0;
        }
    }
}
