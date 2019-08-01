using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VMSpc.XmlFileManagers
{
    public sealed class SettingsManager : XmlFileManager
    {
        static SettingsManager() { }
        public static SettingsManager Settings { get; set; } = new SettingsManager();
        public SettingsManager() : base("Settings.xml") { }

        //GETTERS
        public string Version
        {
            get => getNodeValueByTagName("Version");
            set => SetNodeValueByTagName("Version", value, true);
        }
        public int Port
        {
            get => Int32.Parse(getNodeValueByTagName("Port"));
            set => SetNodeValueByTagName("Port", value.ToString());
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
        public ushort OdometerPID
        {
            get => UInt16.Parse(getNodeValueByTagName("Odometer-PID"));
            set => SetNodeValueByTagName("Odometer-PID", value.ToString());
        }
        public int JibType
        {
            get => Int32.Parse(getNodeValueByTagName("Jib-Type"));
            set => SetNodeValueByTagName("Jib-Type", value.ToString());
        }
        public bool UseClipping
        {
            get => Boolean.Parse(getNodeValueByTagName("Use-Clipping"));
            set => SetNodeValueByTagName("Use-Clipping", value.ToString(), true);
        }

        public int ParseMode
        {
            get => Int32.Parse(getNodeValueByTagName("Parse-Mode"));
            set => SetNodeValueByTagName("Parse-Mode", value.ToString());
        }

        public string LogPlayerFileName
        {
            get => getNodeValueByTagName("LogPlayer-FileName");
            set => SetNodeValueByTagName("LogPlayer-FileName", value);
        }

    }
}
