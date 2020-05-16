using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace VMSpc.Communication
{
    class SerialPortReader : DataReader
    {
        public SerialPortReader()
            : base()
        {
        }
        public override void InitDataReader()
        {
            base.InitDataReader();
            throw new NotImplementedException();
        }

        public override void CloseDataReader()
        {
            throw new NotImplementedException();
        }
        public override void SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(OutgoingMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
