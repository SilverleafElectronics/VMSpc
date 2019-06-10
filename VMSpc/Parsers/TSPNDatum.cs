using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Parsers
{
    public class TSPNDatum
    {
        public uint rawValue;
        public double value, valueMetric;
        public uint parameterIndex;
        public bool seen;

        public TSPNDatum(uint parameter_index)
        {
            parameterIndex = parameter_index;
        }

        public virtual void Parse(byte address, List<byte> data) { }

        private void ConvertAndStore() { }

        private void ConvertAndStore(float scale, float offset) { }
    }

    public class TSPNFlag : TSPNDatum
    {
        protected byte byteIndex, bitIndex;

        public TSPNFlag(uint parameter_index, byte byte_index, byte bit_index) : base(parameter_index)
        {

        }
    }

    public class TSPNBits : TSPNFlag
    {
        protected byte length;

        public TSPNBits(uint parameter_index, byte byte_index, byte bit_index, byte bit_length) : base(parameter_index, byte_index, bit_index)
        {

        }
    }

    public class TSPNByte : TSPNDatum
    {
        protected byte byteIndex;
        protected double scale, offset, metricScale, metricOffset;

        public TSPNByte(uint parameter_index, byte byte_index, double scale, double offset, double metric_scale, double metric_offset) : base(parameter_index)
        {

        }
    }

    public class TSPNWord : TSPNByte
    {
        protected double recipNum;

        public TSPNWord(uint parameter_index, byte byte_index, double scale, double offset, double metric_scale, double metric_offset)
            : base(parameter_index, byte_index, scale, offset, metric_scale, metric_offset)
        {

        }

        public TSPNWord(uint parameter_index, byte byte_index, double scale, double offset, double metric_scale, double metric_offset, double recip_numerator)
            : base(parameter_index, byte_index, scale, offset, metric_scale, metric_offset)
        {

        }
    }

    public class TSPNUint : TSPNByte
    {
        public TSPNUint(uint parameter_index, byte byte_index, double scale, double offset, double metric_scale, double metric_offset)
             : base(parameter_index, byte_index, scale, offset, metric_scale, metric_offset)
        {

        }
    }
}
