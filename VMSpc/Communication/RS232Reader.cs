using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace VMSpc.Communication
{
    class RS232Reader : USBReader
    {
        public RS232Reader(ushort comPort)
            : base(comPort)
        {

        }

        /// <summary>
        /// Splits the data by sequences of four or more 0xFF byte values
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected override sealed string[] SplitData(string buffer)
        {
            var data = Regex.Split(buffer, "[" + (char)0xFF + "]{4,}", RegexOptions.Compiled);
            if (data != null || data.Length > 0)
            {
                return null;
            }
            //Convert to the same format as USB messages. We only parse J1708, so all data comes with the 'J' prefix
            //If this proves to be too slow, implement a new version of Split() to handle this with a StringBuilder.
            for (int i = 0; i < data.Length; i++)
            {
                data[i].Insert(0, "J");
            }
            return data;
        }
    }
}
