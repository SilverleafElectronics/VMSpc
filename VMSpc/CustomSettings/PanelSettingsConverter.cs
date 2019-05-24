using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace VMSpc.CustomSettings
{
    public class PanelSettingsConverter<ClassType> : TypeConverter where ClassType : new()
    {
        protected XmlDocument doc;
        protected XmlNode panelNode;

        //Verifies that we can read the source from the xml file
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        //converts the stringified xml into our configuration object, then loads it into memory
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                VmsSerializer<ClassType> vmsserializer = new VmsSerializer<ClassType>();
                ClassType panel = vmsserializer.DeserializeObject((string)value);
                return panel;
            }
            return base.ConvertFrom(context, culture, value);
        }

        //converts our configuration object into xml, the saves it to VMSpc.exe.config
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                VmsSerializer<ClassType> vmsserializer = new VmsSerializer<ClassType>();
                return vmsserializer.serializeObject((ClassType)value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
