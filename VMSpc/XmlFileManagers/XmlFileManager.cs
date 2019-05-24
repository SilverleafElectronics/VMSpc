﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

/**********************************************************************************
                       XmlFileManager Class
Description: 
            Class for obtaining, reading, creating, and updating 
            configuration files. It is preferable to use one of the
            child classes below rather than manually
            doing file management throughout the program. If you need
            more custom behavior, add it to one of the child classes.

Usage:      Instantiate a new XmlFileManager object with the name
            of the xml configuration file. A new Configuration 
            object is to be instantiated for each config file

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
********************************************************************************/

namespace VMSpc.XmlFileManagers
{
    public struct WindowPlacement
    {
        public int Length;
        public int flags;
        public int showCmd;
        public int pointMinX;
        public int pointMinY;
        public int pointMaxX;
        public int pointMaxY;
        public int rcnpTopLeftX;
        public int rcnpTopLeftY;
        public int rcnpBottomRightX;
        public int rcnpBottomRightY;
    }

    public abstract class XmlFileManager
    {
        //Protected Properties
        protected XmlDocument xmlDoc;
        protected string docName;

        //Constructor
        public XmlFileManager(string docname)
        {
            xmlDoc = new XmlDocument();
            docName = docname;
            initialize();
        }

        //Protected Methods
        protected void initialize()
        {
            using (StreamReader fs = new StreamReader(docName, Encoding.GetEncoding("ISO-8859-1")))
                xmlDoc.Load(fs);
        }

        protected string getNodeValueByTagName(string tagname)
        {
            XmlNodeList node = xmlDoc.GetElementsByTagName(tagname);
            if (node.Count > 1)
                return Constants.STR_USAGEERR;
            if (node.Count == 0)
                return Constants.STR_NODATA;
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
    }
}