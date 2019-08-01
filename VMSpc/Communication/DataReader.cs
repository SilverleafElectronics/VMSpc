using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static VMSpc.Constants;

namespace VMSpc.Communication
{
    abstract class DataReader
    {
        public Action<string> DataProcessor;
        public Timer keepJibAwakeTimer;
        public DataReader(Action<string> DataProcessor)
        {
            this.DataProcessor = DataProcessor;
            keepJibAwakeTimer = CREATE_TIMER(KeepJibAwake, 10000);
        }
        public abstract void CloseDataReader();
        public abstract void InitDataReader();
        protected abstract void KeepJibAwake(object source, ElapsedEventArgs e);
    }
}
