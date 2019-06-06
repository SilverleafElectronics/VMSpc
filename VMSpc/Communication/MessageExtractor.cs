using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Communication
{
    /// <summary>
    /// Class for extracting the contents of the CAN message and storing them in a meaningful format. Both J1939 and J1708 messages are unpacked to the same object type
    /// </summary>
    internal class MessageExtractor
    {
        public MessageExtractor()
        {

        }

        /// <summary>
        /// Accepts a string representing a CAN message and returns a new J1939Message or J1708Message, with the fields extracted from the message
        /// </summary>
        public CanMessage GetMessage(string message)
        {
            
            CanMessage canMessage = FromString(message);
            return canMessage;
        }

        /// <summary>
        /// Determines the message type and calls the appropriate method for extracting the message properties from the provided string. Returns the CanMessage object
        /// </summary>
        public CanMessage FromString(string message)
        {
            try
            {
                switch (message[0])
                {
                    case Constants.J1939_HEADER:
                        return new J1939Message(message);
                    case Constants.J1939_STATUS_HEADER:
                        return new J1939Message(message);
                    case Constants.J1708_HEADER:
                        return new J1708Message(RemoveControlCharacters(message));
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the carriage return/line feed from the end of the message
        /// </summary>
        /// <param name="message"></param>
        private string RemoveControlCharacters(string message)
        {
            return message.Replace("\r", "").Replace("\n", "");
        }
    }
}
