using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VMSpc.Enums.UI;

namespace VMSpc.JsonFileManagers
{
    public class AudibleSettings
    {
        public AlarmFrequency AlarmFrequency;
        public string AudioFilePath;
    }

    public class AlarmCondition
    {
        public ushort Pid;
        public double TriggerValue;
        public ComparativeOperator Comparator;
    }

    public class AlarmSettings
    {
        public string Name;
        public int Instance { get; set; }
        public AlarmCondition AlarmCondition { get; set; }
        public Color Color;
        public string Message;
        public AlarmFrequency Frequency;
        public AudibleSettings SoundSettings;
    }

    public class AlarmContents : IJsonContents
    {
        public List<AlarmSettings> AlarmSettingsList;
    }

    public class AlarmsReader : JsonFileReader<AlarmContents>
    {
        public AlarmsReader() : base("\\configuration\\alarms.json")
        {
        }

        protected override AlarmContents GetDefaultContents()
        {
            return new AlarmContents()
            {
                AlarmSettingsList = new List<AlarmSettings>()
                {
                    new AlarmSettings()
                    {
                        Instance = 0,
                        Name = "Road Speed",
                        Color = Colors.Red,
                        Message = "High Speed",
                        AlarmCondition = 
                        new AlarmCondition()
                        {
                            Pid = 84,
                            Comparator = ComparativeOperator.GreaterThan,
                            TriggerValue = 70,
                        },
                        SoundSettings = new AudibleSettings()
                        {

                        },
                    }
                },
            };
        }

        public AlarmSettings GetAlarmByInstance(int instance)
        {
            return Contents.AlarmSettingsList.Find(x => x.Instance == instance);
        }

        public void AddAlarm(AlarmSettings alarmSettings)
        {
            alarmSettings.Instance = Contents.AlarmSettingsList.Count;
            Contents.AlarmSettingsList.Add(alarmSettings);
        }

        public void DeleteAlarm(ushort instance)
        {
            foreach (var alarm in Contents.AlarmSettingsList)
            {
                if (alarm.Instance == instance)
                {
                    Contents.AlarmSettingsList.Remove(alarm);
                    break;
                }
            }
            for (int i = 1; i <= Contents.AlarmSettingsList.Count; i++)
            {
                Contents.AlarmSettingsList[i - 1].Instance = i;
            }
        }
    }
}
