using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VMSpc.Constants;

/// <summary>
/// Update Notes:
///     removed pareter index from TSPNDatum. It appeared to have no effect
/// </summary>

namespace VMSpc.Parsers
{
    public class TSPNDatum
    {
        public uint rawValue;
        public double value, valueMetric, recipNum;
        public bool seen;

        public TSPNDatum()
        {
            rawValue = 0;
            value = valueMetric = 0.0;
            seen = false;
        }

        public virtual void Parse(byte address, List<byte> data) { }

        protected virtual void ConvertAndStore()
        {
            seen = true;
        }

        #region Update Methods
        //Note: these all compile with the unsafe modifiers.

        private unsafe void VMSmemcpy(void *dest, void *src, int length)
        {
            char* csrc = (char*)src;
            char* cdest = (char*)dest;
            for (int i = 0; i < length; i++)
                cdest[i] = csrc[i];
        }

        /// <summary>
        /// updates flag according to the input two-bit pattern:
        ///    00 = sets datum to 0, returns 1
        ///    01 = sets datum to 1, returns 1
        ///    10 = sets datum to 0, returns 2 for error (not allowed in RV-C standard)
        ///    11 = no change,       returns 0
        /// </summary>
        protected unsafe byte UpdateFlag(out byte dest, byte src, byte pos)
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

        protected unsafe byte UpdateBits(out byte dest, byte src, byte pos, byte numbits)
        {
            byte temp = src;
            byte f = *(&temp);
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

        protected unsafe byte UpdateByte(ref byte dest, byte src)
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

        protected unsafe byte UpdateWord(out ushort dest, byte src)
        {
            ushort s;
            byte temp = src;
            VMSmemcpy(&s, (byte *)temp, 2);
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
            dest = 0xFFFF;
            return 0;
        }

        protected unsafe byte UpdateUint(out uint dest, byte src)
        {
            uint s;
            byte temp = src;
            VMSmemcpy(&s, (byte *)src, 4);
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
            dest = 0xFFFFFFFF;
            return 0;
        }

        #endregion //Update Methods
    }

    #region TSPNFlag
    public class TSPNFlag : TSPNDatum
    {
        protected byte byteIndex, bitIndex;

        public TSPNFlag(byte byte_index, byte bit_index) : base()
        {
            byteIndex = byte_index;
            bitIndex = bit_index;
        }

        public override void Parse(byte address, List<byte> data)
        {
            if (UpdateByte(out byte b, data[byteIndex]) != 0)
            {
                b >>= byteIndex;
                rawValue = (byte)(b & 0x03);
                ConvertAndStore();
            }
        }

        protected override void ConvertAndStore()
        {
            value = valueMetric = rawValue;
            base.ConvertAndStore();
        }
    }
    #endregion //TSPNFlag

    #region TSPNBits
    public class TSPNBits : TSPNDatum
    {
        protected byte length;
        static readonly byte[] bitmask = { 0x00, 0x01, 0x03, 0x07, 0x0F, 0x1F, 0x3F };
        protected byte byteIndex, bitIndex;

        public TSPNBits(byte byte_index, byte bit_index, byte bit_length) : base()
        {
            byteIndex = byte_index;
            bitIndex = bit_index;
            length = bit_length;
        }

        public override void Parse(byte address, List<byte> data)
        {
            if (UpdateBits(out byte b, data[byteIndex], bitIndex, length) != 0)
            {
                rawValue = (uint)(b & bitmask[length]);
                ConvertAndStore();
            }
        }

        protected override void ConvertAndStore()
        {
            base.ConvertAndStore();
            value = valueMetric = rawValue;
        }
    }
    #endregion //TSPNBits

    #region TSPNByte
    public class TSPNByte : TSPNDatum
    {
        protected byte byteIndex;
        protected double scale, offset, metricScale, metricOffset;

        public TSPNByte(byte byte_index, double scale, double offset, double metric_scale, double metric_offset) : base()
        {
            byteIndex = byte_index;
            this.scale = scale;
            this.offset = offset;
            metricScale = metric_scale;
            metricOffset = metric_offset;
        }

        unsafe public override void Parse(byte address, List<byte> data)
        {
            byte b = 0;
            if (UpdateByte(ref b, data[byteIndex]) != 0)
            {
                rawValue = (uint)b;
                ConvertAndStore();
            }
        }

        protected override void ConvertAndStore()
        {
            if (recipNum != 0.0)
            {
                value = rawValue * scale + offset;
                valueMetric = (recipNum / value); //right now only instant mpg recent and rolling mpg use this
                value = valueMetric; //this only applies to gauges using L/100Km
            }
            else
            {
                value = rawValue * scale + offset;
                valueMetric = rawValue * metricScale + metricOffset;
            }
            base.ConvertAndStore();
        }
    }
    #endregion //TSPNByte

    #region TSPNWord
    public class TSPNWord : TSPNByte
    {

        public TSPNWord(byte byte_index, double scale, double offset, double metric_scale, double metric_offset)
            : base(byte_index, scale, offset, metric_scale, metric_offset)
        {
            recipNum = 0.0;
        }

        public TSPNWord(byte byte_index, double scale, double offset, double metric_scale, double metric_offset, double recip_numerator)
            : base(byte_index, scale, offset, metric_scale, metric_offset)
        {
            recipNum = recip_numerator;
        }

        unsafe public override void Parse(byte address, List<byte> data)
        {
            if (UpdateWord(rawValue, data, byteIndex))
            {

            }
        }
    }
    #endregion //TSPNWord

    #region TSPNUint
    public class TSPNUint : TSPNByte
    {
        public TSPNUint(byte byte_index, double scale, double offset, double metric_scale, double metric_offset)
             : base(byte_index, scale, offset, metric_scale, metric_offset)
        {

        }
    }
    #endregion //TSPNUint
}
