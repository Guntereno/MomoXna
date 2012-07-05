using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.IO;
using System.Xml;


namespace MomoMap.Data
{
    public class TmxData
    {
        public enum TypeId
        {
            Orthogonal = 0,
            Isometric = 1
        }

        enum TriggerType
        {
            Invalid = -1,

            KillCount = 0
        }

        public TypeId Type { get; private set; }
        public Point Dimensions { get; private set; }
        public Point TileDimensions { get; private set; }

        public PropertySheet Properties { get; private set; }

        public String FileName { get; private set; }

        public List<Tileset> Tilesets { get; private set; }
        public List<Layer> Layers { get; private set; }
        public List<TileLayer> TileLayers { get; private set; }
        public List<ObjectGroup> ObjectGroups { get; private set; }

        public TmxData(string fileName)
        {
            FileName = fileName;
        }

        // Function initialises the map data from the given XmlDocument
        public void ImportXmlDoc(System.Xml.XmlDocument xmlDoc, ContentImporterContext context)
        {
            Properties = new PropertySheet();
            Tilesets = new List<Tileset>();
            Layers = new List<Layer>();
            TileLayers = new List<TileLayer>();
            ObjectGroups = new List<ObjectGroup>();

            System.Xml.XmlNode mapNode = xmlDoc.GetElementsByTagName("map").Item(0);

            ParseMapNode(mapNode);

            foreach (XmlNode childNode in mapNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "properties":
                        {
                            Properties.ImportXmlNode(childNode, context);
                        }
                        break;

                    case "tileset":
                        {
                            Tileset tileset = new Tileset(this);
                            tileset.ImportXmlNode(childNode, context);
                            Tilesets.Add(tileset);
                        }
                        break;

                    case "layer":
                        {
                            TileLayer tileLayer = new TileLayer();
                            tileLayer.ImportXmlNode(childNode, context);
                            Layers.Add(tileLayer);
                            TileLayers.Add(tileLayer);
                        }
                        break;

                    case "objectgroup":
                        {
                            ObjectGroup objectGroup = new ObjectGroup();
                            objectGroup.ImportXmlNode(childNode, context);
                            Layers.Add(objectGroup);
                            ObjectGroups.Add(objectGroup);
                        }
                        break;
                }
            }
        }

        private void ParseMapNode(System.Xml.XmlNode mapNode)
        {
            // Parse type
            string typeString = mapNode.Attributes["orientation"].Value;
            switch (typeString)
            {
                case "orthogonal":
                    Type = TypeId.Orthogonal;
                    break;

                case "isometric":
                    Type = TypeId.Orthogonal;
                    break;

                default:
                    throw new PipelineException("Unsupported orientation type `{0}`", typeString);
            }

            // Parse dimensions
            Point dimensions = new Point();
            dimensions.X = int.Parse(mapNode.Attributes["width"].Value);
            dimensions.Y = int.Parse(mapNode.Attributes["height"].Value);
            Dimensions = dimensions;

            // Parse tile dimensions
            Point tileDimensions = new Point();
            tileDimensions.X = int.Parse(mapNode.Attributes["tilewidth"].Value);
            tileDimensions.Y = int.Parse(mapNode.Attributes["tileheight"].Value);
            TileDimensions = tileDimensions;
        }

    }
}
