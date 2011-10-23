﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.IO;
using System.Xml;


namespace TmxProcessorLib.Content
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

        public Dictionary<uint, Tile> Tiles { get; private set; }

        public List<Point[]> CollisionBoundaries { get; private set; }

        public List<Vector2> PlayerSpawns { get; private set; }

        public Vector2 MinPlayableArea { get; private set; }
        public Vector2 MaxPlayableArea { get; private set; }

        public List<Vector2> SpawnPoints { get; private set; }

        public List<Object> PressurePlates { get; private set; }

        internal List<Patch>[] Patches { get; private set; }

        internal List<TimerEvent> TimerEvents { get; private set; }
        internal List<SpawnEvent> SpawnEvents { get; private set; }
        internal List<TriggerCounterEvent> TriggerCounterEvents { get; private set; }

        public TmxData(string fileName)
        {
            FileName = fileName;
        }

        public Vector2 Offset = new Vector2(1000.0f, 1000.0f);

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
                Tileset tileset = new Tileset(tileSetIndex);
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

            // Add dependancy to events xml if defined
            TimerEvents = new List<TimerEvent>();
            SpawnEvents = new List<SpawnEvent>();
            TriggerCounterEvents = new List<TriggerCounterEvent>();
            if (Properties.Properties.ContainsKey("events"))
            {
                string eventsPath = Path.Combine(FileName.Remove(FileName.LastIndexOf('\\')), Properties.Properties["events"]);
                context.AddDependency(eventsPath);

                Stack<EventGroup> nameStack = new Stack<EventGroup>();

                if (File.Exists(eventsPath))
                {
                    XmlDocument eventsXml = new XmlDocument();
                    eventsXml.Load(eventsPath);

                    XmlNodeList nodes = eventsXml.SelectNodes("Events/*");
                    foreach (XmlNode node in nodes)
                    {
                        string name = node.Name;
                        Console.Out.WriteLine(name);

                        CheckEventNode(node, nameStack, context);
                    }
                }
            }
        }
        void CheckEventNode(XmlNode node, Stack<EventGroup> groupStack, ContentImporterContext context)
        {
            switch (node.Name)
            {
                case "EventGroup":
                    {
                        EventGroup eventGroup = new EventGroup();
                        eventGroup.ImportXmlNode(node, context);
                        groupStack.Push(eventGroup);

                        XmlNodeList childNodes = node.ChildNodes;
                        foreach (XmlNode childNode in childNodes)
                        {
                            string name = node.Name;
                            Console.Out.WriteLine(name);

                            // Recursively check this groups children
                            CheckEventNode(childNode, groupStack, context);
                        }

                        groupStack.Pop();
                    }
                    break;

                case "Timer":
                    {
                        TimerEvent timerEvent = new TimerEvent();
                        timerEvent.ImportXmlNode(node, groupStack, context);
                        TimerEvents.Add(timerEvent);
                    }
                    break;

                case "Spawn":
                    {
                        SpawnEvent spawnEvent = new SpawnEvent();
                        spawnEvent.ImportXmlNode(node, groupStack, context);
                        SpawnEvents.Add(spawnEvent);
                    }
                    break;

                case "TriggerCounter":
                    {
                        TriggerCounterEvent triggerCounterEvent = new TriggerCounterEvent();
                        triggerCounterEvent.ImportXmlNode(node, groupStack, context);
                        TriggerCounterEvents.Add(triggerCounterEvent);
                    }
                    break;

                case "Repeat":
                    {
                        int count = int.Parse(node.Attributes["count"].Value);
                        for (int i = 0; i < count; ++i)
                        {
                            XmlNodeList childNodes = node.ChildNodes;
                            foreach (XmlNode childNode in childNodes)
                            {
                                // Recursively check this groups children
                                CheckEventNode(childNode, groupStack, context);
                            }
                        }
                    }
                    break;
            }
        }

        public void Process(ContentProcessorContext context)
        {
            Tiles = new Dictionary<uint, Tile>();

            foreach (string tilesetName in TilesetsDict.Keys)
            {
                TilesetsDict[tilesetName].Process(this, context);
            }

            foreach (string tileLayerName in TileLayersDict.Keys)
            {
                TileLayersDict[tileLayerName].Process(this, context);
            }

            BuildCollisionStrip();

            BuildPlayerSpawns();

            CalculatePlayableArea();

            BuildSpawnPoints();

            BuildPatches();

            BuildPressurePointList();
        }

        private void BuildPressurePointList()
        {
            PressurePlates = new List<Object>();

            if (ObjectGroupsDict.ContainsKey("PressurePlates"))
            {
                ObjectGroup pressurePlatesGroup = ObjectGroupsDict["PressurePlates"];
                Dictionary<String, Object> pressurePlatesDict = pressurePlatesGroup.Objects;
                foreach (String pressurePlatesName in pressurePlatesDict.Keys)
                {
                    Object pressurePlateObject = pressurePlatesDict[pressurePlatesName];
                    switch (pressurePlateObject.Type)
                    {
                        case "Interactive":
                        case "Normal":
                        case "":
                            PressurePlates.Add(pressurePlateObject);
                            break;

                        default:
                            throw new InvalidContentException("Invalid pressure plate type!");
                    }
                }
            }
        }

        private void BuildSpawnPoints()
        {
            SpawnPoints = new List<Vector2>();

            foreach (String objGroupName in ObjectGroupsDict.Keys)
            {
                if (objGroupName == "SpawnPoints")
                {
                    ObjectGroup spawnGroup = ObjectGroupsDict[objGroupName];
                    foreach (String spawnPointName in spawnGroup.Objects.Keys)
                    {
                        Object spawnPoint = spawnGroup.Objects[spawnPointName];
                        SpawnPoints.Add(spawnPoint.Position);
                    }
                }
            }
        }

        private void CalculatePlayableArea()
        {
            if (TileLayersDict.ContainsKey("Walls"))
            {
                TileLayer wallLayer = TileLayersDict["Walls"];

                Point min = new Point(int.MaxValue, int.MaxValue);
                Point max = new Point(int.MinValue, int.MinValue);

                for (int x = 0; x < Dimensions.X; ++x)
                {
                    for (int y = 0; y < Dimensions.Y; ++y)
                    {
                        if (wallLayer.Data[x + (y * wallLayer.Dimensions.X)] != 0)
                        {
                            if (x < min.X)
                                min.X = x;
                            if (y < min.Y)
                                min.Y = y;

                            if (x > max.X)
                                max.X = x;
                            if (y > max.Y)
                                max.Y = y;
                        }
                    }
                }

                // Adjust to bring inside the wall
                min.X += 1;
                min.Y += 1;
                max.X -= 1;
                max.Y -= 1;

                MinPlayableArea = new Vector2((float)(min.X * TileDimensions.X), (float)(min.Y * TileDimensions.Y));
                MaxPlayableArea = new Vector2((float)(max.X * TileDimensions.X), (float)(max.Y * TileDimensions.Y));
            }
        }

        private void BuildPlayerSpawns()
        {
            if (ObjectGroupsDict.ContainsKey("Player"))
            {
                ObjectGroup playerGroup = ObjectGroupsDict["Player"];

                PlayerSpawns = new List<Vector2>();
                foreach(String objName in playerGroup.Objects.Keys)
                {
                    Object obj = playerGroup.Objects[objName];
                    if (obj.Type == "PlayerSpawn")
                    {
                        PlayerSpawns.Add(obj.Position);
                    }
                }
            }
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
            if (TileLayersDict.ContainsKey("Walls"))
            {
                TileLayer wallLayer = TileLayersDict["Walls"];
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
                                    top = false;
                                else
                                    top = (wallLayer.Data[x + ((y - 1) * wallLayer.Dimensions.X)] != 0);

                                if (y == wallLayer.Dimensions.Y)
                                    bottom = false;
                                else
                                    bottom = (wallLayer.Data[x + (y * wallLayer.Dimensions.X)] != 0);

                                // If same, discard
                                if (top != bottom)
                                {
                                    Point leftPoint = new Point(x, y);
                                    Point rightPoint = new Point(x + 1, y);

                                    if (top)
                                    {
                                        edgeList.Add(new Edge(rightPoint, leftPoint));
                                    }
                                    else
                                    {
                                        edgeList.Add(new Edge(leftPoint, rightPoint));
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
                                    left = false;
                                else
                                    left = (wallLayer.Data[(x - 1) + (y * wallLayer.Dimensions.X)] != 0);

                                if (x == wallLayer.Dimensions.X)
                                    right = false;
                                else
                                    right = (wallLayer.Data[x + (y * wallLayer.Dimensions.X)] != 0);

                                // If same, discard
                                if (left != right)
                                {
                                    Point topPoint = new Point(x, y);
                                    Point bottomPoint = new Point(x, y + 1);
                                    if (left)
                                    {
                                        edgeList.Add(new Edge(topPoint, bottomPoint));
                                    }
                                    else
                                    {
                                        edgeList.Add(new Edge(bottomPoint, topPoint));
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

        private void BuildPatches()
        {
            int numLayers = TileLayers.Count;
            Patches = new List<Patch>[numLayers];

            const int kPatchSize = 8;

            for (int layerIdx = 0; layerIdx < numLayers; ++layerIdx)
            {
                Patches[layerIdx] = new List<Patch>();
                TileLayer tileLayer = TileLayers[layerIdx];

                int patchCountX = tileLayer.Dimensions.X / kPatchSize;
                for (int patchX = 0; patchX < patchCountX; ++patchX)
                {
                    int patchCountY = tileLayer.Dimensions.Y / kPatchSize;
                    for (int patchY = 0; patchY < patchCountY; ++patchY)
                    {
                        Patch patch = Patch.Build(this, tileLayer, patchX * kPatchSize, patchY * kPatchSize, kPatchSize);
                        if (patch != null)
                        {
                            Patches[layerIdx].Add(patch);
                        }
                    }
                }
            }
        }


        public void Write(ContentWriter output)
        {
            output.Write(TileLayers.Count);

            output.Write(TilesetsDict.Count);
            foreach (string tilesetName in TilesetsDict.Keys)
            {
                TilesetsDict[tilesetName].Write(output);
            }

            // Output the collision boundaries
            if (CollisionBoundaries != null)
            {
                output.Write(CollisionBoundaries.Count);
                foreach (Point[] boundary in CollisionBoundaries)
                {
                    output.Write(boundary.Length);

                    for (int i = 0; i < boundary.Length; ++i)
                    {
                        Vector2 fBoundary = new Vector2(boundary[i].X + Offset.X, boundary[i].Y + Offset.Y);
                        output.WriteObject<Vector2>(fBoundary);
                    }
                }
            }
            else
            {
                output.Write(0);
            }

            // Output the player spawns
            output.Write(PlayerSpawns.Count);
            foreach (Vector2 pos in PlayerSpawns)
            {
                output.Write(pos + Offset);
            }

            // Output the play area definition
            output.Write(MinPlayableArea + Offset);
            output.Write(MaxPlayableArea + Offset);

            // Output each spawn point
            output.Write(SpawnPoints.Count);
            foreach (Vector2 spawnPoint in SpawnPoints)
            {
                output.Write(spawnPoint + Offset);
            }

            // Output the patch objects
            for (int layerNum = 0; layerNum < TileLayers.Count; ++layerNum)
            {
                output.Write(Patches[layerNum].Count);
                foreach (Patch patch in Patches[layerNum])
                {
                    patch.Write(output, Offset);
                }
            }

            // Output the pressure plates
            output.Write(PressurePlates.Count);
            for (int plateNum = 0; plateNum < PressurePlates.Count; ++plateNum)
            {
                Object plate = PressurePlates[plateNum];
                int type;
                if (plate.Type == "")
                {
                    type = (int)(MapData.PressurePlateData.Type.Normal);
                }
                else
                {
                    type = (int)(Enum.Parse(typeof(MapData.PressurePlateData.Type), plate.Type, true));
                }

                // Output defaults
                output.Write(type);
                output.Write(plate.Name);
                output.Write(plate.Position + Offset);

                // Use the largest dimension as the diameter of the circle
                float radius = Math.Max(plate.Dimensions.X, plate.Dimensions.Y) * 0.5f;
                output.Write(radius);

                if(!plate.Properties.Properties.ContainsKey("trigger"))
                {
                    throw new InvalidContentException("Pressure plate missing 'trigger' property!");
                }
                output.Write(plate.Properties.Properties["trigger"]);

                switch (plate.Type)
                {
                    case "Interactive":
                        if(!plate.Properties.Properties.ContainsKey("interactTime"))
                        {
                            throw new InvalidContentException("Interactive pressure plate missing 'interactTime' property!");
                        }
                        output.Write(float.Parse(plate.Properties.Properties["interactTime"]));
                        break;

                    case "Normal":
                    case null:
                        // Do nothing
                        break;

                    default:
                        throw new InvalidContentException("Invalid pressure plate type!");
                }
            }

            // Output the events
            // Timers
            output.Write(TimerEvents.Count);
            foreach( TimerEvent timerEvent in TimerEvents)
            {
                timerEvent.Write(output);
            }

            // Spawns
            output.Write(SpawnEvents.Count);
            foreach (SpawnEvent spawnEvent in SpawnEvents)
            {
                spawnEvent.Write(output);
            }

            // Trigger Counters
            output.Write(TriggerCounterEvents.Count);
            foreach (TriggerCounterEvent triggerCounterEvent in TriggerCounterEvents)
            {
                triggerCounterEvent.Write(output);
            }
        }

        internal Tile GetTile(uint tileIdx)
        {
            if (tileIdx == 0)
                return null;
            else
                return Tiles[tileIdx];
        }
    }
}
