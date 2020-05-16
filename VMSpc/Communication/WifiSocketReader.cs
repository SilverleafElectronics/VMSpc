using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Sockets;

namespace VMSpc.Communication
{
    class WifiSocketReader : DataReader
    {
        Socket wifiReader;
        public WifiSocketReader()
            : base()
        {
            wifiReader = new Socket(SocketType.Stream, ProtocolType.Tcp);//TODO
        }
        public override void InitDataReader()
        {
            base.InitDataReader();
        }

        public override void CloseDataReader()
        {
            wifiReader.Shutdown(SocketShutdown.Both);
            wifiReader.Close();
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
