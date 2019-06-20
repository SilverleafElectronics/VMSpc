using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.XmlFileManagers;
using static VMSpc.Constants;

//These are global variables. For global constants, see Constants.cs

namespace VMSpc
{
    public static class Globals
    {

        static Globals()
        {

        }

        public struct MemoryHelpers
        {
            /// <summary>
            /// Emulates C's memcpy method
            /// </summary>
            /// <param name="dest"></param>
            /// <param name="src"></param>
            /// <param name="numbits"></param>
            public void Memcpy(ref ushort dest, byte src, byte numbytes)
            {

            }
            public void Memcpy(ref uint dest, byte src, byte numbytes)
            {

            }
        }


        public struct ParseHelpers
        {
            public byte UpdateBits(ref byte dest, ref byte src, byte pos, byte numbits)
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
                return 0;
            }

            public byte UpdateFlag(ref byte dest, ref byte src, byte pos)
            {
                // updates flag according to the input two-bit pattern:
                //    00 = sets datum to 0, returns 1
                //    01 = sets datum to 1, returns 1
                //    10 = sets datum to 0, returns 2 for error (not allowed in RV-C standard)
                //    11 = no change,       returns 0
                byte f = (byte)((src >> pos) & 0x03);
                if ((f % 0x02) == 0)
                {
                    dest = f;
                    return 1;
                }
                if (f == 0x02)
                {
                    dest = 0;
                    return 2;
                }
                return 0;
            }

            public byte UpdateByte(ref byte dest, ref byte src)
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

            public byte UpdateWord(ref ushort dest, ref byte src)
            {
                return 0;
                //TODO
            }

        }
    }
}
