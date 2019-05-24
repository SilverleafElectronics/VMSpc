using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace VMSpc
{
    public class VMSComm
    {
        //class variables
        private SerialPort portreader;
        public VMSComm()
        {
            portreader = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        }
        public void process()
        {

        }
    }
}
