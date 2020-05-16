using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.Common;
using VMSpc.Parsers;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.AdvancedParsers.Tires
{
    public abstract class TireManager : IEventPublisher, IEventConsumer, ISingleton
    {
        public const int MAX_TIRES = 16;
        static TireManager()
        {

        }
        public static TireManager Instance { get; private set; }
        public TireManager() { }
        public static void Initialize()
        {
            switch (ConfigManager.Settings.Contents.tpmsType)
            {
                case Enums.Parsing.TpmsType.PPRO:
                    break;
                case Enums.Parsing.TpmsType.TST:
                    Instance = new TSTTireManager();
                    break;
            }
        }

        public event EventHandler<VMSEventArgs> RaiseVMSEvent;

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
