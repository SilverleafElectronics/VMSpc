using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using VMSpc.DevHelpers;
using System.Windows;
using System.Timers;
using static VMSpc.Parsers.PIDWrapper;
using static VMSpc.Constants;
using VMSpc.Parsers;
using static VMSpc.Parsers.PGNMapper;
using static VMSpc.Parsers.PresenterWrapper;

namespace VMSpc.Communication
{
    #region CanMessage
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
            rawMessage = message.Substring(1, messageLength);
        }

        public abstract void ExtractMessage();
        public override abstract string ToString();
        public abstract string ToParsedString(object parser);

    }
    #endregion

    #region J1939Message
    public class J1939Message : CanMessage
    {
        public byte address;
        public uint pgn;
        public byte[] data;

        public J1939Message(string message)
            : base(message)
        {
            data = new byte[8];
            ExtractMessage();
        }

        /// <summary>
        /// Extracts the address, pgn, and array of data bytes from the message and stores the results in the J1939Message instance
        /// </summary>
        public override void ExtractMessage()
        {
            try
            {
                //J1939 Message comes in the format "R[SA:2 (index 1-2)][PGN:6 (index 3-8)][Data:16 (index 9-24)][cr][lf]"
                messageType = J1939;
                address = Convert.ToByte(rawMessage.Substring(0, 2), 16);
                pgn = Convert.ToUInt32(rawMessage.Substring(2, 6), 16);
                string dataSection = rawMessage.Substring(8, 16);
                bool dataStored = BYTE_STRING_TO_BYTE_ARRAY(data, dataSection, dataSection.Length);
                if (!dataStored)
                    messageType = INVALID_CAN_MESSAGE;
            }
            catch (Exception ex)
            {
                //VMSConsole.PrintLine("ERROR: " + ex.Message);
                //VMSConsole.PrintLine(rawMessage);
                messageType = INVALID_CAN_MESSAGE;
            }
        }

        public override string ToString()
        {
            string message;
            message = ((messageType == J1939) ? "\nJ1939 Message:\n" : "\nINVALID J1939 MESSAGE:\n")
                      + "Address - " + address.ToString("X2")
                      + "\nPGN - " + pgn.ToString("X2")
                      + "\nData - [" + BitConverter.ToString(data).Replace("-", ",") + "]\n";
            return message;
        }

        public override string ToParsedString(object parser)
        {
            try
            {
                J1939Parser j1939 = (J1939Parser)parser;
                j1939.Parse(this);
                string message = "J1939 Parsed Message\n";
                foreach (TSPNDatum datum in PGNMap[pgn])
                {
                    try
                    {
                        message += PresenterList[datum.spn].ToString() + "\n";
                    }
                    catch
                    {
                        message += "No definition for SPN " + datum.spn + "\n";
                    }
                }
                    return message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.ToString();
            }
        }
    }
    #endregion //J1939Message


    #region J1708Message
    public class J1708Message : CanMessage
    {
        public Dictionary<byte, byte[]> data; 
        public J1708Message(string message) : base(message)
        {
            ExtractMessage();
        }

        /// <summary>
        /// Extracts the array of data bytes and stores them in the J1708Message instance
        /// </summary>
        /// <param name="message"></param>
        public override void ExtractMessage()
        {
            try
            {
                //J1708 Message comes in the format "J[data:2n][checksum:2][cr][lf]"
                messageType = J1708;
                bool dataStored = BYTE_STRING_TO_BYTE_ARRAY(ref rawData, rawMessage, messageLength);
                if (dataStored)
                    data = SplitMessageByPID();
                else
                    messageType = INVALID_CAN_MESSAGE;
            }
            catch (Exception ex)
            {
                messageType = INVALID_CAN_MESSAGE;
            }
        }

        /// <summary>
        /// Splits a J1708 Message into a Dictionary of PIDs and their corresponding data segments (stored as a byte array)
        /// </summary>
        public Dictionary<byte, byte[]> SplitMessageByPID()
        {
            Dictionary<byte, byte[]> MessageList = new Dictionary<byte, byte[]>();
            int pos = 1;
            int bytesUnprocessed = (messageLength / 2) - 1;
            bool done = false;
            while (!done)
            {
                int indexToCopyTo = 0;
                if (pos >= rawData.Count)
                    break;
                byte pid = rawData[pos];
                bool continueFlag = false;
                if (PIDManager.PIDList.ContainsKey(pid))
                    indexToCopyTo = PIDManager.PIDList[pid].NumDataBytes;
                else
                {
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
                }
                SavePid(MessageList, pid, continueFlag, pos, indexToCopyTo);
                pos += indexToCopyTo;
                bytesUnprocessed -= indexToCopyTo;
                if (bytesUnprocessed < 2 || bytesUnprocessed > 22)
                    done = true;
            }
            return MessageList;
        }

        /// <summary>
        /// Saves a key/value pair (pid/data[]) to the MessageList Dictionary. Ignores an element if the data is empty
        /// </summary>
        private void SavePid(Dictionary<byte, byte[]> MessageList, byte pid, bool continueFlag, int pos, int indexToCopyTo)
        {
            byte[] pidData = new byte[6];
            int j = 0;
            int i = pos + 1;
            if (!continueFlag)
            {
                while (i < (pos + indexToCopyTo))
                {
                    pidData[j] = rawData[i];
                    i++;
                    j++;
                }
            }
            if (j > 0)
                MessageList.Add(pid, pidData);
        }

        /// <summary>
        /// Returns a stringified CanMessage in the format "`Raw Message`:\n\t`PID[i-n]`:\t`data`". Each Raw Message will display `n` PID/data pairs"
        /// </summary>
        public override string ToString()
        {
            string message = (messageType == J1708) ? "\nJ1708 Message:\n" : "\nINVALID J1708 MESSAGE\n";
            foreach (var pidItem in data)
            {
                int tabCount = 6 - ((pidItem.Key.PidToString().Length + 1) / 4);
                string tabs = "";
                for (int i = 0; i < tabCount; i++) tabs += "\t";
                message += ("\t" + pidItem.Key.PidToString() + ":" + tabs + "[" + BitConverter.ToString(pidItem.Value).Replace('-', ',') + "]\n");
            }
            return message;
        }

        public override string ToParsedString(object parser)
        {
            try
            {
                J1708Parser j1708 = (J1708Parser)parser;
                j1708.Parse(this);
                string message = "J1708 Parsed Message\n";
                foreach (var pidItem in data)
                {
                    message += PresenterList[pidItem.Key].ToString() + "\n";
                }
                return message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.ToString();
            }
        }
    }
    #endregion //J1708Message
}