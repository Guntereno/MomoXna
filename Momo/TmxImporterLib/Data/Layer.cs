using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TmxProcessorLib.Data
{
    public class Layer: INamed
    {
        public string Name { get; private set; }
        public Microsoft.Xna.Framework.Point Dimensions { get; private set; }

        public virtual void ImportXmlNode(System.Xml.XmlNode layerNode, ContentImporterContext context)
        {
            Name = layerNode.Attributes["name"].Value;

            Microsoft.Xna.Framework.Point dimensions = new Microsoft.Xna.Framework.Point();
            if (layerNode.Attributes["width"] == null)
                throw new PipelineException("No 'width' attribute found in layer {0} with xml {1}", Name, layerNode.InnerXml);
            dimensions.X = int.Parse(layerNode.Attributes["width"].Value);

            if (layerNode.Attributes["height"] == null)
                throw new PipelineException("No 'firstgid' attribute found in tileset {0} with xml {1}", Name, Name, layerNode.InnerXml);
            dimensions.Y = int.Parse(layerNode.Attributes["height"].Value);

            if ((dimensions.X + dimensions.Y) == 0)
            {
                throw new PipelineException("Layer {0} has no dimensions", Name);
            }

            Dimensions = dimensions;
        }
    }
}
