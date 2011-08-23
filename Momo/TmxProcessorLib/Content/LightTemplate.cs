using Microsoft.Xna.Framework.Content.Pipeline;
using System.Diagnostics;

namespace TmxProcessorLib.Content
{
    public class LightTemplate
    {
        public string Name { get; private set; }
        public string Type { get; private set; }

        public PropertySheet Properties { get; private set; }

        public void ImportXmlNode(System.Xml.XmlNode templateNode, ContentImporterContext context)
        {
            Debug.Assert(templateNode.Attributes["name"] != null);
            Name = templateNode.Attributes["name"].Value;

            Debug.Assert(templateNode.Attributes["type"] != null);
            Type = templateNode.Attributes["type"].Value;

            System.Xml.XmlNode propertiesNode = templateNode.SelectSingleNode("properties");
            Debug.Assert(propertiesNode != null);
            Properties = new PropertySheet();
            Properties.ImportXmlNode(propertiesNode, context);
        }
    }
}
