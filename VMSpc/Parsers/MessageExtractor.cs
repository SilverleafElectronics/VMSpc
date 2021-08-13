using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Enums.Parsing;
using VMSpc.Exceptions;
using VMSpc.Loggers;
using static VMSpc.Constants;

namespace VMSpc.Parsers
{
    /// <summary>
    /// Class for extracting the contents of the CAN message and storing them in a meaningful format. Both J1939 and J1708 messages are unpacked to the same object type
    /// </summary>
    public sealed class CanMessageHandler : IEventConsumer, IEventPublisher, ISingleton
    {
        public event EventHandler<VMSEventArgs> RaiseVMSEvent;
        private readonly J1708Parser _J1708Parser;
        private readonly J1939Parser _J1939Parser;

        public static CanMessageHandler Instance { get; private set; }
        static CanMessageHandler() {}
        private CanMessageHandler()
        {
            EventBridge.Instance.SubscribeToEvent(this, EventIDs.NEW_COMM_DATA_EVENT);
            EventBridge.Instance.AddEventPublisher(this);
            _J1708Parser = new J1708Parser();
            _J1939Parser = new J1939Parser();
        }

        public static void Initialize()
        {
            Instance = new CanMessageHandler();
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            try
            {
                HandleMessage((e as VMSCommDataEventArgs));
            }
            catch (Exception ex)
            {
                throw new RawDataParsingException((e as VMSCommDataEventArgs), ex);
            }
        }

        private void HandleMessage(VMSCommDataEventArgs e)
        {
            var message = RemoveControlCharacters(e.message);
            var type = GetMessageType(message);
            message = message.Substring(1); //remove the type indicator from the message ('J', 'R', or 'I')
            switch (type)
            {
                case VMSDataSource.J1708:
                    ProcessJ1708Data(message, e);
                    break;
                case VMSDataSource.J1939:
                    ProcessJ1939Data(message, e);
                    break;
                case VMSDataSource.None:
                    break;
            }
        }

        /// <summary>
        /// Istantiates a J1708 can message from the raw message. The J1708 Can Message handles it's own
        /// raw value extraction, which is then passed onto the J1708Parser to get standard and metric values
        /// </summary>
        /// <param name="rawMessage"></param>
        /// <param name="e"></param>
        private void ProcessJ1708Data(string rawMessage, VMSCommDataEventArgs e)
        {
            try
            {
                var canMessage = new J1708Message(rawMessage, e.timeStamp);
                if (canMessage.IsValidMessage())
                {
                    canMessage.ExtractMessage();
                    _J1708Parser.Parse(canMessage);
                    PublishNewDataEvent(canMessage);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {   //Todo - trim this down to only invoke CommDataErrorEvent on specific exceptions (ie, from CanMesssage or parser)
                RaiseVMSEvent?.Invoke(this, new VMSCommDataErrorEventArgs(e, MessageError.UnrecognizedMessage, ex));
                ErrorLogger.GenerateErrorRecord(ex);
            }
        }

        /// <summary>
        /// Instantiates a J1939 can message from the raw message, then passes the message to the J1939 parser,
        /// which handles both extraction and parsing
        /// </summary>
        /// <param name="rawMessage"></param>
        /// <param name="e"></param>
        private void ProcessJ1939Data(string rawMessage, VMSCommDataEventArgs e)
        {
            try
            {
                var canMessage = new J1939Message(rawMessage, e.timeStamp);
                if (canMessage.IsValidMessage())
                {
                    canMessage.ExtractMessage();
                    _J1939Parser.Parse(canMessage);
                    PublishNewDataEvent(canMessage);
                }
            }
            catch (Exception ex)
            {
                RaiseVMSEvent?.Invoke(this, new VMSCommDataErrorEventArgs(e, MessageError.UnrecognizedMessage, ex));
                ErrorLogger.GenerateErrorRecord(ex);
            }
        }

        private void PublishNewDataEvent(CanMessage message)
        {
            foreach (var segment in message.CanMessageSegments)
            {
                if (segment.ParseStatus == ParseStatus.Parsed)
                {
                    segment.TimeParsed = DateTime.Now;
                    var e = new VMSParsedDataEventArgs(segment.Pid, segment);
                    RaiseVMSEvent?.Invoke(this, e);
                }
                else
                {
                    PublishUnparsedData(segment);
                }
            }
        }

        public void PublishUnparsedData(CanMessageSegment segment)
        {
            VMSRawDataEventArgs e;
            switch (segment.DataSource)
            {
                case VMSDataSource.J1708:
                    e = new VMSJ1708RawDataEventArgs(segment as J1708MessageSegment);
                    RaiseVMSEvent?.Invoke(this, e);
                    break;
                case VMSDataSource.J1939:
                    e = new VMSJ1939RawDataEventArgs(segment as J1939MessageSegment);
                    RaiseVMSEvent?.Invoke(this, e);
                    break;
            }
        }

        /*
        /// <summary>
        /// Determines the message type and calls the appropriate method for extracting the message properties from the provided string. Returns the CanMessage object
        /// </summary>
        public CanMessage FromString(string message)
        {

            try
            {
                switch (message[0])
                {
                    case J1939_HEADER:
                        return new J1939Message(message);
                    case J1939_STATUS_HEADER:
                        return null;//new J1939Message(message);
                    case J1708_HEADER:
                        return new J1708Message(message, DateTime.Now);
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }
        */

        /// <summary>
        /// Removes the carriage return/line feed from the end of the message
        /// </summary>
        /// <param name="message"></param>
        private static string RemoveControlCharacters(string message)
        {
            if (string.IsNullOrEmpty(message))
                return string.Empty;
            try
            {
               var ret = message
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Trim();
                return ret;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static VMSDataSource GetMessageType(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return VMSDataSource.None;
            }
            switch (message[0])
            {
                case 'J':
                    return VMSDataSource.J1708;
                case 'R':
                    return VMSDataSource.J1939;
                default:
                    return VMSDataSource.None;
            }
        }

        private void ReportMessageError(MessageError messageError, string message)
        {

        }
    }
}
