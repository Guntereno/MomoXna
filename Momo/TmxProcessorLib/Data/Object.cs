using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework;


namespace TmxProcessorLib.Data
{
    public class Object
    {
        public PropertySheet Properties { get; private set; }
        public string Name { get; set; }
        public string Type { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Dimensions { get; private set; }

        public Object()
        {
            Properties = new PropertySheet();
        }

        public void ImportXmlNode(System.Xml.XmlNode objectNode, ContentImporterContext context)
        {
            if (objectNode.Attributes["name"] != null)
            {
                Name = objectNode.Attributes["name"].Value;
            }

            context.Logger.LogMessage("OBJECT: {0}", Name);

            if (objectNode.Attributes["type"] != null)
            {
                Type = objectNode.Attributes["type"].Value;
            }

            Vector2 position = new Vector2();
            
            Debug.Assert(objectNode.Attributes["x"] != null);
            position.X = (float)int.Parse(objectNode.Attributes["x"].Value);

            Debug.Assert(objectNode.Attributes["y"] != null);
            position.Y = (float)int.Parse(objectNode.Attributes["y"].Value);

            Position = position;


            if ((objectNode.Attributes["height"] != null) && (objectNode.Attributes["width"] != null))
            {
                Vector2 dimensions = new Vector2();
                dimensions.X = (float)int.Parse(objectNode.Attributes["width"].Value);
                dimensions.Y = (float)int.Parse(objectNode.Attributes["height"].Value);
                Dimensions = dimensions;

                // Offset the position so it's in the center of the object
                Vector2 offset = dimensions * 0.5f;
                Position = Position + offset;
            }
            else
            {
                Dimensions = Vector2.Zero;
            }

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
