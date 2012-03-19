using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TInput = TmxProcessorLib.Data.TmxData;
using TOutput = TmxProcessorLib.Content.Map;
using VFormat = Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture;
using TmxProcessorLib.Content;
using TmxProcessorLib.Data;

namespace TmxProcessorLib
{
    /// <summary>
    /// Processer creates a new Content.Map from the TmxData
    /// </summary>
    [ContentProcessor(DisplayName = "TmxProcessorLib.TmxProcessor")]
    public class TmxProcessor : ContentProcessor<TInput, TOutput>
    {
        public Vector2 Offset = new Vector2(1000.0f, 1000.0f);
        public static Random Random { get; private set; }

        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            TmxProcessorLib.Content.Map output = new TmxProcessorLib.Content.Map();

            Random = new Random();

            output.LayerCount = input.TileLayers.Count;

            ProcessTilesets(context, input, output);
            ProcessCollisionStrips(context, input, output);
            ProcessPlayerSpawns(context, input, output);
            CalculatePlayableArea(context, input, output);
            ProcessSpawnPoints(context, input, output);
            BuildPatches(context, input, output);
            ProcessEvents(context, input, output);
            ProcessSceneObjects(context, input, output);

            return output;
        }

        private void ProcessPlayerSpawns(ContentProcessorContext context, TInput input, TOutput output)
        {
            output.PlayerSpawns = new List<Vector2>();

            foreach (ObjectGroup objGroup in input.ObjectGroups)
            {
                if (objGroup.Name == "Player")
                {
                    foreach (String spawnPointName in objGroup.Objects.Keys)
                    {
                        TmxProcessorLib.Data.Object spawnPoint = objGroup.Objects[spawnPointName];
                        output.PlayerSpawns.Add(spawnPoint.Position + Offset);
                    }
                }
            }
        }

        private void ProcessSpawnPoints(ContentProcessorContext context, TInput input, TOutput output)
        {
            output.SpawnPoints = new List<Vector2>();

            foreach (ObjectGroup objGroup in input.ObjectGroups)
            {
                if (objGroup.Name == "SpawnPoints")
                {
                    foreach (String spawnPointName in objGroup.Objects.Keys)
                    {
                        TmxProcessorLib.Data.Object spawnPoint = objGroup.Objects[spawnPointName];
                        output.SpawnPoints.Add(spawnPoint.Position + Offset);
                    }
                }
            }
        }

