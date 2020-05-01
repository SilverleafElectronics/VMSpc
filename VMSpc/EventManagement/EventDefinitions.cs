using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.EventManagement
{
    public static class EventDefinitions
    {
        public static uint PID_BASE = 0x00010000;
        public static uint DIAGNOSTIC_BASE = 0x00020000;
        public static uint TIRE_BASE = 0x00030000;
        public static uint MANAGER_BASE = 0x00040000;
    }
}
