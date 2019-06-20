using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VMSpc.XmlFileManagers
{
    public class SettingsManager : XmlFileManager
    {
        //Constructor
        public SettingsManager() : base("Settings.xml")
        {
        }
        //GETTERS
        public string get_version()
        {
            return getNodeValueByTagName("Version");
        }
        public int get_port()
        {
            return Int32.Parse(getNodeValueByTagName("Port"));
        }
        public int get_commDatabus()
        {
            return Int32.Parse(getNodeValueByTagName("Comm-Databus"));
        }
        public string get_scrnFileName()
        {
            return getNodeValueByTagName("Scrn-File-Name");
        }
        public int get_gaugeLock()
        {
            return Int32.Parse(getNodeValueByTagName("Gauge-Lock"));
        }
        public WindowPlacement get_windowPlacement()
        {
            WindowPlacement windowPlacement = new WindowPlacement();
            XmlNode node = getNodeByTagName("Window-Placement");
            windowPlacement.Length = Int32.Parse(getAttributeValueByNode(node, "Length"));
            windowPlacement.flags = Int32.Parse(getAttributeValueByNode(node, "Length"));
            windowPlacement.showCmd = Int32.Parse(getAttributeValueByNode(node, "showCmd"));
            windowPlacement.pointMinX = Int32.Parse(getAttributeValueByNode(node, "Point-MinX"));
            windowPlacement.pointMinY = Int32.Parse(getAttributeValueByNode(node, "Point-MinY"));
            windowPlacement.pointMaxX = Int32.Parse(getAttributeValueByNode(node, "Point-MaxX"));
            windowPlacement.pointMaxY = Int32.Parse(getAttributeValueByNode(node, "Point-MaxY"));
            windowPlacement.rcnpBottomRightX = Int32.Parse(getAttributeValueByNode(node, "rcNP-top-rightX"));
            windowPlacement.rcnpBottomRightY = Int32.Parse(getAttributeValueByNode(node, "rcNP-top-rightY"));
            windowPlacement.rcnpTopLeftX = Int32.Parse(getAttributeValueByNode(node, "rcNP-top-leftX"));
            windowPlacement.rcnpTopLeftY = Int32.Parse(getAttributeValueByNode(node, "rcNP-top-leftY"));
            return windowPlacement;
        }
        public int get_datalogFrequency()
        {
            return Int32.Parse(getNodeValueByTagName("Frequency"));
        }
        public int get_datalogNumPids()
        {
            return Int32.Parse(getNodeValueByTagName("Num-Pids"));
        }
        public string get_datalogFileName()
        {
            return getNodeValueByTagName("File-Name");
        }
        public int get_autoRestartFlag()
        {
            return Int32.Parse(getNodeValueByTagName("Auto-Restart-Flag"));
        }
        public int get_rollingBuffSize()
        {
            return Int32.Parse(getNodeValueByTagName("Rolling-Buff-Size"));
        }
        public string get_engineName()
        {
            return getNodeValueByTagName("Engine-Name");
        }
        public int get_odometerPID()
        {
            return Int32.Parse(getNodeValueByTagName("Odometer-PID"));
        }
        public int get_autoDataLogFlag()
        {
            return Int32.Parse(getNodeValueByTagName("Auto-Data-Log-Flag"));
        }
        public int get_logWhenEngineOff()
        {
            return Int32.Parse(getNodeValueByTagName("Log-When_engine-Off"));
        }
        public int get_jibType()
        {
            return Int32.Parse(getNodeValueByTagName("Jib-Type"));
        }
        public int get_usbDelay()
        {
            return Int32.Parse(getNodeValueByTagName("Usb-Delay"));
        }
        public int get_tpmsType()
        {
            return Int32.Parse(getNodeValueByTagName("TPMS-Type"));
        }
        public int get_rqstFuelmeter()
        {
            return Int32.Parse(getNodeValueByTagName("Rqst-Fuelmeter"));
        }
        public int get_rqstOdometer()
        {
            return Int32.Parse(getNodeValueByTagName("Rqst-Odometer"));
        }
        public string get_IPAddress()
        {
            return getNodeValueByTagName("IP-Address");
        }
        public int get_IPPort()
        {
            return Int32.Parse(getNodeValueByTagName("IP-Port"));
        }
        public int get_cfgEngineModel()
        {
            return Int32.Parse(getNodeValueByTagName("CFG-Engine_model"));
        }
        public int get_showSplashscreen()
        {
            return Int32.Parse(getNodeValueByTagName("Show-Splashscreen"));
        }

        //SETTERS

    }
}
