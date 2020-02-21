using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Helpers
{
    public class IndexingInteger
    {
        public int Value { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public IndexingInteger(int Value)
        {
            this.Value = Value;
            this.MaxValue = Int32.MaxValue;
            this.MinValue = Int32.MinValue;
        }
        public IndexingInteger(int Value, int MaxValue, int MinValue)
        {
            this.Value = Value;
            this.MaxValue = MaxValue;
            this.MinValue = MinValue;
        }

        public override bool Equals(object obj)
        {
            return Value == (int)obj;
        }

    }
}
