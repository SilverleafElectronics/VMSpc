using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Exceptions
{
    [Serializable]
    public class CannotReadEngineFileException : Exception
    {
        public CannotReadEngineFileException(string EngineFilePath, ushort[] readBuffer, uint bytesRead)
            : base(GetMessage(EngineFilePath, readBuffer, bytesRead))
        {
        }

        private static string GetMessage(string EngineFilePath, ushort[] readBuffer, uint bytesRead)
        {
            string concatenatedBuffer = "{" + string.Join(",", readBuffer) + "}";
            string message = $"Failed to properly load engine data from {EngineFilePath}. {bytesRead} bytes were read. Read Buffer: {concatenatedBuffer}";
            return message;
        }
    }
}
