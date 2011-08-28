using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content.Pipeline;


namespace TmxProcessorLib.Content
{
    public class Object
    {
        public PropertySheet Properties { get; private set; }
        public string Name { get; set; }
        public string Type { get; private set; }
        public Microsoft.Xna.Framework.Vector2 Position { get; private set; }

        public Object()
        {
            Properties = new PropertySheet();
        }

        public void ImportXmlNode(System.Xml.XmlNode objectNode, ContentImporterContext context)
        {
            Debug.Assert(objectNode.Attributes["name"] != null);
            Name = objectNode.Attributes["name"].Value;

            context.Logger.LogMessage("OBJECT: {0}", Name);

            Debug.Assert(objectNode.Attributes["type"] != null);
            Type = objectNode.Attributes["type"].Value;

             Microsoft.Xna.Framework.Vector2 position = new Microsoft.Xna.Framework.Vector2();        
            
            Debug.Assert(objectNode.Attributes["x"] != null);
            position.X = (float)int.Parse(objectNode.Attributes["x"].Value);

            Debug.Assert(objectNode.Attributes["y"] != null);
            position.Y = (float)int.Parse(objectNode.Attributes["y"].Value);

            Position = position;

            context.Logger.LogMessage("POSITION: {0}", Position);

            System.Xml.XmlNode propertiesNode = objectNode.SelectSingleNode("properties");
            if (propertiesNode != null)
            {
                Properties = new PropertySheet();
                Properties.ImportXmlNode(propertiesNode, context);
            }
        }

    }
}
