using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.IO;
using System.Xml;


namespace TmxProcessorLib.Data
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

        public Dictionary<string, Tileset> TilesetsDict { get; private set; }
        public List<Tileset> Tilesets { get; private set; }
        public Dictionary<string, TileLayer> TileLayersDict { get; private set; }
        public List<TileLayer> TileLayers { get; private set; }
        public Dictionary<string, ObjectGroup> ObjectGroupsDict { get; private set; }

        public TmxData(string fileName)
        {
            FileName = fileName;
        }

        // Function initialises the map data from the given XmlDocument
        public void ImportXmlDoc(System.Xml.XmlDocument xmlDoc, ContentImporterContext context)
        {
            System.Xml.XmlNode mapNode = xmlDoc.GetElementsByTagName("map").Item(0);

            // Parse type
            string typeString = mapNode.Attributes["orientation"].Value;
            switch (typeString)
            {
                case "orthogonal":
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

            // Parse the map properties
            Properties = new PropertySheet();
            System.Xml.XmlNode propertiesNode = mapNode.SelectSingleNode("properties");
            if (propertiesNode != null)
            {
                Properties.ImportXmlNode(propertiesNode, context);
            }

            // Parse the tilesets
            System.Xml.XmlNodeList tilesetNodes = mapNode.SelectNodes("tileset");
            TilesetsDict = new Dictionary<string, Tileset>();
            Tilesets = new List<Tileset>();
            int tileSetIndex = 0;
            foreach (System.Xml.XmlNode tilesetNode in tilesetNodes)
            {
                Tileset tileset = new Tileset(this, tileSetIndex);
                tileset.ImportXmlNode(tilesetNode, context);
                TilesetsDict[tileset.Name] = tileset;
                Tilesets.Add(tileset);
                ++tileSetIndex;
            }

            // Parse the layers
            System.Xml.XmlNodeList layerNodes = mapNode.SelectNodes("layer");
            TileLayersDict = new Dictionary<string, TileLayer>();
            TileLayers = new List<TileLayer>();
            int tileLayerIndex = 0;
            foreach (System.Xml.XmlNode layerNode in layerNodes)
            {
                TileLayer tileLayer = new TileLayer(tileLayerIndex);
                tileLayer.ImportXmlNode(layerNode, context);
                TileLayersDict[tileLayer.Name] = tileLayer;
                TileLayers.Add(tileLayer);
                ++tileLayerIndex;
            }
             
            // Parse the object groups
            System.Xml.XmlNodeList objectgroupNodes = mapNode.SelectNodes("objectgroup");
            ObjectGroupsDict = new Dictionary<string, ObjectGroup>();
            foreach (System.Xml.XmlNode objectGroupNode in objectgroupNodes)
            {
                ObjectGroup objectGroup = new ObjectGroup();
                objectGroup.ImportXmlNode(objectGroupNode, context);
                ObjectGroupsDict[objectGroup.Name] = objectGroup;
            }
        }

    }
}
