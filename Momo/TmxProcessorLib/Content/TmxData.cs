using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Xml;
using Microsoft.Xna.Framework;

namespace TmxProcessorLib.Content
{
    public class TmxData
    {
        public enum TypeId
        {
            Orthogonal = 0,
            Isometric = 1
        }

        public TypeId Type { get; private set; }
        public Point Dimensions { get; private set; }
        public Point TileDimensions { get; private set; }

        public PropertySheet Properties { get; private set; }

        public String FileName { get; private set; }

        public Dictionary<string, Tileset> Tilesets { get; private set; }
        public Dictionary<string, TileLayer> TileLayers { get; private set; }
        public Dictionary<string, ObjectGroup> ObjectGroups { get; private set; }

        public List<Point[]> CollisionBoundaries { get; private set; }

        public TmxData(string fileName)
        {
            FileName = fileName;
        }

        // Function initialises the map data from the given XmlDocument
        public void ImportXmlDoc(System.Xml.XmlDocument xmlDoc, ContentImporterContext context)
        {
            string LightingXmlPath = Path.Combine(FileName.Remove(FileName.LastIndexOf('\\')), "lighting.xml");
            context.AddDependency(LightingXmlPath);

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
            Tilesets = new Dictionary<string, Tileset>();
            foreach (System.Xml.XmlNode tilesetNode in tilesetNodes)
            {
                Tileset tileset = new Tileset();
                tileset.ImportXmlNode(tilesetNode, context);
                Tilesets[tileset.Name] = tileset;
            }

            // Parse the layers
            System.Xml.XmlNodeList layerNodes = mapNode.SelectNodes("layer");
            TileLayers = new Dictionary<string, TileLayer>();
            foreach (System.Xml.XmlNode layerNode in layerNodes)
            {
                TileLayer tileLayer = new TileLayer();
                tileLayer.ImportXmlNode(layerNode, context);
                TileLayers[tileLayer.Name] = tileLayer;
            }
             
            // Parse the object groups
            System.Xml.XmlNodeList objectgroupNodes = mapNode.SelectNodes("objectgroup");
            context.Logger.LogMessage("OBJECT GROUP NODES: {0} {1}", objectgroupNodes, objectgroupNodes.Count);
            ObjectGroups = new Dictionary<string, ObjectGroup>();
            foreach (System.Xml.XmlNode objectGroupNode in objectgroupNodes)
            {
                ObjectGroup objectGroup = new ObjectGroup();
                objectGroup.ImportXmlNode(objectGroupNode, context);
                ObjectGroups[objectGroup.Name] = objectGroup;
            }
        }

        public void Process(ContentProcessorContext context)
        {
            foreach (string tilesetName in Tilesets.Keys)
            {
                Tilesets[tilesetName].Process(this, context);
            }

            foreach (string tileLayerName in TileLayers.Keys)
            {
                TileLayers[tileLayerName].Process(this, context);
            }

            BuildCollisionStrip();
        }


        private struct Edge
        {
            public Point Point1 { get { return m_point1; } set{ m_point1 = value;} }
            public Point Point2 { get { return m_point2; } set { m_point2 = value; } }

            public Edge(Point point1, Point point2)
            {
                m_point1 = point1;
                m_point2 = point2;
            }

            Point m_point1;
            Point m_point2;
        }

