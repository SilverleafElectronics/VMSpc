using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.Enums.Parsing;
using System.Windows;
using VMSpc.Common;
using VMSpc.UI.CustomComponents;
using VMSpc.JsonFileManagers;
using System.Windows.Threading;

namespace VMSpc.UI.GaugeComponents
{
    /// <summary>
    /// Base class of all Gauge Components, a flexible custom canvas implementation
    /// that redraws itself on changes to PID values
    /// </summary>
    public abstract class GaugePIDComponent : GaugeComponent
    {
        protected ushort pid;
        protected JParameter parameter;
        protected double currentValue;
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
            currentValue = STALE_DATA;
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
            if (parameter == null || parameter.Pid != this.pid)
            {
                parameter = ConfigurationManager.ConfigManager.ParamData.GetParam(pid);
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
            ulong eventID = EventIDs.PID_BASE | (ulong)pid;
            SubscribeToEvent(eventID);
        }

        public void UnsubscribeToPID(ushort pid)
        {
            ulong eventID = EventIDs.PID_BASE | (ulong)pid;
            UnsubscribeFromEvent(eventID);
        }

        public override sealed void Draw()
        {
            DrawComponentLayout();
            if (!double.IsNaN(parameter.LastValue))
            {
                HandleNewData(parameter.LastValue);
            }
        }

        public abstract void DrawComponentLayout();
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
            //TODO - get this to work properly if there are any issues with performance
            Action action = () => HandleNewData((e as VMSPidValueEventArgs).value);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
            //var value = (e as VMSPidValueEventArgs)?.value;
            //HandleNewData((double)value);
        }

        protected void HandleNewData(double newValue)
        {
            if (newValue == currentValue)
            {
                return;
            }
            cachedValue = newValue;
            if (!updating)
            {
                //HandleNewData() is called asynchronously each time a new data event is raised,
                //
                while (cachedValue != VOID_DATA)
                {
                    updating = true;
                    currentValue = cachedValue;
                    cachedValue = VOID_DATA;
                    if (enabled)
                        Update();
                    updating = false;
                }
            }
        }
    }
}
