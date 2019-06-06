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
        public List<byte> rawData;
        protected string rawMessage;

        public CanMessage(string message)
        {
            rawData = new List<byte>();
            rawMessage = message;
        }

        public abstract void ExtractMessage(string message);
        public override abstract string ToString();

    }

    public class J1939Message : CanMessage
    {
        public byte address;
        public uint pgn;

        public J1939Message(string message) : base(message)
        {

        }

        /// <summary>
        /// Extracts the address, pgn, and array of data bytes from the message and stores the results in the J1939Message instance
        /// </summary>
        public override void ExtractMessage(string message)
        {
            try
            {
                //J1939 Message comes in the format "R[SA:2 (index 1-2)][PGN:6 (index 3-8)][Data:16 (index 9-24)][cr][lf]"
                messageType = Constants.J1939;
                address = Convert.ToByte(message.Substring(1, 2));
                pgn = Convert.ToUInt32(message.Substring(3, 6));
                string dataSection = message.Substring(9, 16);
                bool dataStored = Constants.ByteStringToByteArray(ref rawData, dataSection, dataSection.Length);
                if (!dataStored)
                    messageType = Constants.INVALID_CAN_MESSAGE;
            }
            catch
            {
                messageType = Constants.INVALID_CAN_MESSAGE;
            }
        }

        public override string ToString()
        {
            string message;
            message = "";
            return message;
        }
    }

    public class J1708Message : CanMessage
    {
        public int messageLength;
        public Dictionary<byte, byte[]> data; 
        public J1708Message(string message) : base(message)
        {
            ExtractMessage(message);
        }

        /// <summary>
        /// Extracts the array of data bytes and stores them in the J1708Message instance
        /// </summary>
        /// <param name="message"></param>
        public override void ExtractMessage(string message)
        {
            try
            {
                //J1708 Message comes in the format "J[data:2n][checksum:2][cr][lf]"
                messageType = Constants.J1708;
                messageLength = message.Length;
                bool dataStored = Constants.ByteStringToByteArray(ref rawData, message, messageLength);
                if (dataStored)
                    data = SplitMessageByPID(rawData);
                else
                    messageType = Constants.INVALID_CAN_MESSAGE;
            }
            catch
            {
                messageType = Constants.INVALID_CAN_MESSAGE;
            }
        }

        public Dictionary<byte, byte[]> SplitMessageByPID(List<byte> data)
        {
            Dictionary<byte, byte[]> MessageList = new Dictionary<byte, byte[]>();
            int pos = 1;
            int bytesUnprocessed = messageLength;
            bool done = false;
            while (!done)
            {
                int indexToCopyTo = 0;
                byte pid = data[pos];
                switch (pid)
                {
                    case PIDs.retarderSwitch:            
                    case PIDs.retarderOilPressure:       
                    case PIDs.retarderOilTemp:           
                    case PIDs.retarderStatus:            
                    case PIDs.percentAcceleratorPosition:
                    case PIDs.voltage:                   
                    case PIDs.cruiseSpeed:               
                    case PIDs.coolantTemp:               
                    case PIDs.engineLoad:                
                    case PIDs.intakeTemp:                
                    case PIDs.oilPSI:                    
                    case PIDs.retarderPercent:           
                    case PIDs.roadSpeed:                 
                    case PIDs.turboBoost:
                    case PIDs.cruiseSetStatus:
                        indexToCopyTo = 2;
                        break;
                          
                    case PIDs.oilTemp:          
                    case PIDs.engineSpeed:      
                    case PIDs.transmissionTemp: 
                    case PIDs.transmissionSpeed:
                    case PIDs.fuelRate:         
                    case PIDs.fuelTemp:         
                    case PIDs.instantMPG:       
                    case PIDs.airInletTemp:
                    case PIDs.rangeSelected:
                    case PIDs.rangeAttained:
                        indexToCopyTo = 3;
                        break;

                    case PIDs.totalMiles:
                    case PIDs.totalMilesCummins:
                    case PIDs.engineHours:
                    case PIDs.totalFuel:
                        indexToCopyTo = 6;
                        break;

                    case PIDs.multipartMessage:
                        done = true;
                        indexToCopyTo = 0;
                        break;
                }
                byte[] pidData = new byte[6];
                int j = 0;
                int i = pos + 1;
                while (i < (pos + indexToCopyTo))
                {
                    pidData[j] = data[i];
                    i++;
                    j++;
                }
                pos += indexToCopyTo;
                bytesUnprocessed -= indexToCopyTo;

                if (j > 0)
                    MessageList.Add(pid, pidData);
                if (bytesUnprocessed < 2 || bytesUnprocessed > 22)
                    done = true;
            }
            return MessageList;
        }

        public override string ToString()
        {
            string message = String.Copy(rawMessage);
            message += ":";
            foreach (var pidItem in data)
            {
                message += "\n\t" + pidItem.Key.PidToString() + ":\t" + pidItem.Value;
            }
            return message;
        }
    }
}