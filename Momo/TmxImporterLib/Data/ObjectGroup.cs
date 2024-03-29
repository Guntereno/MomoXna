﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TmxImporterLib.Data
{
    public class ObjectGroup : Layer
    {
        public Dictionary<string, Object> Objects { get; private set; }

        public override void ImportXmlNode(System.Xml.XmlNode layerNode, ContentImporterContext context)
        {
            base.ImportXmlNode(layerNode, context);
          
            System.Xml.XmlNodeList objectNodes = layerNode.SelectNodes("object");

            Objects = new Dictionary<string, Object>();
            foreach (System.Xml.XmlNode objectNode in objectNodes)
            {
                Object obj = new Object();
                obj.ImportXmlNode(objectNode, context);

                // Tiled has no requirement that names be unique, but we do
                string name = obj.Name;
                // Use a default of the layer's name if none provided
                if (name == null || name == "")
                {
                    name = String.Format("{0}_{1:000}", layerNode.Name, 0);
                }
                int curNum = 0;
                while (Objects.Keys.Contains<string>(name))
                {
                    name = String.Format("{0}_{1:000}", name, curNum);
                    curNum++;
                }
                obj.Name = name;

                Objects[obj.Name] = obj;
            }
        }
    }
}
