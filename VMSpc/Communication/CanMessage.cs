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
        public int messageLength;
        public byte messageType;
        public List<byte> rawData;
        protected string rawMessage;

        public CanMessage(string message)
        {
            rawData = new List<byte>();
            //messageLength and rawMessage account for dropping the J or R from the message
            messageLength = message.Length - 1;
            rawMessage = message.Substring(1, messageLength - 2);
        }

        public abstract void ExtractMessage(string message);
        public override abstract string ToString();
        public abstract void PrintMessage();

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
                address = Convert.ToByte(rawMessage.Substring(0, 2));
                pgn = Convert.ToUInt32(rawMessage.Substring(2, 6));
                string dataSection = rawMessage.Substring(8, 16);
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

        public override void PrintMessage()
        {

        }
    }

    public class J1708Message : CanMessage
    {
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
                bool dataStored = Constants.ByteStringToByteArray(ref rawData, rawMessage, messageLength);
                if (dataStored)
                    data = SplitMessageByPID(rawData);
                else
                    messageType = Constants.INVALID_CAN_MESSAGE;
            }
            catch (Exception ex)
            {
                messageType = Constants.INVALID_CAN_MESSAGE;
            }
        }

        public Dictionary<byte, byte[]> SplitMessageByPID(List<byte> data)
        {
            Dictionary<byte, byte[]> MessageList = new Dictionary<byte, byte[]>();
            int pos = 1;
            int bytesUnprocessed = (messageLength / 2) - 1;
            bool done = false;
            while (!done)
            {
                try
                {
                    int indexToCopyTo = 0;
                    byte pid = data[pos];
                    bool continueFlag = false;
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
                            break;
                        default:
                            if (pid < 0x80 && bytesUnprocessed > 1)
                            {
                                indexToCopyTo = 2;
                                continueFlag = true;
                            }
                            else if (pid < 0xC0 && bytesUnprocessed > 2)
                            {
                                indexToCopyTo = 3;
                                continueFlag = true;
                            }
                            else
                                done = true;
                            break;
                    }
                    byte[] pidData = new byte[6];
                    int j = 0;
                    int i = pos + 1;
                    if (!continueFlag)
                    {
                        while (i < (pos + indexToCopyTo))
                        {
                            pidData[j] = data[i];
                            i++;
                            j++;
                        }
                    }
                    pos += indexToCopyTo;
                    bytesUnprocessed -= indexToCopyTo;

                    if (j > 0)
                        MessageList.Add(pid, pidData);
                    if (bytesUnprocessed < 2 || bytesUnprocessed > 22)
                        done = true;
                }
                catch (Exception ex)
                {
                    messageType = Constants.INVALID_CAN_MESSAGE;
                    return null;
                }
            }
            return MessageList;
        }

        public override string ToString()
        {
            string message = String.Copy(rawMessage);
            message += ":";
            foreach (var pidItem in data)
            {
                string byteString = "";
                foreach (var item in pidItem.Value)
                    byteString += item;
                message += "\n\t" + pidItem.Key.PidToString() + ":\t" + byteString;
            }
            return message;
        }

        public override void PrintMessage()
        {
            string message = String.Copy(rawMessage);
            message += ":";
            VMSConsole.PrintLine(message);
            foreach (var pidItem in data)
            {
                string byteString = "";
                foreach (var item in pidItem.Value)
                    byteString += item;
                VMSConsole.PrintLine("\t" + pidItem.Key.PidToString() + ":\t" + byteString);
            }
        }
    }
}