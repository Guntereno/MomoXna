using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TmxProcessorLib.Content
{
    public class TileLayer : Layer
    {
        public uint[] Data { get; private set; }

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
                            uint value = uint.Parse(indices[i]);
                            Data[i] = value & 0x3FFFFFFF;

                            // These are currently ignored for now
                            bool x = (value & 0x80000000) != 0;
                            bool y = (value & 0x40000000) != 0;
                        }
                    }
                    break;

                default:
                    throw new PipelineException("Unsupported encoding type {0}", encoding);
            }

        }

        public override void Process(TmxData parent, ContentProcessorContext context)
        {

        }

        public override void Write(ContentWriter output)
        {
            base.Write(output);

            output.Write(Data.Length);
            foreach (int i in Data)
            {
                output.Write(i);
            }
        }
    }
}
