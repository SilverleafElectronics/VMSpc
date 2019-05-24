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

/*
 * VmsSerializer
 * - Accepts a generic <ClassType> parameter
 * - serializeObject: converts an object to xml
 * - DeserializeObject: converts xml to the provided type
 */
namespace VMSpc.CustomSettings
{ 
    public class VmsSerializer<ClassType> where ClassType : new()
    {
        public string serializeObject(object obj)
        {
            System.Xml.XmlDocument doc = new XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            try
            {
                serializer.Serialize(stream, obj);
                stream.Position = 0;
                doc.Load(stream);
                return doc.InnerXml;
            }
            catch
            {
                throw;
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        public ClassType DeserializeObject(string XmlString)
        {
            ClassType myObject = new ClassType();
            System.IO.StringReader read = new StringReader(XmlString);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(myObject.GetType());
            System.Xml.XmlReader reader = new XmlTextReader(read);
            try
            {
                myObject = (ClassType)serializer.Deserialize(reader);
                return myObject;
            }
            catch
            {
                throw;
            }
            finally
            {
                reader.Close();
                read.Close();
                read.Dispose();
            }
        }
    }
}
