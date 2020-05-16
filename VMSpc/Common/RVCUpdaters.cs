using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.Constants;

namespace VMSpc.Common
{
    public static class RVCUpdaters
    {
        /// <summary>
        /// updates flag according to the input two-bit pattern:
        ///    00 = sets datum to 0, returns 1
        ///    01 = sets datum to 1, returns 1
        ///    10 = sets datum to 0, returns 2 for error (not allowed in RV-C standard)
        ///    11 = no change,       returns 0
        /// </summary>
        public static byte UpdateFlag(ref byte dest, byte src, byte pos)
        {
            byte f = (byte)(((src) >> pos) & 0x03);
            if ((f & 0x02) == 0)
            {
                dest = f;
                return 1;
            }
            if (f == 0x02)
            {
                dest = 0;
                return 2;
            }
            dest = 0xFF;
            return 0;
        }

        public static byte UpdateBits(ref byte dest, byte src, byte pos, byte numbits)
        {
            byte f = src;
            byte mask = (byte)((1 << numbits) - 1);
            f >>= pos;
            f &= mask;
            if (f < (mask - 1))
            {
                dest = f;
                return 1;
            }
            if (f < mask)
            {
                dest = 0;
                return 2;
            }
            dest = 0xFF;
            return 0;
        }

        public static byte UpdateByte(ref byte dest, byte src)
        {
            if (src <= RVC_BYTE(RVC_MAXVAL))
            {
                dest = src;
                return 1;
            }
            if (src != RVC_BYTE(RVC_NODATA))
            {
                dest = 0;
                return 2;
            }
            return 0;
        }

        public static byte UpdateWord(ref ushort dest, byte[] data, byte pos)
        {
            ushort s = (ushort)((data[pos + 1] << 8) | (data[pos]));
            if (s <= RVC_WORD(RVC_MAXVAL))
            {
                dest = s;
                return 1;
            }
            if (s != RVC_WORD(RVC_NODATA))
            {
                dest = 0;
                return 2;
            }
            dest = s;
            return 0;
        }

        public static byte UpdateUint(ref uint dest, byte[] data, byte pos)
        {
            uint s = (uint)((data[pos + 3] << 24) | (data[pos + 2] << 16) | (data[pos + 1] << 8) | (data[pos]));
            if (s <= RVC_LONG(RVC_MAXVAL))
            {
                dest = s;
                return 1;
            }
            if (s != RVC_LONG(RVC_NODATA))
            {
                dest = 0;
                return 2;
            }
            dest = s;
            return 0;
        }
    }
}
