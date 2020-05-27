using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMSpc.UI.TireMaps;
using VMSpc.Enums.Parsing;
using VMSpc.Enums.UI;
using System.Windows;

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
            ipPort,
            comPort;
        public string
            screenFilePath,
            meterFilePath,
            jibPlayerFilePath,
            rawLogFilePath,
            ipAddress,
            engineFilePath;
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
        public OdometerType
            odometerType;
        public int
            selectedColorPaletteId;
        public double
            WindowHeight,
            WindowWidth,
            WindowTop,
            WindowLeft;
    }

    public class SettingsReader : JsonFileReader<SettingsContents>
    {
        public SettingsReader() : base("\\configuration\\settings.json")
        {
        }

        protected override SettingsContents GetDefaultContents()
        {
            return new SettingsContents()
            {
                selectedColorPaletteId = 1,
                lockAllGauges = false,
                autoRestartFlag = true,
                showSplashScreen = false,
                useClipping = false,
                usbDelay = false,
                requestFuelmenter = true,
                requestOdometer = true,
                autoDataLogFlag = false,
                logWhenEngineOff = false,

                WindowWidth = SystemParameters.VirtualScreenWidth / 2,
                WindowHeight = SystemParameters.VirtualScreenHeight / 2,
                WindowLeft = SystemParameters.VirtualScreenWidth / 4,
                WindowTop = SystemParameters.VirtualScreenHeight / 4,

                rollingBufferSize = 100,
                odometerType = OdometerType.Standard,
                ipPort = 51966,
                comPort = 1,

                engineFilePath = "\\engines\\Caterpillar 3126 (250 Hp).ENG",
                ipAddress = "169.254.0.2",
                jibPlayerFilePath = "\\rawlogs\\j1708_demo.vms",
                rawLogFilePath = "\\rawlogs\\j1708_demo.vms",
                screenFilePath = "\\configuration\\screens\\default.scr.json",
                meterFilePath = "\\configuration\\odometer.json",

                engineModel = EngineModel.NONE,
                tpmsType = TpmsType.NONE,
                globalParseBehavior = ParseBehavior.PARSE_ALL,
                jibType = JibType.LOGPLAYER,
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
