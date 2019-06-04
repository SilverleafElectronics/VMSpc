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
    public struct CanMessage
    {
        public byte engineType;
        public byte address;
        public uint pgn;
        public byte pid;
        public byte[] data;

        /// <summary>
        /// Determines the message type and calls the appropriate method for extracting the message properties from the provided string. Returns true if the message was successfully extracted
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool FromString(string message)
        {
            switch (message[0])
            {
                case Constants.J1939_HEADER:
                    return ExtractJ1939(message);
                case Constants.J1939_STATUS_HEADER:
                    return true;    //CHANGEME
                case Constants.J1708_HEADER:
                    return ExtractJ1708(message);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Extracts the address, pgn, and array of data bytes from the message and stores the results in the canMessage struct
        /// </summary>
        /// <param name="message"></param>
        /// <param name="canMessage"></param>
        private bool ExtractJ1939(string message)
        {
            try
            {
                //J1939 Message comes in the format "R[SA:2 (index 1-2)][PGN:6 (index 3-8)][Data:16 (index 9-24)][cr][lf]"
                engineType = Constants.J1939;
                address = Convert.ToByte(message.Substring(1, 2));
                pgn = Convert.ToUInt32(message.Substring(3, 6));
                data = new byte[8];
                string dataSection = message.Substring(9, 16);
                bool dataStored = Constants.ByteStringToByteArray(ref data, dataSection, dataSection.Length);
                if (!dataStored)
                    engineType = Constants.INVALID_ENGINE;
            }
            catch
            {
                engineType = Constants.INVALID_ENGINE;
            }
            return (engineType != Constants.INVALID_ENGINE);
        }

        private bool ExtractJ1708(string message)
        {
            try
            {
                //J1708 Message comes in the format "J[data:2n][checksum:2][cr][lf]"
                engineType = Constants.J1708;
                pid = Convert.ToByte(message.Substring(1, 2));
                data = new byte[8];
                for (int i = 0; i < 8; i++)
                    data[i] = 0;
            }
            catch
            {
                engineType = Constants.INVALID_ENGINE;
            }
            return (engineType != Constants.INVALID_ENGINE);
        }

    }
}