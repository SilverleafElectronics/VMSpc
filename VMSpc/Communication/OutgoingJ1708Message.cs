using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Enums.Parsing;

namespace VMSpc.Communication
{
    public abstract class OutgoingMessage
    {
        public DataBusType DataBusType;
        /// <summary>
        /// Customizable prefix character that gets sent out on USB connections. Defaults to the default values for J1708('J') and J1939('R');
        /// </summary>
        public char USBPrefix { get; set; }
        public abstract byte[] ToByteArray();
        public abstract string ToString(bool useUSBPrefix);
    }
    public class OutgoingJ1708Message : OutgoingMessage
    {
        public byte Mid;
        public byte Pid;
        public List<byte> Data;
        private byte checkSum;
        public byte CheckSum { get; set; }
        public OutgoingJ1708Message()
        {
            DataBusType = DataBusType.J1708;
            USBPrefix = 'J';
        }
        public void GenerateChecksum()
        {
            CheckSum = 0;
            CheckSum += Mid;
            CheckSum += Pid;
            foreach (var value in Data)
                CheckSum += value;
        }
        public override byte[] ToByteArray()
        {
            var length = 3 + Data.Count;
            byte[] message = new byte[length];
            message[0] = Mid;
            message[1] = Pid;
            for (int i = 2, j = 0; j < Data.Count; i++, j++)
            {
                message[i] = Data[j];
            }
            message[length - 1] = CheckSum;
            return message;
        }

        public override string ToString(bool useUSBPrefix)
        {
            throw new NotImplementedException();
        }
    }

    public class OutgoingJ1939Message : OutgoingMessage
    {
        public byte SA { get; set; }
        public uint PGN { get; set; }
        public byte[] Data { get; set; }
        public OutgoingJ1939Message()
            :base()
        {
            DataBusType = DataBusType.J1939;
            Data = new byte[] { 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF };
            USBPrefix = 'R';
        }

        public override byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        public override string ToString(bool useUSBPrefix = false)
        {
            StringBuilder builder = new StringBuilder();
            if (useUSBPrefix)
            {
                builder.Append(USBPrefix);
            }
            builder.Append(string.Format("{0:X2}", SA));
            builder.Append(string.Format("{0:X6}", PGN));
            foreach (var value in Data)
            {
                builder.Append(string.Format("{0:X2}", value));
            }
            return builder.ToString();
        }
    }
}
