using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using VMSpc.Enums.UI;

namespace VMSpc.Common
{
    /// <summary>
    /// Class for assigning tasks to defined schedules. Each instance of the class can manage one single task.
    /// </summary>
    public class InAppTaskScheduler
    {
        private Timer InitialRunTimer;
        private Timer TaskTimer;
        public DateTime ScheduledExecutionTime { get; private set; }
        private Action Callback;
        /// <summary>
        /// The number of milliseconds in 24 hours
        /// </summary>
        private const int dayMilliseconds = 86400000;
        public InAppTaskScheduler()
        {

        }

        ~InAppTaskScheduler()
        {
            KillTimers();
        }

        public void Start()
        {
            StartTimers();
        }

        public void Stop()
        {
            StopTimers();
        }

        private void KillTimers()
        {
            StopTimers();
            InitialRunTimer = null;
            TaskTimer = null;
        }

        private void StopTimers()
        {
            InitialRunTimer?.Stop();
            TaskTimer?.Stop();
        }

        /// <summary>
        /// Calls the specified callback at the first passing of the h/m/s specified in the scheduledExecutionTime. Once the first
        /// runtime passes, a new timer is created to execute the callback every 24 hours.
        /// </summary>
        private void StartTimers()
        {
            KillTimers();
            int millisecondsToFirstExecution = (int)DayNightManager.GetDateIndependentTimeSpan(DateTime.Now, ScheduledExecutionTime).TotalMilliseconds;
            //If setting the schedule to a time of day that has already passed, calculate the time until its execution the following day
            if (millisecondsToFirstExecution < 0)
            {
                millisecondsToFirstExecution = dayMilliseconds + millisecondsToFirstExecution;
            }
            InitialRunTimer = Constants.CREATE_TIMER(
                () =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Callback();
                        TaskTimer?.Stop();
                        TaskTimer = Constants.CREATE_TIMER(Callback, dayMilliseconds);
                        TaskTimer.Start();
                    });
                },
                millisecondsToFirstExecution
                );
            InitialRunTimer.AutoReset = false;
        }

        /// <summary>
        /// Assigns the ScheduledExecutionTime and the Callback. This does not start the scheduled task timer.
        /// </summary>
        /// <param name="scheduledExecutionTime"></param>
        /// <param name="callback"></param>
        public void AssignDailyTask(DateTime scheduledExecutionTime, Action callback)
        {
            ScheduledExecutionTime = scheduledExecutionTime;
            Callback = callback;
        }
    }
}
