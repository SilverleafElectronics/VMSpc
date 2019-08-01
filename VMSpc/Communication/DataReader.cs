﻿using System;
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
        public ulong messagesReceived;
        public ulong lastMessageCount;
        public DataReader(Action<string> DataProcessor)
        {
            this.DataProcessor = DataProcessor;
            messagesReceived = 0;
        }
        public abstract void CloseDataReader();
        public virtual void InitDataReader()
        {
            KeepJibAwake(null, null);
            keepJibAwakeTimer = CREATE_TIMER(KeepJibAwake, 10000);
        }
        public abstract bool SendMsg();
        protected abstract void KeepJibAwake(object source, ElapsedEventArgs e);
    }
}
