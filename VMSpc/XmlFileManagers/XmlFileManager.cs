using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using static VMSpc.Constants;

/**********************************************************************************
                       XmlFileManager Class
Description: 
            Class for obtaining, reading, creating, and updating 
            configuration files. It is preferable to use one of the
            child classes below rather than manually
            doing file management throughout the program. If you need
            more custom behavior, add it to one of the child classes.

Usage:      Instantiate a new XmlFileManager object with the name
            of the xml configuration file. A new Configuration inherited
            class should be designed for each config file

Methods:
            initialize()
                - Loads/reloads the associated file into the DOM tree
                - NOTE: This should rarely be used outside of the constructor
            getNodeValueByTagName(string tagname)
                - returns the node value for the specified tag name
                - NOTE: when using this method, you must be certain
                  that there is only one of the specified tag in the
                  document. If there are more than one, it will return
                  "USAGE_ERR"
            getAllNodesByTagName(string tagname)
                - returns the list of nodes with the specified tag name

NOTE: all child classes of XmlFileManager are implemented as singletons. This
      allows them to be globally accessed. See https://csharpindepth.com/articles/singleton
      for a walkthrough on implementing singleton classes.
********************************************************************************/

namespace VMSpc.XmlFileManagers
{
    public abstract class XmlFileManager
    {
        //Protected Properties
        protected XmlDocument xmlDoc;
        protected string docName;

        //Constructor
        public XmlFileManager(string docname)
        {
            xmlDoc = new XmlDocument();
            docName = "./configuration/" + docname;
            if (!File.Exists(docName))
                CreateTemplate();
            else
                Initialize();
        }

        #region Construction Helpers
        protected void Initialize()
        {
            using (StreamReader fs = new StreamReader(docName, Encoding.GetEncoding("utf-8")))
                xmlDoc.Load(fs);
        }

        /// <summary> 
        /// Generates the template necessary for bare VMSpc operation. Child classes are responsible for 
        /// generating their own template, and should store this template in `xmlDoc` before calling this base method
        /// </summary>
        protected virtual void CreateTemplate()
        {
            File.Create(docName).Close();   //close right away so we can load it into the StreamReader
            xmlDoc.Save(docName);
            Initialize();
        }
        #endregion //Construction Helpers

        #region XML Reading
        protected string getNodeValueByTagName(string tagname)
        {
            XmlNodeList node = xmlDoc.GetElementsByTagName(tagname);
            if (node.Count > 1)
                return STR_USAGEERR;
            if (node.Count == 0)
                return STR_NODATA;
            else
            {
                return node[0].InnerXml;
            }
        }

        protected XmlNode getNodeByTagName(string tagname)
        {
            XmlNodeList node = xmlDoc.GetElementsByTagName(tagname);
            if (node.Count > 1)
                return null;
            if (node.Count == 0)
                return null;
            else
                return node[0];
        }

        protected XmlNode getNodeByTagAndAttr(string tagname, string attrName, string attrVal)
        {
            XmlNodeList nodes = xmlDoc.GetElementsByTagName(tagname);
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes[attrName].Value == attrVal)
                {
                    return node;
                }
            }
            return null;
        }

        protected XmlNodeList getAllNodesByTagName(string tagname)
        {
            return xmlDoc.GetElementsByTagName(tagname);
        }

        protected string getAttributeValueByTagName(string tagname, string attr)
        {
            XmlNode node = getNodeByTagName(tagname);
            return node.Attributes[attr].Value;
        }

        protected string getAttributeValueByNode(XmlNode node, string attr)
        {
            return node.Attributes[attr].Value;
        }
        #endregion //XML Reading

        #region XML Writing

        /// <summary>
        /// Overwrites the current document with the specified newXml string parameter
        /// </summary>
        protected void OverwriteFile(string newXml)
        {
            xmlDoc.LoadXml(newXml);
        }

        public XmlNode AddNodeToParentNode(XmlNode parent, string childNodeName)
        {
            XmlElement newNode = xmlDoc.CreateElement(childNodeName);
            parent.AppendChild(newNode);
            return newNode;
        }

        public XmlNode AddNodeToParentTag(string parentTag, string childNodeName)
        {
            XmlNode parent = getNodeByTagName(parentTag);
            XmlElement newNode = xmlDoc.CreateElement(childNodeName);
            parent.AppendChild(newNode);
            return newNode;
        }

        public void AddAttributeToNode(XmlNode node, string attrName, string attrValue)
        {
            XmlNode attr = xmlDoc.CreateNode(XmlNodeType.Attribute, attrName, "");
            attr.Value = attrValue;
            node.Attributes.SetNamedItem(attr);
        }

        public void ChangeAttribute(XmlNode node, string attrName, string attrValue)
        {
            node.Attributes[attrName].InnerText = attrValue;
        }

        public virtual void SaveConfiguration()
        {
            foreach (XmlNode node in xmlDoc)
            {
                node.InnerText.Replace("°", "&#176;");
                node.InnerText.Replace("Â", "");
            }
            xmlDoc.Save(docName);
        }

        #endregion //XML Writing
    }
}
