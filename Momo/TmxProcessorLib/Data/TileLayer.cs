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
        public uint[] Data { get; private set; }

        private int m_index;

        public TileLayer(int index)
        {
            m_index = index;
        }

        public int GetIndex()
        {
            return m_index;
        }


        public override void ImportXmlNode(System.Xml.XmlNode layerNode, ContentImporterContext context)
        {
            base.ImportXmlNode(layerNode, context);

            Data = new uint[Dimensions.X * Dimensions.Y];

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
                            Data[i] = index;

                            // These are currently ignored for now
                            //bool flipX = (data & 0x80000000) != 0;
                            //bool flipY = (data & 0x40000000) != 0;
                        }
                    }
                    break;

                default:
                    throw new PipelineException("Unsupported encoding type {0}", encoding);
            }

        }
    }
}
