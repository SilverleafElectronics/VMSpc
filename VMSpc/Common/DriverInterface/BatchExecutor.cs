using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Common.DriverInterface
{
    public static class BatchExecutor
    {
        public static BatchOutput ExecuteCommand(string command, bool elevated=false)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            
            if (elevated)
            {
                processInfo.Verb = "runas";
            }

            process = Process.Start(processInfo);

            string output = string.Empty;
            string error = string.Empty;

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => output += (e.Data + "\n");
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => error += (e.Data + "\n");
            process.BeginOutputReadLine();

            process.WaitForExit();

            var batchOutput = new BatchOutput(output, error, process.ExitCode);
            process.Close();

            return batchOutput;
        }
    }

    public class BatchOutput
    {
        public readonly string StandardOutput;
        public readonly string StandardError;
        public readonly int ExitCode;
        public BatchOutput(string StandardOutput, string StandardError, int ExitCode)
        {
            this.StandardOutput = StandardOutput;
            this.StandardError = StandardError;
            this.ExitCode = ExitCode;
        }
    }
}
