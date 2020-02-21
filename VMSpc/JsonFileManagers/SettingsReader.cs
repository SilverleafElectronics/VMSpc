using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.UI.TireMaps;
using VMSpc.VEnum.Parsing;

namespace VMSpc.JsonFileManagers
{
    //TODO - add window placement
    public class SettingsContents : IJsonContents
    {
        public bool
            autoRestartFlag,
            autoDataLogFlag,
            logWhenEngineOff,
            requestFuelmenter,
            requestOdometer,
            showSplashScreen,
            useClipping,
            usbDelay,
            lockAllGauges;
        public ushort
            rollingBufferSize,
            odometerPid,
            ipPort,
            comPort;
        public string
            screenFilePath,
            jibPlayerFilePath,
            rawLogFilePath,
            ipAddress,
            engineName;
        public JibType 
            jibType;
        public EngineModel
            engineModel;
        public TpmsType
            tpmsType;
        public ParseBehavior
            globalParseBehavior;
        public TireMapType
            tireMapType;
    }

    public class SettingsReader : JsonFileReader<SettingsContents>
    {
        public SettingsReader() : base(cwd + "/configuration/settings.json")
        {
        }

        protected override SettingsContents GetDefaultContents()
        {
            return new SettingsContents()
            {
                lockAllGauges = false,
                autoRestartFlag = true,
                showSplashScreen = false,
                useClipping = true,
                usbDelay = false,
                requestFuelmenter = true,
                requestOdometer = true,
                autoDataLogFlag = false,
                logWhenEngineOff = false,

                rollingBufferSize = 100,
                odometerPid = 245,
                ipPort = 51966,
                comPort = 1,

                engineName = "",
                ipAddress = "169.254.0.2",
                jibPlayerFilePath = Directory.GetCurrentDirectory() + "/rawlogs/j1939_demo.log",
                rawLogFilePath = Directory.GetCurrentDirectory() + "/rawlogs/j1939_demo.log",
                screenFilePath = Directory.GetCurrentDirectory() + "/configuration/screens/default.scr.json",

                engineModel = EngineModel.NONE,
                tpmsType = TpmsType.NONE,
                globalParseBehavior = ParseBehavior.PARSE_ALL,
                jibType = JibType.USB,
                tireMapType = TireMapType.SIX_WHEEL,
            };
        }

        //TODO
        private dynamic getDefaultWindowPlacement()
        {
            dynamic windowPlacement = new ExpandoObject();
            return windowPlacement;
        }
    }
}
