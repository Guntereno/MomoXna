﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Diagnostics;

namespace TmxImporterLib.Data
{
    public class PropertySheet
    {
        public Dictionary<string, string> Properties { get; private set; }

        public void ImportXmlNode(System.Xml.XmlNode propertiesNode, ContentImporterContext context)
        {
            Properties = new Dictionary<string, string>();

            System.Xml.XmlNodeList propertyNodes = propertiesNode.SelectNodes("property");
            foreach (System.Xml.XmlNode property in propertyNodes)
            {
                if (property.Attributes["name"] == null)
                {
                    throw new PipelineException("Property without 'name' attribute!");
                }
                string name = property.Attributes["name"].Value;

                if (property.Attributes["value"] == null)
                {
                    throw new PipelineException("Property {0} without 'value' attribute!", name);
                }
                string value = property.Attributes["value"].Value;

                Properties.Add(name, value);
            }
        }
    }
}
