using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TmxProcessorLib.Data
{
    public class TileLayer : Layer
    {
        public class TileEntry
        {
            public uint Index { get; private set; }
            public bool FlipX { get; private set; }
            public bool FlipY { get; private set; }

            public TileEntry(uint index, bool flipX, bool flipY)
            {
                Index = index;
                FlipX = flipX;
                FlipY = flipY;
            }

        }

        public TileEntry[] Data { get; private set; }

        public TileLayer()
        {
        }

        public override void ImportXmlNode(System.Xml.XmlNode layerNode, ContentImporterContext context)
        {
            base.ImportXmlNode(layerNode, context);

            Data = new TileEntry[Dimensions.X * Dimensions.Y];

            System.Xml.XmlNode dataNode = layerNode.SelectSingleNode("data");
            if (dataNode == null)
                throw new PipelineException("No 'data' node found in layer {0} with xml {1}", Name, layerNode.InnerXml);

            string encoding = dataNode.Attributes["encoding"].Value;
            if (encoding == null)
                throw new PipelineException("No 'encoding' node found in data node of layer {0} with xml {1}", Name, layerNode.InnerXml);

            switch (encoding)
            {
                case "csv":
                    {
                        // Merge all data into one line
                        string csv = dataNode.InnerText.Replace("\n", "");
                        string[] indices = csv.Split(new[] { ',' });

                        for (int i = 0; i < indices.Length; i++)
                        {
                            uint data = uint.Parse(indices[i]);

                            uint index = data & 0x3FFFFFFF;
    
                            bool flipX = (data & 0x80000000) != 0;
                            bool flipY = (data & 0x40000000) != 0;

                            Data[i] = new TileEntry(index, flipX, flipY);
                        }
                    }
                    break;

                default:
                    throw new PipelineException("Unsupported encoding type {0}", encoding);
            }

        }
    }
}
