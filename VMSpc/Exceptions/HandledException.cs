using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Exceptions
{
    /// <summary>
    /// Handled exception, which should be thrown by all catch { } blocks in the application. This exception class wraps
    /// whichever exception is caught
    /// </summary>
    public class HandledException : Exception
    {
        public Exception exception { get; set; }
        public HandledException(Exception exception)
            :base("Handled Exception")
        {
            this.exception = exception;
        }
    }
}
