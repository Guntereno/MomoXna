using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework;


namespace TmxImporterLib.Data
{
    public class Object: INamed
    {
        public PropertySheet Properties { get; private set; }
        public string Name { get; set; }
        public string Type { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Dimensions { get; private set; }
        public Polygon Polygon { get; private set; }
        public float Orientation { get; private set; }

        public Object()
        {
            Properties = new PropertySheet();
            Orientation = 0.0f;
        }

        public void ImportXmlNode(System.Xml.XmlNode objectNode, ContentImporterContext context)
        {
            if (objectNode.Attributes["name"] != null)
            {
                Name = objectNode.Attributes["name"].Value;
            }

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

            System.Xml.XmlNode propertiesNode = objectNode.SelectSingleNode("properties");
            if (propertiesNode != null)
            {
                Properties = new PropertySheet();
                Properties.ImportXmlNode(propertiesNode, context);

                if (Properties.Properties.ContainsKey("orientation"))
                {
                    Orientation = float.Parse(Properties.Properties["orientation"]);
                    Orientation = MathHelper.ToRadians(Orientation);
                }
            }

            System.Xml.XmlNode polygonNode;
            polygonNode = objectNode.SelectSingleNode("polygon");
            if (polygonNode == null)
            {
                polygonNode = objectNode.SelectSingleNode("polyline");
            }
            if (polygonNode != null)
            {
                Polygon = new Polygon();
                Polygon.ImportXmlNode(polygonNode, context);
            }
        }

    }
}
