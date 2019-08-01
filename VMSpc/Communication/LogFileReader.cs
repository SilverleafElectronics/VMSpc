using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace VMSpc.Communication
{
    class LogFileReader : DataReader
    {
        public LogFileReader(Action<string> DataProcessor)
            : base(DataProcessor)
        {

        }
        public override void InitDataReader()
        {
            throw new NotImplementedException();
        }
        public override void CloseDataReader()
        {
            throw new NotImplementedException();
        }
        protected override void KeepJibAwake(object source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
