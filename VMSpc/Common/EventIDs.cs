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
        * Event ID Structure (For all but Simple Events):
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
        * NOTE: Simple Events (see below) must have a base of 0xFD000000. The remaining 6 bits are to be used as unique identifiers, and 
        * don't follow the instancing/sub-ID scheme mentioned above. They start at 0xFE000001 and should increment from there.
        * 
        */

        //Simple Events - These events only carry an Event Identifier, without any instancing or other multiplexing. The event object that
        //gets published with these can have an associated value item (e.g., NEW_COMM_DATA_EVENT), but usually does not.
        #region Simple Events

            public static uint GUI_RESET_EVENT => 0xFE000001;
            public static uint NEW_COMM_DATA_EVENT => 0xFE000002;
            public static uint J1708_CHECKSUM_ERROR_EVENT => 0xFE000003;

        #endregion Simple Events

        //Instanced Events - These base events that can only use the Event Base ID and the instance. i.e., the LSWord is not used. 
        //The MSByte in these events must be in the range 129-253 (0x81 - 0xFD)
        #region Instanced Event Bases
            public static uint CURRENT_MPG_EVENT => 0x81000000;
            public static uint DISTANCE_REMAINING_EVENT => 0x82000000;
            public static uint FUEL_READING_EVENT => 0x83000000;
            public static uint DISTANCE_TRAVELLED_EVENT => 0x84000000;
            public static uint HOURS_EVENT => 0x85000000;
            public static uint AVERAGE_SPEED_EVENT => 0x86000000;

        #endregion Instanced Event Bases

        //Complex Event Bases Bases - These are the base values for multiplexing a broad range of events. They can use all parts of the event structure
        //The MSByte in these events must be in the range 1-128 (0x01 - 0x80)
        #region Complex Event Bases

            public static uint PID_BASE => 0x01000000;
            public static uint DIAGNOSTIC_BASE => 0x02000000;
            public static uint TIRE_BASE => 0x03000000;
            public static uint ALARM_BASE => 0x04000000;

        #endregion Instanced Event Bases

    }
}
