using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace VMSpc.Communication
{
    class SerialPortReader : DataReader
    {
        public SerialPortReader(Action<string> DataProcessor)
            : base(DataProcessor)
        {
        }
        public override void InitDataReader()
        {
            base.InitDataReader();
            throw new NotImplementedException();
        }

        public override void CloseDataReader()
        {
            throw new NotImplementedException();
        }
        public override bool SendMsg()
        {
            throw new NotImplementedException();
        }
        protected override void KeepJibAwake()
        {
            throw new NotImplementedException();
        }
    }
}
