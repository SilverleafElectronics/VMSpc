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
        public string message;
        /// <summary>
        /// If true, the data can be published as is. Otherwise, additional parsing is necessary.
        /// </summary>
        public bool Parsed { get; set; }

        public List<CanMessageSegment> CanMessageSegments { get; private set; }

        public VMSDataSource VMSDataSource;

        public CanMessage(string message, VMSDataSource VMSDataSource, DateTime timeReceived)
        {
            this.message = message;
            rawMessage = message.Substring(0, message.Length - 2);  //removes the checksum from the end of the message
            messageLength = rawMessage.Length; 
            this.VMSDataSource = VMSDataSource;
            CanMessageSegments = new List<CanMessageSegment>();
            this.timeReceived = timeReceived;
        }

        public abstract void ExtractMessage();
        public override abstract string ToString();
        public abstract string ToParsedString(object parser);
        public virtual bool IsValidMessage()
        {
            return false;
        }

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

        public static bool IsValidMessage(string message)
        {
            return true;
        }

        /// <summary>
        /// Extracts the address, pgn, and array of data bytes from the message and stores the results in the J1939Message instance
        /// </summary>
        public override void ExtractMessage()
        {
            bool dataStored;
            try
            {
                //J1939 Message comes in the format "R[SA:2 (index 1-2)][PGN:6 (index 3-8)][Data:16 (index 9-24)][cr][lf]"
                messageType = J1939;
                address = Convert.ToByte(rawMessage.Substring(0, 2), 16);   //Message byte 0: SA
                pgn = Convert.ToUInt32(rawMessage.Substring(2, 6), 16);     //Message bytes 2-6: PGN
                string dataSection = rawMessage.Substring(8);           //Message bytes 7 - end: data section
                dataStored = BYTE_STRING_TO_BYTE_ARRAY(data, dataSection, dataSection.Length);
            }
            catch (Exception ex)
            {
                dataStored = false;
            }
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

        public static bool IsValidMessage(string message)
        {
            return true;
        }

        /// <summary>
        /// Extracts the array of data bytes and stores them in the J1708Message instance.
        /// </summary>
        /// <param name="message"></param>
        public override void ExtractMessage()
        {
            try
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
            catch (Exception ex)
            {
                VMSConsole.PrintLine("Failed to parse " + message);
            }
        }

        /// <summary>
        /// Returns a stringified CanMessage in the format "`Raw Message`:\n\t`PID[i-n]`:\t`data`". Each Raw Message will display `n` PID/data pairs"
        /// </summary>
        public override string ToString()
        {
            return string.Empty;
        }

        public override string ToParsedString(object parser)
        {
            return string.Empty;
        }
    }
    #endregion J1708Message
}