using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.AdvancedParsers
{
    /// <summary>
    /// GUIParsers are classes that handle complex data. To avoid too much messy coupling, many complex
    /// values are fed through the J1708 and J1939 parsers without any data conversion.Once the values are
    /// broadcast by the message extractor, they can be caught by these GUIParsers to perform more advanced
    /// parsing.
    /// </summary>
    public abstract class AdvancedParser
    {
    }
}
