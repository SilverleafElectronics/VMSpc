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
        public ushort Instance;
        public List<AlarmCondition> AlarmConditions;
        public Color Color;
        public string Message;
        public AudibleSettings SoundSettings;
    }

    public class AlarmContents : IJsonContents
    {
        public List<AlarmSettings> alarmSettingsList;
    }

    public class AlarmsReader : JsonFileReader<AlarmContents>
    {
        public AlarmsReader() : base("\\configuration\\messages.json")
        {
        }

        protected override AlarmContents GetDefaultContents()
        {
            return new AlarmContents()
            {
                alarmSettingsList = new List<AlarmSettings>()
                {
                    new AlarmSettings()
                    {
                        Instance = 1,
                        Name = "Amb. Temp",
                        Color = Colors.Red,
                        Message = "Low Ambient Temperature",
                        AlarmConditions = new List<AlarmCondition>()
                        {
                            new AlarmCondition()
                            {
                                Pid = 171,
                                Comparator = ComparativeOperator.GreaterThan,
                                TriggerValue = 0,
                            },
                        },
                        SoundSettings = new AudibleSettings()
                        {

                        },
                    }
                },
            };
        }

        public void AddAlarm(AlarmSettings alarmSettings)
        {
            alarmSettings.Instance = (ushort)Contents.alarmSettingsList.Count;
            Contents.alarmSettingsList.Add(alarmSettings);
        }

        public void DeleteAlarm(ushort instance)
        {
            foreach (var alarm in Contents.alarmSettingsList)
            {
                if (alarm.Instance == instance)
                {
                    Contents.alarmSettingsList.Remove(alarm);
                    break;
                }
            }
            for (int i = 0; i <= Contents.alarmSettingsList.Count; i++)
            {
                Contents.alarmSettingsList[i + 1].Instance = (ushort)i;
            }
        }
    }
}
