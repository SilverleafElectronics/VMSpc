using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.IO;

namespace VMSpc.CustomSettings
{
    /*
    public struct PanelCoordinates
    {
        public int TopLeft_X;
        public int TopLeft_Y;
        public int BottomRight_X;
        public int BottomRight_Y;
    }
    public struct PanelColor
    {
        public int Red;
        public int Green;
        public int Blue;
    }

    //Root: Parent of TransmissionGaugeSettings, RecordedSettings, and GaugeSettings
    public class PanelSettings
    {
        public int Number;
        public string ID;
        public PanelCoordinates Rect_Cord;
        public PanelColor Background_Color;
        public PanelColor Base_Color;
        public PanelColor Explicit_Color;
        public int Text_Position;
        public int Use_Static_Color;
    }

    //HistogramSettings
    [TypeConverter(typeof(PanelSettingsConverter<HistogramSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class HistogramSettings : PanelSettings
    {
        public int Number_Of_PIDs;
        public int Show_In_Metric;
        public int[] PID_List;
        public int Scan_Rate;
    }

    //ClockSettings
    [TypeConverter(typeof(PanelSettingsConverter<ClockSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class ClockSettings : PanelSettings
    {
        public int Show_Date;
        public int Show_AMPM;
    }

    //MessageBoxSettings
    [TypeConverter(typeof(PanelSettingsConverter<MessageBoxSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class MessageBoxSettings : PanelSettings
    {
        public int Num_Lines;
        public int PID;
        public int PID_Limited;
    }

    //PictureSettings
    [TypeConverter(typeof(PanelSettingsConverter<PictureSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class PictureSettings : PanelSettings
    {
        public string BMP_File_Name;
    }

    //DiagnosticGaugeSettings
    [TypeConverter(typeof(PanelSettingsConverter<DiagnosticGaugeSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class DiagnosticGaugeSettings : PanelSettings
    {
        public PanelColor Warning_Color;
    }

    //TextGaugeSettings
    [TypeConverter(typeof(PanelSettingsConverter<TextGaugeSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class TextGaugeSettings : PanelSettings
    {
        public int Text_Length;
        public string Text;
    }

    //TransmissionGaugeSettings
    [TypeConverter(typeof(PanelSettingsConverter<TransmissionGaugeSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class TransmissionGaugeSettings : PanelSettings
    {
        public int Show_Attained;
        public int Show_Selected;
    }

    //Parent of OdometerSettings and TankMinderSettings
    public class RecordedSettings : PanelSettings
    {
        public int Show_MPG;
        public int Show_Captions;
        public int Show_Units;
        public int Show_In_Metric;
        public string File_Name;
    }

    //OdometerSettings
    [TypeConverter(typeof(PanelSettingsConverter<OdometerSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class OdometerSettings : RecordedSettings
    {
        public int Show_Fuel_Locked;
        public int Show_Hours;
        public int Show_Miles;
        public int Show_Speed;
    }

    //TankMinderSettings
    [TypeConverter(typeof(PanelSettingsConverter<TankMinderSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class TankMinderSettings : RecordedSettings
    {
        public int Show_Fuel;
        public int Show_Miles_To_Empty;
        public int Use_Rolling_MPG;
        public int Layout_Horizontal;
        public int Tank_Size;
        public int Tank_Size_Metric;
    }

    //Parent of SimpleGaugeSettings, ScanGaugeSettings, and RoundGaugeSettings
    public class GaugeSettings : PanelSettings
    {
        public int Show_Spot;
        public int Show_Name;
        public int Show_Value;
        public int Show_Unit;
        public int Show_Graph;
        public int Show_Abbreviation;
        public int Show_In_Metric;
    }

    //ScanGaugeSettings
    [TypeConverter(typeof(PanelSettingsConverter<ScanGaugeSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class ScanGaugeSettings : GaugeSettings
    {
        public int[] PID_List;
    }

    //SimpleGaugeSettings. Also, parent of RoundGaugeSettings
    [TypeConverter(typeof(PanelSettingsConverter<SimpleGaugeSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class SimpleGaugeSettings : GaugeSettings
    {
        public int PID;
    }

    //RoundGaugeSettings
    [TypeConverter(typeof(PanelSettingsConverter<RoundGaugeSettings>))]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class RoundGaugeSettings : SimpleGaugeSettings
    {
        public PanelColor Gauge_Color;
        public PanelColor Fill_Color;
    }
    */
}
