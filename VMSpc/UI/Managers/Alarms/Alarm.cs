using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.JsonFileManagers;
using VMSpc.Enums.UI;
using System.Timers;
using System.Windows.Media;
using VMSpc.Common;
using static VMSpc.Constants;

namespace VMSpc.UI.Managers.Alarms
{
    public class Alarm : IEventPublisher
    { 
        protected AlarmSettings Settings;
        protected List<AlarmConditionProcessor> alarmConditionProcessors;
        protected Timer alarmCheckTimer;
        public ulong SecondsTriggered { get; private set; }
        public bool IsTriggered { get; private set; }
        private MediaPlayer alarmSoundPlayer;

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        public Alarm(AlarmSettings settings)
        {
            this.Settings = settings;
            alarmConditionProcessors = new List<AlarmConditionProcessor>();
            alarmSoundPlayer = new MediaPlayer();
            AddConditions();
            alarmCheckTimer = CREATE_TIMER(ProcessConditions, 1000);
            SecondsTriggered = 0;
        }

        public void DeleteAlarm()
        {
            alarmCheckTimer?.Stop();
            foreach (var conditionProcessor in alarmConditionProcessors)
            {
                conditionProcessor.DropCondition();
            }
            alarmConditionProcessors.Clear();
        }

        protected virtual void OnRaiseCustomEvent(AlarmEventArgs e)
        {
            RaiseVMSEvent?.Invoke(this, e);
        }

        protected void AddConditions()
        {
            alarmConditionProcessors.Add(new AlarmConditionProcessor(Settings.AlarmCondition));
        }

        /// <summary>
        /// Checks all alarm conditions and calls TriggerAlarm() if all are triggered. Turns off the alarm if it was previousy triggered.
        /// </summary>
        protected void ProcessConditions()
        {
            foreach (var alarmCondition in alarmConditionProcessors)
            {
                if (!alarmCondition.IsMet)
                {
                    if (IsTriggered)
                        TurnOffAlarm();
                    return;
                }
            }
            TriggerAlarm();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void TriggerAlarm()
        {
            IsTriggered = true;
            if (Settings.SoundSettings != null)
            {
                switch (Settings.SoundSettings.AlarmFrequency)
                {
                    case AlarmFrequency.Once:
                        if ((SecondsTriggered == 1))
                            PlayAudio();
                        break;
                    case AlarmFrequency.EveryFifteenMinutes:
                        if ((SecondsTriggered % 900) == 0)
                            PlayAudio();
                        break;
                    case AlarmFrequency.EveryMinute:
                        if ((SecondsTriggered % 60) == 0)
                            PlayAudio();
                        break;
                    case AlarmFrequency.Continuous:
                        PlayAudioContinuous();
                        break;
                }
            }
            OnRaiseCustomEvent(new AlarmEventArgs(EventIDs.ALARM_BASE | (ushort)Settings.Instance, Settings));
            SecondsTriggered++;
        }

        protected void PlayAudio()
        {
            if (!string.IsNullOrEmpty(Settings.SoundSettings.AudioFilePath))
            {
                alarmSoundPlayer.Dispatcher.Invoke(() =>
                {
                    alarmSoundPlayer.Open(new Uri("." + Settings.SoundSettings.AudioFilePath, UriKind.Relative));
                    alarmSoundPlayer.Play();
                });
            }
        }

        protected void PlayAudioContinuous()
        {
            if (!string.IsNullOrEmpty(Settings.SoundSettings.AudioFilePath))
            {
                alarmSoundPlayer.Dispatcher.Invoke(() =>
                {
                    alarmSoundPlayer.Open(new Uri("." + Settings.SoundSettings.AudioFilePath, UriKind.Relative));
                    alarmSoundPlayer.Play();
                    alarmSoundPlayer.MediaEnded += AlarmSoundPlayer_MediaEnded;
                });
            }
        }

        private void AlarmSoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (IsTriggered)
            {
                alarmSoundPlayer.Dispatcher.Invoke(() =>
                {
                    alarmSoundPlayer?.Stop();
                    alarmSoundPlayer.Play();
                });
            }
        }

        protected void TurnOffAlarm()
        {
            IsTriggered = false;
            SecondsTriggered = 0;
            alarmSoundPlayer.MediaEnded -= AlarmSoundPlayer_MediaEnded;
            alarmSoundPlayer?.Stop();
        }

        protected internal class AlarmConditionProcessor : IEventConsumer
        {
            private double pidValue;
            private AlarmCondition alarmCondition;
            public bool IsMet
            {
                get
                {
                    if (pidValue == PID_NODATA)
                        return false;
                    switch (alarmCondition.Comparator)
                    {
                        case ComparativeOperator.GreaterThan:
                            return (pidValue > alarmCondition.TriggerValue);
                        case ComparativeOperator.LessThan:
                            return (pidValue < alarmCondition.TriggerValue);
                        case ComparativeOperator.EqualTo:
                            return (pidValue == alarmCondition.TriggerValue);
                        case ComparativeOperator.BitMatch:
                            return (((ushort)pidValue & (ushort)alarmCondition.TriggerValue) == alarmCondition.TriggerValue);
                        case ComparativeOperator.BitZero:
                            return (((ushort)pidValue & (ushort)alarmCondition.TriggerValue) == 0);
                        default:
                            return false;
                    }
                }
            }

            public AlarmConditionProcessor(AlarmCondition alarmCondition)
            {
                this.alarmCondition = alarmCondition;
                SubscribeToPid();
            }

            private void SubscribeToPid()
            {
                EventBridge.Instance.SubscribeToEvent(this, EventIDs.PID_BASE | alarmCondition.Pid);
            }

            public void DropCondition()
            {
                EventBridge.Instance.UnsubscribeFromEvent(this, EventIDs.PID_BASE | alarmCondition.Pid);
            }

            public void ConsumeEvent(VMSEventArgs e)
            {
                pidValue = (e as VMSPidValueEventArgs).value;
            }
        }
    }
}
