using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.EventManagement
{
    public class ManagerEventArgs
    {
        public static uint MILES_EVENT = 0x00000001;
        public static uint FUEL_EVENT = 0x00000002;
        public static uint GALLONS_EVENT = 0x00000003;
        public static uint LITERS_EVENT = 0x00000004;
    }
}
