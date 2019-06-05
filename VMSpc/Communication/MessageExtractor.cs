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

        public CanMessage GetMessage(string message)
        {
            CanMessage canMessage = FromString(message);

            return canMessage;
        }

        /// <summary>
        /// Determines the message type and calls the appropriate method for extracting the message properties from the provided string. Returns true if the message was successfully extracted
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public CanMessage FromString(string message)
        {
            switch (message[0])
            {
                case Constants.J1939_HEADER:
                    return new J1939Message();
                case Constants.J1939_STATUS_HEADER:
                    return new J1939Message();
                case Constants.J1708_HEADER:
                    return new J1939Message();
                default:
                    return null;
            }
        }
    }
}
