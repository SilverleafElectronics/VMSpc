using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Communication;

namespace VMSpc.Parsers
{
    public interface IDataBus
    {
        void Parse(CanMessage message);
        void SendMessage(byte[] message);
    }
}
