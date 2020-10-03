using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;

namespace VMSpc.Exceptions
{
    public class RawDataParsingException : Exception
    {
        public RawDataParsingException(VMSCommDataEventArgs e, Exception ex)
            :base($"Could not parse the raw message \"{e.message}\"" + ex.Message)
        {

        }
    }
}
