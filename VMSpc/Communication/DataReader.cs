using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VMSpc.Common;
using static VMSpc.Constants;

namespace VMSpc.Communication
{
    abstract class DataReader : IEventPublisher
    {
        public Timer keepJibAwakeTimer;
        public ulong messagesReceived;
        public ulong lastMessageCount;
        protected const int MaxBufferSize = 4096;

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        public DataReader()
        {
            messagesReceived = 0;
            EventBridge.Instance.AddEventPublisher(this);
        }
        ~DataReader()
        {

        }
        public abstract void CloseDataReader();
        public virtual void InitDataReader()
        {
            KeepJibAwake();
            keepJibAwakeTimer = CREATE_TIMER(KeepJibAwake, 10000);
        }

        public abstract void SendMessage(OutgoingMessage message);
        public abstract void SendMessage(string message);

        protected virtual void KeepJibAwake()
        {
            SendMessage("V");
        }

        protected void OnDataReceived(string data)
        {
            RaiseVMSEvent?.Invoke(
                this,
                new VMSCommDataEventArgs(data, DateTime.Now)
            );
        }
    }
}
