using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.Managers.Alarms
{
    public static class AlarmManager
    {
        public static List<Alarm> Alarms = new List<Alarm>();

        public static void LoadAlarms()
        {
            ResetAlarms();
            foreach (var alarmSetting in ConfigurationManager.ConfigManager.AlarmsReader.Contents.alarmSettingsList)
            {
                Alarms.Add(new Alarm(alarmSetting));
            }
        }

        private static void ResetAlarms()
        {
            foreach (var alarm in Alarms)
            {
                alarm.DeleteAlarm();
            }
            Alarms?.Clear();
            Alarms = new List<Alarm>();
        }

        public static void AddAlarm(AlarmSettings alarm)
        {
            ConfigurationManager.ConfigManager.AlarmsReader.AddAlarm(alarm);
            LoadAlarms();
        }

        public static void DeleteAlarm(ushort instance)
        {
            ConfigurationManager.ConfigManager.AlarmsReader.DeleteAlarm(instance);
            LoadAlarms();
        }
    }
}
