using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.JsonFileManagers
{
    public class RawLogOpener : FileOpener
    {
        public static string RawLogBaseDirectory => BaseDirectory + "\\rawlogs";
        public RawLogOpener(string rawLogFileName) : base(rawLogFileName)
        {
        }
    }
}
