using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Enums.Parsing;
using VMSpc.Parsers;
using System.Timers;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.AdvancedParsers.Tires
{
    public abstract class Tire
    {
        public TireStatus TireStatus { get; internal set; } = TireStatus.None;
        public ushort index { get; internal set; }
        public int id { get; internal set; } = 0;
        public bool sensorReporting { get; internal set; } = false;
        public abstract double DisplayPressure { get; }
        public abstract double DisplayTemperature { get; }
        public DateTime lastUpdate { get; set; }
        public virtual string TireStatusString()
        {
            switch (TireStatus)
            {
                case TireStatus.NoData:
                    return "No Data";
                case TireStatus.Warning:
                case TireStatus.Alert:
                case TireStatus.Okay:
                    return $"Pressure: {DisplayPressure:F2} PSI   Temperature:  {DisplayTemperature:F2} Deg    ";
                case TireStatus.None:
                default:
                    return "No Sensor Assigned";
            }
        }
    }

    public abstract class TireManager : IEventPublisher, IEventConsumer, ISingleton
    {
        public const int MAX_TIRE_POSITIONS = 20;
        public const int MAX_TIRES = 16;

        public Tire[] Tires;
        public Timer PublishTimer;

        static TireManager()
        {
        }
        public static TireManager Instance { get; private set; }
        public TireManager() 
        {
            Tires = new Tire[MAX_TIRE_POSITIONS];
            PublishTimer = Constants.CREATE_TIMER(PublishTireEvents, 2000);
            EventBridge.Instance.AddEventPublisher(this);
        }

        private void Destroy()
        {
            PublishTimer?.Stop();
            PublishTimer = null;
            EventBridge.Instance.RemoveEventPublisher(this);
            EventBridge.Instance.UnsubscribeFromAllEvents(this);
            Instance = null;
        }

        public static void Initialize()
        {
            if (Instance != null)
            {
                Instance.Destroy();
            }
            switch (ConfigManager.Settings.Contents.tpmsType)
            {
                case Enums.Parsing.TpmsType.PressurePro:
                    Instance = new PProTireManager();
                    break;
                case Enums.Parsing.TpmsType.TST:
                    Instance = new TSTTireManager();
                    break;
            }
        }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

        private void PublishTireEvents()
        {
            for (int i = 0; i < MAX_TIRE_POSITIONS; i++)
            {
                if (Tires[i] != null)
                {
                    var e = new TireEventArgs(Tires[i]);
                    RaiseVMSEvent?.Invoke(this, e);
                }
            }
        }

        public void ConsumeEvent(VMSEventArgs e)
        {
            var segment = (e as VMSRawDataEventArgs)?.messageSegment;
            if (segment != null)
            {
                Parse(segment);
            }
        }

        public abstract void Parse(CanMessageSegment segment);
        public abstract void LearnTire(byte instance);
        public abstract void ClearTire(byte instance);
        public abstract void AbortLearn();
    }
}