        private static void ProcessTilesets(ContentProcessorContext context, TInput input, TOutput output)
        {
            output.Tiles = new Dictionary<uint, Tile>();
            output.Tilesets = new List<Content.Tileset>();

            foreach (Data.Tileset tilesetData in input.Tilesets)
            {
                Content.Tileset tilesetContent = new Content.Tileset();

                tilesetContent.Name = tilesetData.Name;
                
                // build the path using the TileSetDirectory relative to the Content root directory
                string diffusePath = tilesetData.GetFileFullPath(tilesetData.DiffuseName);

                tilesetContent.DiffuseName = diffusePath;

                // build the asset as an external reference
                OpaqueDataDictionary data = new OpaqueDataDictionary();
                data.Add("GenerateMipmaps", false);
                data.Add("ResizeToPowerOfTwo", false);
                data.Add("TextureFormat", TextureProcessorOutputFormat.Color);

                tilesetContent.DiffuseMap = context.BuildAsset<TextureContent, TextureContent>(
                    new ExternalReference<TextureContent>(diffusePath),
                    "TextureProcessor",
                    data,
                    "TextureImporter",
                    tilesetData.GetAssetName(diffusePath));

                // Check for the existance of the other maps
                int pointPos = diffusePath.LastIndexOf('.');

                // figure out how many frames fit on the X axis
                int columns = 1;
                while (columns * tilesetData.TileDimensions.X < tilesetData.Width)
                {
                    columns++;
                }

                // figure out how many frames fit on the Y axis
                int rows = 1;
                while (rows * tilesetData.TileDimensions.Y < tilesetData.Height)
                {
                    rows++;
                }

                // make our tiles. tiles are numbered by row, left to right.
                uint curId = tilesetData.FirstGid;
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {
                        new Microsoft.Xna.Framework.Rectangle();

                        int rx = x * tilesetData.TileDimensions.X;
                        int ry = y * tilesetData.TileDimensions.Y;
                        Microsoft.Xna.Framework.Rectangle rect =
                            new Microsoft.Xna.Framework.Rectangle(rx, ry, tilesetData.TileDimensions.X, tilesetData.TileDimensions.Y);

                        Tile tile = new Tile(curId, tilesetData, rect);

                        output.Tiles[tile.Id] = tile;

                        ++curId;
                    }
                }

                output.Tilesets.Add(tilesetContent);
            }
        }

        private void CalculatePlayableArea(ContentProcessorContext context, TInput input, TOutput output)
        {
            TileLayer wallLayer = input.TileLayers.Find( l => l.Name == "Walls");
            if (wallLayer != null)
            {
                Point min = new Point(int.MaxValue, int.MaxValue);
                Point max = new Point(int.MinValue, int.MinValue);

                for (int x = 0; x < input.Dimensions.X; ++x)
                {
                    for (int y = 0; y < input.Dimensions.Y; ++y)
                    {
                        if (wallLayer.Data[x + (y * wallLayer.Dimensions.X)].Index != 0)
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

                output.MinPlayableArea =
                    new Vector2((float)(min.X * input.TileDimensions.X),
                                (float)(min.Y * input.TileDimensions.Y)) + Offset;
                output.MaxPlayableArea =
                    new Vector2((float)(max.X * input.TileDimensions.X),
                                (float)(max.Y * input.TileDimensions.Y)) + Offset;
            }
        }

        private struct Edge
        {
            public Point Point1 { get { return m_point1; } set { m_point1 = value; } }
            public Point Point2 { get { return m_point2; } set { m_point2 = value; } }

            public Edge(Point point1, Point point2)
            {
                m_point1 = point1;
                m_point2 = point2;
            }

            Point m_point1;
            Point m_point2;
        }

        private void ProcessCollisionStrips(ContentProcessorContext context, TInput input, TOutput output)
        {
            ProcessTileCollisionStrips(context, input, output);
            ProcessSceneObjectsCollisionStrips(context, input, output);
        }

        private void ProcessTileCollisionStrips(ContentProcessorContext context, TInput input, TOutput output)
        {
            TileLayer wallLayer = input.TileLayers.Find(l => l.Name == "Walls");
            if (wallLayer != null)
            {
                List<Edge> edgeList = new List<Edge>();

                // Iterate through each point
                for (int x = 0; x <= input.Dimensions.X; ++x)
                {
                    for (int y = 0; y <= input.Dimensions.Y; ++y)
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
                                    top = (wallLayer.Data[x + ((y - 1) * wallLayer.Dimensions.X)].Index != 0);

                                if (y == wallLayer.Dimensions.Y)
                                    bottom = false;
                                else
                                    bottom = (wallLayer.Data[x + (y * wallLayer.Dimensions.X)].Index != 0);

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
                                    left = (wallLayer.Data[(x - 1) + (y * wallLayer.Dimensions.X)].Index != 0);

                                if (x == wallLayer.Dimensions.X)
                                    right = false;
                                else
                                    right = (wallLayer.Data[x + (y * wallLayer.Dimensions.X)].Index != 0);

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

                output.CollisionBoundaries = new List<List<Vector2>>();

                foreach (List<Edge> currentStrip in edgeLoops)
                {
                    List<Vector2> strip = new List<Vector2>();
                    for (int i = 0; i < currentStrip.Count; ++i)
                    {
                        strip.Add(new Vector2(
                            currentStrip[i].Point1.X * input.TileDimensions.X,
                            currentStrip[i].Point1.Y * input.TileDimensions.Y) + Offset);
                    }

                    // Add the last point
                    strip.Add(new Vector2(
                        currentStrip[currentStrip.Count - 1].Point2.X * input.TileDimensions.X,
                        currentStrip[currentStrip.Count - 1].Point2.Y * input.TileDimensions.Y) + Offset);

                    output.CollisionBoundaries.Add(strip);
                }
            }
        }

        private void ProcessSceneObjectsCollisionStrips(ContentProcessorContext context, TInput input, TOutput output)
        {
            // Add the polygons from the Collision layer
            ObjectGroup sceneObjectsLayer = input.ObjectGroups.Find(l => l.Name == "SceneObjects");
            if (sceneObjectsLayer != null)
            {
                foreach (string objName in sceneObjectsLayer.Objects.Keys)
                {
                    Data.Object obj = sceneObjectsLayer.Objects[objName];
                    if (obj.Polygon != null)
                    {
                        List<Vector2> strip = new List<Vector2>();

                        foreach (Vector2 point in obj.Polygon.Points)
                        {
                            strip.Add(point + obj.Position + Offset);
                        }

                        if (obj.Polygon.Closed)
                        {
                            strip.Add(obj.Polygon.Points[0] + obj.Position + Offset);
                        }

                        output.CollisionBoundaries.Add(strip);
                    }
                }
            }
        }

        private void BuildPatches(ContentProcessorContext context, TInput input, TOutput output)
        {
            int numLayers = input.TileLayers.Count;
            output.Patches = new List<List<Content.Patch>>();

            const int kPatchSize = 4;

            for (int layerIdx = 0; layerIdx < numLayers; ++layerIdx)
            {
                List<Content.Patch> layerPatches = new List<Content.Patch>();
                TileLayer tileLayer = input.TileLayers[layerIdx];

                int patchCountX = tileLayer.Dimensions.X / kPatchSize;
                for (int patchX = 0; patchX < patchCountX; ++patchX)
                {
                    int patchCountY = tileLayer.Dimensions.Y / kPatchSize;
                    for (int patchY = 0; patchY < patchCountY; ++patchY)
                    {
                        Content.Patch patch = BuildPatch(context, input, output, tileLayer, patchX * kPatchSize, patchY * kPatchSize, kPatchSize);
                        if (patch != null)
                        {
                            layerPatches.Add(patch);
                        }
                    }
                }

                output.Patches.Add(layerPatches);
            }
        }

        private void ProcessEvents(ContentProcessorContext context, TInput input, TOutput output)
        {
            /*
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
            */
        }


        private void ProcessSceneObjects(ContentProcessorContext context, TInput input, TOutput output)
        {
            output.SceneObjects = new List<Content.ModelInst>();

            // Add the model instances from the SceneObjects layer
            ObjectGroup sceneObjectsLayer = input.ObjectGroups.Find(l => l.Name == "SceneObjects");
            if (sceneObjectsLayer != null)
            {
                foreach (string objName in sceneObjectsLayer.Objects.Keys)
                {
                    Data.Object obj = sceneObjectsLayer.Objects[objName];

                    if ((obj.Properties.Properties != null) && (obj.Properties.Properties.ContainsKey("model")))
                    {
                        string modelName = "sceneObjects/" + obj.Properties.Properties["model"];

                        Vector3 pos = new Vector3(obj.Position + Offset, 0.0f);

                        float orientation = 0.0f;
                        if (obj.Properties.Properties.ContainsKey("orientation"))
                        {
                            orientation = float.Parse(obj.Properties.Properties["orientation"]);
                            orientation = MathHelper.ToRadians(orientation);
                        }

                        Matrix worldMatrix = Matrix.Identity;
                        worldMatrix *= Matrix.CreateRotationZ(orientation);
                        worldMatrix *= Matrix.CreateTranslation(pos);

                        output.SceneObjects.Add(new ModelInst(modelName, worldMatrix));
                    }
                }
            }
        }

        private struct TileInfo
        {
            public TileInfo(Tile tile, int x, int y, bool flipX, bool flipY)
            {
                mTile = tile;
                mX = x;
                mY = y;
                mFlipX = flipX;
                mFlipY = flipY;
            }

            public Tile mTile;
            public int mX;
            public int mY;
            public bool mFlipX;
            public bool mFlipY;
        };

        private Content.Patch BuildPatch(ContentProcessorContext context, TInput input, TOutput output, TileLayer tileLayer, int xMin, int yMin, int patchSize)
        {
            // Create a dictionary of all neccessary tile info
            // indexed by the texture
            Dictionary<Data.Tileset, List<TileInfo>> tileDict = new Dictionary<Data.Tileset, List<TileInfo>>();
            for (int x = xMin; x < (xMin + patchSize); ++x)
            {
                System.Diagnostics.Debug.Assert(x < input.Dimensions.X);

                for (int y = yMin; y < (yMin + patchSize); ++y)
                {
                    System.Diagnostics.Debug.Assert(y < input.Dimensions.Y);

                    TileLayer.TileEntry tileEntry = tileLayer.Data[x + (y * tileLayer.Dimensions.X)];
                    uint tileId = tileEntry.Index;
                    if (tileId == 0)
                        continue;

                    Tile tile = output.Tiles[tileId];

                    if (!tileDict.ContainsKey(tile.Parent))
                    {
                        tileDict.Add(tile.Parent, new List<TileInfo>());
                    }
                    tileDict[tile.Parent].Add(new TileInfo(tile, x, y, tileEntry.FlipX, tileEntry.FlipY));
                }
            }

            if (tileDict.Count > 0)
            {
                Content.Patch patch = new Content.Patch();

                // Now iterate the dictionary, building a mesh for each texture
                foreach (Data.Tileset tileset in tileDict.Keys)
                {
                    List<TileInfo> tileList = tileDict[tileset];

                    // Use models for the wall layer
                    if (tileset.Name == "citywalls")
                    {
                        string modelName = "tiles/cityBlockWall";


                        for (int i = 0; i < tileList.Count; ++i)
                        {
                            TileInfo tileInfo = tileList[i];

                            float centerX = (tileInfo.mX * input.TileDimensions.X) + (input.TileDimensions.X * 0.5f) + Offset.X;
                            float centerY = (tileInfo.mY * input.TileDimensions.Y) + (input.TileDimensions.Y * 0.5f) + Offset.X;

                            float randScaleY = 1.0f + (float)TmxProcessor.Random.NextDouble();

                            int rotCount = TmxProcessor.Random.Next(3);
                            float randScaleZ = ((float)Math.PI * 0.5f) * rotCount;


                            const float kGlobalScale = 0.35f;

                            Vector3 pos = new Vector3(centerX, centerY, 0.0f);

                            Matrix worldMatrix = Matrix.Identity;

                            worldMatrix *= Matrix.CreateScale(kGlobalScale, kGlobalScale * randScaleY, kGlobalScale);
                            worldMatrix *= Matrix.CreateRotationY(randScaleZ);
                            worldMatrix *= Matrix.CreateRotationX((float)Math.PI * 0.5f);
                            worldMatrix *= Matrix.CreateTranslation(pos);

                            patch.Models.Add(new Content.ModelInst(modelName, worldMatrix));
                        }
                    }


                    // Add the ground info
                    int numVerts = tileList.Count * 6;
                    VFormat[] vertList = new VFormat[numVerts];
                    int currentVert = 0;

                    for (int i = 0; i < tileList.Count; ++i)
                    {
                        TileInfo tileInfo = tileList[i];

                        float left = (tileInfo.mX * input.TileDimensions.X) + Offset.X;
                        float right = ((tileInfo.mX + 1) * input.TileDimensions.X) + Offset.X;
                        float top = (tileInfo.mY * input.TileDimensions.Y) + Offset.Y;
                        float bottom = ((tileInfo.mY + 1) * input.TileDimensions.Y) + Offset.Y;

                        const float kEpsilon = 0.0f; // used to prevent colour bleeding
                        float texLeft = (((float)tileInfo.mTile.Source.Left + kEpsilon) / (float)tileset.Width);
                        float texRight = (((float)tileInfo.mTile.Source.Right - kEpsilon) / (float)tileset.Width);
                        float texTop = (((float)tileInfo.mTile.Source.Top + kEpsilon) / (float)tileset.Height);
                        float texBottom = (((float)tileInfo.mTile.Source.Bottom - kEpsilon) / (float)tileset.Height);

                        if (tileInfo.mFlipX)
                        {
                            float temp = texLeft;
                            texLeft = texRight;
                            texRight = temp;
                        }

                        if (tileInfo.mFlipY)
                        {
                            float temp = texTop;
                            texTop = texBottom;
                            texBottom = temp;
                        }

                        Vector3 up = new Vector3(0.0f, 0.0f, 1.0f);

                        vertList[currentVert].Position = new Vector3(left, top, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texLeft, texTop);
                        vertList[currentVert].Normal = up;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(right, top, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texRight, texTop);
                        vertList[currentVert].Normal = up;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(left, bottom, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texLeft, texBottom);
                        vertList[currentVert].Normal = up;
                        ++currentVert;


                        vertList[currentVert].Position = new Vector3(left, bottom, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texLeft, texBottom);
                        vertList[currentVert].Normal = up;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(right, top, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texRight, texTop);
                        vertList[currentVert].Normal = up;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(right, bottom, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texRight, texBottom);
                        vertList[currentVert].Normal = up;
                        ++currentVert;
                    }

                    patch.Meshes.Add(new Content.Mesh(input.Tilesets.IndexOf(tileset), vertList));
                }

                return patch;
            }
            else
            {
                return null;
            }
        }


        /*
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
        */

        /*
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
        */
    }
}