using System;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MomoMap.Data
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

            // Format defaults to XML
            Encoding encoding = Encoding.Xml;
            if (dataNode.Attributes["encoding"] != null)
            {
                encoding = (Encoding)Enum.Parse(typeof(Encoding), dataNode.Attributes["encoding"].Value, true);
            }

            switch (encoding)
            {
                case Encoding.Csv:
                    {
                        // Merge all data into one line
                        string csv = dataNode.InnerText.Replace("\n", "");
                        string[] indices = csv.Split(new[] { ',' });

                        for (int i = 0; i < indices.Length; i++)
                        {
                            uint data = uint.Parse(indices[i]);

                            Data[i] = CreateTileFromGid(data);
                        }
                    }
                    break;

                case Encoding.Xml:
                    {
                        int tileCount = dataNode.ChildNodes.Count;
                        for (int i = 0; i < tileCount; ++i )
                        {
                            XmlNode childNode = dataNode.ChildNodes[i];
                            if (childNode.Name == "tile")
                            {
                                XmlAttribute gidAttribute = childNode.Attributes["gid"];
                                if (gidAttribute == null)
                                    throw new PipelineException("No 'gid' node found in tile {0} with xml {1}", Name, layerNode.InnerXml);
                                uint data = uint.Parse(gidAttribute.Value);

                                Data[i] = CreateTileFromGid(data);
                            }
                        }
                    }
                    break;

                // base64 is currently unsupported

                default:
                    throw new PipelineException("Unsupported encoding type {0}", encoding);
            }

        }

        private static TileEntry CreateTileFromGid(uint data)
        {
            uint index = data & 0x3FFFFFFF;

            bool flipX = (data & 0x80000000) != 0;
            bool flipY = (data & 0x40000000) != 0;

            return new TileEntry(index, flipX, flipY);
        }

        private enum Encoding
        {
            Xml,
            Csv,
            Base64,
        };
    }
}
