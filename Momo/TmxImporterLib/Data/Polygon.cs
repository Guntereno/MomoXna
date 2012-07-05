using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TmxImporterLib.Data
{
    public class Polygon
    {
        public List<Vector2> Points { get; private set; }
        public Boolean Closed { get; private set; }

        public void ImportXmlNode(System.Xml.XmlNode polygonNode, ContentImporterContext context)
        {
            Points = new List<Vector2>();

            if (polygonNode.Attributes["points"] != null)
            {
                string pointsStr = polygonNode.Attributes["points"].Value;

                string[] points = pointsStr.Split(' ');
                foreach (string pointStr in points)
                {
                    string[] axis = pointStr.Split(',');
                    if(axis.Length != 2)
                    {
                        throw new InvalidContentException("Invalid polygon 'points' value: " + pointsStr);
                    }
                    Vector2 curPoint = new Vector2(float.Parse(axis[0]), float.Parse(axis[1]));
                    Points.Add(curPoint);
                }
            }

            Closed = (polygonNode.Name == "polygon");
        }
    }
}
