using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace VMSpc.Enums.Parsing
{
    public enum JibType
    {
        [Description("None")]
        NONE = 0,
        [Description("USB")]
        USB = 1,
        [Description("Serial")]
        SERIAL = 2,
        [Description("Wi-Fi")]
        WIFI = 3,
        [Description("Log Player")]
        LOGPLAYER = 4
    };
    public enum OdometerType
    {
        [Description("Standard")]
        Standard = 0,
        [Description("Cummins")]
        Cummins = 1,
    }
    public enum J1708DiagnosticIDType
    {
        [Description("PID")]
        PID,
        [Description("SID")]
        SID
    }
    public enum EngineModel
    {
        NONE = 0
    };
    public enum TpmsType
    {
        [Description("None")]
        None = 0,
        [Description("TST")]
        TST = 1,
        [Description("Pressure Pro")]
        PressurePro = 2
    };
    public enum TireStatus
    {
        [Description("None")]
        None = 0,
        [Description("No Data")]
        NoData = 1,
        [Description("Okay")]
        Okay = 2,
        [Description("Warning")]
        Warning = 3,
        [Description("Alert")]
        Alert = 4,
    }
    public enum ParseBehavior
    {
        [Description("Ignore All Data")]
        PARSE_NONE = 0,
        [Description("Parse All Data")]
        PARSE_ALL = 1,
        [Description("Ignore J1939 Data")]
        IGNORE_1939 = 2,
        [Description("Ignore J1708 Data")]
        IGNORE_1708 = 3,
        [Description("Prioritize J1939 Data")]
        PRIORITIZE_1939 = 4,
        [Description("Prioritize J1708 Data")]
        PRIORITIZE_1708 = 5,
    };
    public enum ParseStatus
    {
        [Description("Parsed")]
        Parsed = 0,
        [Description("Not Parsed")]
        NotParsed = 1,
        [Description("Partially Parsed")]
        PartiallyParsed = 2,
    }
    public enum DataBusType
    {
        [Description("None")]
        NONE = 0,
        [Description("J1708")]
        J1708 = 1,
        [Description("J1939")]
        J1939 = 2,
    };
    public enum MessageError
    {
        Ok = 0,
        UnrecognizedMessage,
        J1708ChecksumFailure,
        J1708InvalidMessage,
        J1939ChecksumFailure,
        J1939InvalidMessage,
        Unkown,
    }
    public enum VMSDataSource
    {
        None,
        J1708,
        J1939,
        Inferred,
    }
}

namespace VMSpc.Enums
{
    public static class EnumUtils
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<string> GetDescriptions<T>()
        {
            var values = GetValues<T>();
            List<string> list = new List<string>();
            foreach (IConvertible value in values)
            {
                var valueAsString = value.GetDescription();
                if (string.IsNullOrEmpty(valueAsString))
                    valueAsString = value.ToString();
                list.Add(valueAsString);
            }
            return list;
        }

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}