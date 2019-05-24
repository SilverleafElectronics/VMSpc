using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace VMSpc.CustomSettings
{
    public class PanelSettingsConverter<T> : TypeConverter where T : new()
    {
        protected XmlDocument doc;
        protected XmlNode panelNode;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                VmsSerializer<T> vmsserializer = new VmsSerializer<T>();
                T panel = vmsserializer.DeserializeObject((string)value);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public virtual void ConvertAdditionalFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {

        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {

            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public virtual void ConvertAdditionalTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {

        }
    }
}
