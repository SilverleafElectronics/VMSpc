using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using VMSpc.DevHelpers;
using VMSpc.Exceptions;

namespace VMSpc.Loggers
{
    public static class ErrorLogger
    {
        public static void GenerateErrorRecord(Exception e)
        {
            VMSConsole.PrintLine(e.ToString());
        }
    }
}
