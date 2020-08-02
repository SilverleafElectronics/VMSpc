﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using static VMSpc.Extensions.UI.TextBlockExtensions;

namespace VMSpc.UI.GaugeComponents
{
    /// <summary>
    /// A textblock that automatically displays the time. Is enabled by default.
    /// </summary>
    class ClockComponent : TextBlock
    {
        private Timer ClockTimer;
        private DateTime lastDrawnTime;
        private bool textIsScaled;
        public bool useMilitaryTime { get; set; }
        private bool TimerEnabled { get; set; }

        public ClockComponent(bool useMilitaryTime) : base()
        {
            this.useMilitaryTime = useMilitaryTime;
            //VerticalContentAlignment = VerticalAlignment.Center;
            ClockTimer = Constants.CREATE_TIMER(UpdateClock, 250, Enums.UI.DispatchType.OnMainThread);
            ClockTimer.Start();
            Enable();
        }

        public void Enable()
        {
            if (!TimerEnabled)
            {
                textIsScaled = false;
                TimerEnabled = true;
                Constants.DestroyTimer(ClockTimer);
                Constants.CREATE_TIMER(UpdateClock, 250, Enums.UI.DispatchType.OnMainThread);
                ClockTimer.Start();
            }
        }
        public void Disable()
        {
            if (TimerEnabled)
            {
                TimerEnabled = false;
                Constants.DestroyTimer(ClockTimer);
            }
        }

        private void UpdateClock()
        {
            DateTime currentTime = DateTime.Now;
            if (currentTime.Second != lastDrawnTime.Second)
            {
                lastDrawnTime = currentTime;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetTime(currentTime);
                });
            }
        }

        private void SetTime(DateTime currentTime)
        {
            if (!useMilitaryTime)
            {
                Text = currentTime.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);
            }
            else
            {
                Text = currentTime.ToString("HH:mm:ss");
            }
            if (!textIsScaled)
            {
                this.ScaleText();
                textIsScaled = true;
            }
        }
    }
}
