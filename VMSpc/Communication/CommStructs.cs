using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using VMSpc.DevHelpers;
using System.Windows;
using System.Timers;

namespace VMSpc.Communication
{
    public abstract class CanMessage
    {
        public byte messageType;
        public byte address;
        public uint pgn;
        public byte pid;
        public byte[] data;

        public CanMessage() { }

        public abstract bool ExtractMessage(string message);

    }

    public class J1939Message : CanMessage
    {

        public J1939Message() : base() { }

        /// <summary>
        /// Extracts the address, pgn, and array of data bytes from the message and stores the results in the canMessage struct
        /// </summary>
        /// <param name="message"></param>
        /// <param name="canMessage"></param>
        public override bool ExtractMessage(string message)
        {
            try
            {
                //J1939 Message comes in the format "R[SA:2 (index 1-2)][PGN:6 (index 3-8)][Data:16 (index 9-24)][cr][lf]"
                messageType = Constants.J1939;
                address = Convert.ToByte(message.Substring(1, 2));
                pgn = Convert.ToUInt32(message.Substring(3, 6));
                data = new byte[8];
                string dataSection = message.Substring(9, 16);
                bool dataStored = Constants.ByteStringToByteArray(ref data, dataSection, dataSection.Length);
                if (!dataStored)
                    messageType = Constants.INVALID_ENGINE;
            }
            catch
            {
                messageType = Constants.INVALID_ENGINE;
            }
            return (messageType != Constants.INVALID_ENGINE);
        }

    }

    public class J1708Message : CanMessage
    { 
        public override bool ExtractMessage(string message)
        {
            try
            {
                //J1708 Message comes in the format "J[data:2n][checksum:2][cr][lf]"
                messageType = Constants.J1708;
                pid = Convert.ToByte(message.Substring(1, 2));
                data = new byte[8];
                for (int i = 0; i < 8; i++)
                    data[i] = 0;
            }
            catch
            {
                messageType = Constants.INVALID_ENGINE;
            }
            return (messageType != Constants.INVALID_ENGINE);
        }
    }
}