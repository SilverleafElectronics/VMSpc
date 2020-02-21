using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.VEnum.Parsing;
using System.Windows;
using static VMSpc.EventBridge;
using VMSpc.CustomComponents;

namespace VMSpc.UI.GaugeComponents
{
    /// <summary>
    /// Base class of all Gauge Components, a flexible custom canvas implementation
    /// that redraws itself on changes to PID values
    /// </summary>
    public abstract class GaugePIDComponent : GaugeComponent
    {
        protected ushort pid;
        protected double pidValue;
        protected static double STALE_DATA = double.NaN;
        protected static double VOID_DATA = double.MaxValue;
        protected static ushort INVALID_PID = ushort.MaxValue;
        private bool enabled;
        private bool updating;
        double cachedValue;
        public GaugePIDComponent(ushort pid)
        {
            cachedValue = VOID_DATA;
            updating = false;
            pidValue = STALE_DATA;
            enabled = true;
            ChangePID(pid);
        }

        public void ChangePID(ushort pid)
        {
            if (this.pid != pid)
            {
                if (this.pid != INVALID_PID)
                    UnsubscribeToPID(this.pid);
                this.pid = pid;
                SubscribeForPID(this.pid);
            }
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        public void Collapse()
        {
            Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Subscribes to an event based on the PID
        /// </summary>
        public void SubscribeForPID(ushort pid)
        {
            uint eventID = Constants.PID_BASE | (uint)pid;
            SubscribeToEvent(eventID);
        }

        public void UnsubscribeToPID(ushort pid)
        {
            uint eventID = Constants.PID_BASE | (uint)pid;
            UnsubscribeFromEvent(eventID);
        }

        public override abstract void Draw();
        public override abstract void Update();

        public override void Enable()
        {
            enabled = true;
        }

        public override void Disable()
        {
            enabled = false;
        }

        protected override void HandleNewData(VMSEventArgs e)
        {
            HandleNewData(((VMSDataEventArgs)e).value);
        }

        protected void HandleNewData(double newValue)
        {
            cachedValue = newValue;
            if (!updating)
            {
                //HandleNewData() is called asynchronously each time a new data event is raised,
                //
                while (cachedValue < VOID_DATA)
                {
                    updating = true;
                    pidValue = cachedValue;
                    cachedValue = VOID_DATA;
                    if (enabled)
                        Update();
                    updating = false;
                }
            }
        }
    }
}