        private void BuildCollisionStrip()
        {
            if (TileLayers.ContainsKey("Walls"))
            {
                TileLayer wallLayer = TileLayers["Walls"];
                List<Edge> edgeList = new List<Edge>();

                // Iterate through each point
                for (int x = 0; x <= Dimensions.X; ++x)
                {
                    for (int y = 0; y <= Dimensions.Y; ++y)
                    {
                        // Handle Horizontal
                        {
                            // Can't handle the last column
                            if (x < wallLayer.Dimensions.X)
                            {
                                bool top, bottom;

                                // Compare sides (treat edges as collision)
                                if (y == 0)
                                    top = true;
                                else
                                    top = (wallLayer.Data[x + ((y - 1) * wallLayer.Dimensions.X)] != 0);

                                if (y == wallLayer.Dimensions.Y)
                                    bottom = true;
                                else
                                    bottom = (wallLayer.Data[x + (y * wallLayer.Dimensions.X)] != 0);

                                // If same, discard
                                if (top != bottom)
                                {
                                    Point leftPoint = new Point(x, y);
                                    Point rightPoint = new Point(x + 1, y);

                                    if (top)
                                    {
                                        edgeList.Add(new Edge(leftPoint, rightPoint));
                                    }
                                    else
                                    {
                                        edgeList.Add(new Edge(rightPoint, leftPoint));
                                    }
                                }
                            }
                        }

                        // Handle Vertical
                        {
                            // Can't handle the last row
                            if (y < wallLayer.Dimensions.Y)
                            {
                                bool left, right;

                                // Compare sides (treat edges as collision)
                                if (x == 0)
                                    left = true;
                                else
                                    left = (wallLayer.Data[(x - 1) + (y * wallLayer.Dimensions.X)] != 0);

                                if (x == wallLayer.Dimensions.X)
                                    right = true;
                                else
                                    right = (wallLayer.Data[x + (y * wallLayer.Dimensions.X)] != 0);

                                // If same, discard
                                if (left != right)
                                {
                                    Point topPoint = new Point(x, y);
                                    Point bottomPoint = new Point(x, y + 1);
                                    if (left)
                                    {
                                        edgeList.Add(new Edge(bottomPoint, topPoint));
                                    }
                                    else
                                    {
                                        edgeList.Add(new Edge(topPoint, bottomPoint));
                                    }
                                }
                            }
                        }
                    }
                }

                // Now connect the edges
                List<List<Edge>> edgeLoops = new List<List<Edge>>();
                {
                    List<Edge> currentLoop = new List<Edge>();
                    {
                        Edge currentEdge = edgeList[0];
                        currentLoop.Add(currentEdge);
                        edgeList.Remove(currentEdge);

                        do
                        {
                            bool found = false;
                            for (int i = 0; i < edgeList.Count; ++i)
                            {
                                if (edgeList[i].Point1 == currentEdge.Point2)
                                {
                                    currentEdge = edgeList[i];
                                    found = true;
                                    break;
                                }
                            }

                            // If there's no edge matching we must've completed a loop
                            if (!found)
                            {
                                // Add the first node again to complete the loop
                                edgeLoops.Add(currentLoop);
                                currentLoop = new List<Edge>();

                                // If there's nothing left on the list then we're done
                                if (edgeList.Count == 0)
                                {
                                    break;
                                }

                                // Use the first edge
                                currentEdge = edgeList[0];
                            }

                            currentLoop.Add(currentEdge);
                            edgeList.Remove(currentEdge);
                        }
                        while (true); // Exited from within loop
                    }
                }



                // Now optimise the list
                CollisionBoundaries = new List<Point[]>();
                for (int stripIdx = 0; stripIdx < edgeLoops.Count; ++stripIdx)
                {
                    List<Edge> currentStrip = edgeLoops[stripIdx];
                    List<Edge> optimised = new List<Edge>();

                    int currentIndex = 0;
                    Edge newEdge = currentStrip[currentIndex];
                    Point delta = new Point(
                        currentStrip[currentIndex].Point2.X - currentStrip[currentIndex].Point1.X,
                        currentStrip[currentIndex].Point2.Y - currentStrip[currentIndex].Point1.Y);

                    while (currentIndex < currentStrip.Count)
                    {
                        Edge currentEdge;
                        Point currentDelta;
                        do
                        {
                            currentIndex++;

                            if (currentIndex == currentStrip.Count)
                            {
                                optimised.Add(newEdge);
                                break;
                            }

                            currentEdge = currentStrip[currentIndex];
                            currentDelta = new Point(
                                currentEdge.Point2.X - currentEdge.Point1.X,
                                currentEdge.Point2.Y - currentEdge.Point1.Y);

                            if (delta == currentDelta)
                            {
                                newEdge.Point2 = currentEdge.Point2;
                            }
                            else
                            {
                                delta = currentDelta;
                                optimised.Add(newEdge);
                                newEdge = currentEdge;
                            }
                        }
                        while (true);
                    }

                    edgeLoops[stripIdx] = optimised;
                }

                CollisionBoundaries = new List<Point[]>();
                foreach (List<Edge> currentStrip in edgeLoops)
                {
                    Point[] strip = new Point[currentStrip.Count + 1];
                    for (int i = 0; i < currentStrip.Count; ++i)
                    {
                        strip[i] = new Point(
                            currentStrip[i].Point1.X * TileDimensions.X,
                            currentStrip[i].Point1.Y * TileDimensions.Y);
                    }
                    // Add the last point
                    strip[currentStrip.Count] = new Point(
                        currentStrip[currentStrip.Count - 1].Point2.X * TileDimensions.X,
                        currentStrip[currentStrip.Count - 1].Point2.Y * TileDimensions.Y);

                    CollisionBoundaries.Add(strip);
                }
            }
        }

        public void Write(ContentWriter output)
        {
            output.Write((byte)Type);
            output.WriteObject<Point>(Dimensions);
            output.WriteObject<Point>(TileDimensions);

            output.Write(Tilesets.Count);
            foreach (string tilesetName in Tilesets.Keys)
            {
                Tilesets[tilesetName].Write(output);
            }

            output.Write(TileLayers.Count);
            foreach (string tileLayerName in TileLayers.Keys)
            {
                TileLayers[tileLayerName].Write(output);
            }

            // Output the collision boundaries
            if(CollisionBoundaries != null)
            {
                output.Write(CollisionBoundaries.Count);
                foreach(Point[] boundary in CollisionBoundaries)
                {
                    output.Write(boundary.Length);

                    for (int i = 0; i < boundary.Length; ++i)
                    {
                        output.WriteObject<Point>(boundary[i]);
                    }
                }
            }
            else
            {
                output.Write(0);
            }

        }
    }
}
