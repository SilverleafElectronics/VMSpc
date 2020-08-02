using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMSpc.Common;
using VMSpc.JsonFileManagers;

namespace VMSpc
{
    public class DayNightManager : IEventPublisher, ISingleton
    {
        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        public static DayNightManager Instance { get; private set; }
        static DayNightManager() { }
        public static void Initialize() 
        {
            Instance = new DayNightManager();
        }

        private ScreenContents Screen = ConfigurationManager.ConfigManager.Screen.Contents;
        private InAppTaskScheduler ChangeToDayScheduler;
        private InAppTaskScheduler ChangeToNightScheduler;
        private DayNightManager()
        {

            EventBridge.Instance.AddEventPublisher(this);
            if (Screen.UseDayNightTimer)
            {
                SetDayStartTime(Screen.DayStartTime);
                SetNightStartTime(Screen.NightStartTime);
                ActivateTimer();
            }
        }

        public void DectivateTimer()
        {
            ChangeToDayScheduler?.Stop();
            ChangeToNightScheduler?.Stop();
        }

        public void ActivateTimer()
        {
            ChangeToDayScheduler?.Start();
            ChangeToNightScheduler?.Start();
            DeterminePaletteFromTimers();
        }

        /// <summary>
        /// Determines which palette to used based on the difference between the current time and the day/night scheduled times.
        /// </summary>
        public void DeterminePaletteFromTimers()
        {
            var now = DateTime.Now;
            var timeUntilDay = GetDateIndependentTimeSpan(now, ChangeToDayScheduler.ScheduledExecutionTime).TotalHours;
            var timeUntilNight = GetDateIndependentTimeSpan(now, ChangeToNightScheduler.ScheduledExecutionTime).TotalHours;
            if (timeUntilDay < 0 && timeUntilNight > 0)
            {
                SetDay();
            }
            else if (timeUntilDay < 0 && timeUntilNight < 0)
            {
                if (timeUntilDay < timeUntilNight)
                {
                    SetNight();
                }
                else
                {
                    SetDay();
                }
            }
            else
            {
                SetNight();
            }
        }

        public void SetDayStartTime(DateTime time)
        {
            ChangeToDayScheduler = new InAppTaskScheduler();
            ChangeToDayScheduler.AssignDailyTask(time, SetDay);
            if (GetDateIndependentTimeSpan(DateTime.Now, time).TotalSeconds < 0)
                SetDay();
        }

        public void SetNightStartTime(DateTime time)
        {
            ChangeToNightScheduler = new InAppTaskScheduler();
            ChangeToNightScheduler.AssignDailyTask(time, SetNight);
            if (GetDateIndependentTimeSpan(DateTime.Now, time).TotalSeconds < 0)
                SetNight();
        }

        public void SetDay()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!Screen.OnDayPalette)
                {
                    Screen.OnDayPalette = true;
                    PublishResetEvent();
                }
            });
        }

        public void SetNight()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Screen.OnDayPalette)
                {
                    Screen.OnDayPalette = false;
                    PublishResetEvent();
                }
            });
        }

        private void PublishResetEvent()
        {
            RaiseVMSEvent?.Invoke(this, new VMSEventArgs(EventIDs.GUI_RESET_EVENT));
        }

        /// <summary>
        /// Compares two dates based on hour/minute/second, irrespective of date. Returns a TimeSpan
        /// indicating the difference between endDate and startDate
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static TimeSpan GetDateIndependentTimeSpan(DateTime startDate, DateTime endDate)
        {
            var startTime = new DateTime(1, 1, 1, startDate.Hour, startDate.Minute, startDate.Second);
            var endTime = new DateTime(1, 1, 1, endDate.Hour, endDate.Minute, endDate.Second);
            return endTime - startTime;
        }
    }
}
