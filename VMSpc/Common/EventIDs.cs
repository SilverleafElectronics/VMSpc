using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Common
{
    public static class EventIDs
    {

        /*
        * Event ID Anatomy (For all events, Complex Event Bases, unless specified otherwise):
        * 0x{EventBase(1 byte)}{EventInstance(1 byte)}{EventSubID(2 bytes)}
        * byte 0-1 - sub-identifier (e.g., PID)
        * byte 2 - instance
        * byte 3 - Event Base ID
        * For example, suppose that a PID event could be instanced (currently doesn't happen). The eventID could be
        * 0x02010085
        * 02 is the PID_BASE
        * 01 is the instance
        * 0085 is the PID
        * 
        * 
        * Any additional values accompanying a published event should be included as a separate property of the event. For instance,
        * PID Events (VMSDataEventArgs) publish the `value` as a separate field of the event object.
        * 
        * NOTE: Simple Events (see below) must have a base of 0xFD000000000000. The remaining 56 bits are to be used as unique identifiers, and 
        * don't follow the instancing/sub-ID scheme mentioned above. They start at 0xFE00000000000001 and should increment from there.
        * 
        */

        //Simple Events - These events only carry an Event Identifier, without any instancing or other multiplexing. The event object that
        //gets published with these can have an associated value item (e.g., NEW_COMM_DATA_EVENT), but usually does not.
        #region Simple Events

        public static ulong GUI_RESET_EVENT => 0xFE00000000000001;
        public static ulong NEW_COMM_DATA_EVENT => 0xFE00000000000002;
        public static ulong COMM_DATA_ERROR_EVENT => 0xFE00000000000003;


        #endregion Simple Events

        //Instanced Events - These base events only carry an Event Identifier and an instance. 
        #region Instanced Event Bases
        public static ulong CURRENT_MPG_EVENT => 0xFD00000100000000;
        public static ulong DISTANCE_REMAINING_EVENT => 0xFD00000200000000;
        public static ulong FUEL_READING_EVENT => 0xFD00000300000000;
        public static ulong DISTANCE_TRAVELLED_EVENT => 0xFD00000400000000;
        public static ulong HOURS_EVENT => 0xFD00000500000000;
        public static ulong AVERAGE_SPEED_EVENT => 0xFD00000600000000;

        #endregion Instanced Event Bases

        //Complex Event Bases Bases - These are the base values for multiplexing a broad range of events. They can use all parts of the event structure
        //The MSByte in these events must be in the range 1-128 (0x01 - 0x80)
        #region Complex Event Bases

        public static ulong PID_BASE => 0x0100000000000000;
        public static ulong DIAGNOSTIC_BASE => 0x0200000000000000;
        public static ulong TIRE_BASE => 0x0300000000000000;
        public static ulong ALARM_BASE => 0x0400000000000000;
        
        /// <summary>
        /// Data published for use by UI Elements. This should always be OR'd with a PID in the LSWord
        /// </summary>
        public static ulong PARSED_DATA_EVENT => 0x0500000000000000;
        /// <summary>
        /// Data published for use by Advanced J1939 Parsers. Byte 0 = 0x06; Byte 1 = Source Address (optional. subscribing to 0xFF SA will catch all); Bytes 2-8 = PGN, Bytes 7-15 are unused
        /// </summary>
        public static ulong J1939_RAW_DATA_EVENT => 0x0600000000000000;
        /// <summary>
        /// /Returns a J1939_RAW_DATA_EVENT with the specified PGN and SourceAddress applied. Leaving the SourceAdress at the default 0xFF will catch all Source Addresses
        /// </summary>
        /// <param name="pgn"></param>
        /// <param name="SourceAddress"></param>
        /// <returns></returns>
        public static ulong Get_J1939RawDataEvent(uint pgn, byte SourceAddress = 0xFF)
        {
            ulong e = J1939_RAW_DATA_EVENT | ((ulong)pgn << 24);
            e |= ((ulong)SourceAddress << 48);
            return e;
        }
        /// <summary>
        /// Gets the PGN from a J1939RawDataEvent
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        //TODO
        public static uint Extract_PGN_J1939RawDataEvent(ulong e)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets teh Source Address from a J1939RawDataEvent
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        //TODO
        public static byte Extract_SourceAddress_J1939RawDataEvent(ulong e)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Data published for use by Advanced J1708 Parsers. Byte 0 = 0x07; Byte 1 = MID (optional. subscribing to 0xFF MID will catch all); Byte 1-2 = PID; Byte 3-15 are unused
        /// </summary>
        public static ulong J1708_RAW_DATA_EVENT => 0X0700000000000000;
        public static ulong Get_J1708RawDataEvent(ushort pid, byte mid = 0xFF)
        {
            ulong e = J1708_RAW_DATA_EVENT | ((ulong)pid << 32);
            e |= ((ulong)mid << 48);
            return e;
        }
        //TODO
        public static ushort Extract_PID_J1708RawDataEvent(ulong e)
        {
            throw new NotImplementedException();
        }
        //TODO
        public static byte Extract_MID_J1708RawDataEvent(ulong e)
        {
            throw new NotImplementedException();
        }

        #endregion Complex Event Bases

    }
}
