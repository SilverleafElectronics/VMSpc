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
using VMSpc.Enums.Parsing;

namespace VMSpc.Parsers
{
    #region CanMessage
    public abstract class CanMessage
    {
        public int messageLength { get; set; }
        public byte messageType { get; set; }
        public DateTime timeReceived { get; private set; }
        public DateTime timeParsed { get; private set; }
        public string rawMessage { get; private set; }
        public List<byte> rawData { get; private set; }
        /// <summary>
        /// If true, the data can be published as is. Otherwise, additional parsing is necessary.
        /// </summary>
        public bool Parsed { get; set; }

        public List<CanMessageSegment> CanMessageSegments { get; private set; }

        public VMSDataSource VMSDataSource;

        public CanMessage(string message, VMSDataSource VMSDataSource, DateTime timeReceived)
        {
            rawData = new List<byte>();
            rawMessage = message.Substring(0, message.Length - 2);  //removes the checksum from the end of the message
            messageLength = rawMessage.Length; 
            this.VMSDataSource = VMSDataSource;
            CanMessageSegments = new List<CanMessageSegment>();
            this.timeReceived = timeReceived;
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

        public J1939Message(string message, DateTime timeReceived)
            : base(message, VMSDataSource.J1939, timeReceived)
        {
            data = new byte[8];
            ExtractMessage();
        }

        /// <summary>
        /// Extracts the address, pgn, and array of data bytes from the message and stores the results in the J1939Message instance
        /// </summary>
        public override void ExtractMessage()
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

        public override string ToString()
        {
            return null;
        }
        
        public override string ToParsedString(object parser)
        {
            return string.Empty;
            /*
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
            */
        }
        
    }
    #endregion J1939Message

    #region J1708Message
    public class J1708Message : CanMessage
    {
        public static byte ENGINE_HEADER => 0x80;
        public static byte TRANSMISSION_HEADER => 0x82;
        public static byte ABS_HEADER => 0x88;

        public byte Mid { get; private set; }
        public ushort Pid { get; private set; }

        public J1708Message(string message, DateTime timeReceived) 
            : base(message, VMSDataSource.J1708, timeReceived)
        {
            ExtractMessage();
        }

        /// <summary>
        /// Extracts the array of data bytes and stores them in the J1708Message instance.
        /// </summary>
        /// <param name="message"></param>
        public override void ExtractMessage()
        {
            Mid = BinConvert(rawMessage[0], rawMessage[1]);
            var trimmedMessage = rawMessage.Substring(2);   //remove the MID from the message
            while (trimmedMessage.Length > 0)
            {
                var segment = new J1708MessageSegment(trimmedMessage, Mid)
                {
                    TimeReceived = this.timeReceived
                };
                int messageLength = segment.GetTotalMessageBytes() * 2;
                if (messageLength >= trimmedMessage.Length)
                    trimmedMessage = string.Empty;
                else
                    trimmedMessage = trimmedMessage.Substring(messageLength);
                CanMessageSegments.Add(segment);
            }
        }

        /// <summary>
        /// Returns a stringified CanMessage in the format "`Raw Message`:\n\t`PID[i-n]`:\t`data`". Each Raw Message will display `n` PID/data pairs"
        /// </summary>
        public override string ToString()
        {
            return string.Empty;
            /*
            string message = (messageType == J1708) ? "\nJ1708 Message:\n" : "\nINVALID J1708 MESSAGE\n";
            foreach (var pidItem in data)
            {
                int tabCount = 6 - ((pidItem.Key.PidToString().Length + 1) / 4);
                string tabs = "";
                for (int i = 0; i < tabCount; i++) tabs += "\t";
                message += ("\t" + pidItem.Key.PidToString() + ":" + tabs + "[" + BitConverter.ToString(pidItem.Value).Replace('-', ',') + "]\n");
            }
            return message;
            */
        }

        public override string ToParsedString(object parser)
        {
            return string.Empty;
            /*
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
            */
        }
    }
    #endregion J1708Message
}