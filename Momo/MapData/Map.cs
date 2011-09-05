using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace MapData
{
    using VFormat = VertexPositionTexture;

    public class Map
    {
        public enum TypeId
        {
            Orthogonal = 0,
            Isometric = 1
        }

        public TypeId Type { get; private set; }
        public Point Dimensions { get; private set; }
        public Point TileDimensions { get; private set; }

        public Tileset[] Tilesets { get; private set; }
        public TileLayer[] TileLayers { get; private set; }

        public Tile[] Tiles { get; private set; }

        public Vector2[][] CollisionBoundaries { get; set; }

        public Vector2[] PlayerSpawnPoints { get; private set; }

        public Vector2 PlayAreaMin { get; private set; }
        public Vector2 PlayAreaMax { get; private set; }

        public Wave[] Waves { get; private set; }

        int m_patchSize = 8;
        public List<MapData.Patch>[] Patches;

        public void Read(ContentReader input)
        {
            // TODO: Read this from pipeline
            //m_patchSize = patchSize;

            Type = (TypeId)input.ReadByte();
            Dimensions = input.ReadObject<Point>();
            TileDimensions = input.ReadObject<Point>();

            int tilesetCount = input.ReadInt32();
            Tilesets = new Tileset[tilesetCount];
            for (int i = 0; i < tilesetCount; i++)
            {
                Tilesets[i] = new Tileset();
                Tilesets[i].Read(this, input);
            }

            int tileLayerCount = input.ReadInt32();
            TileLayers = new TileLayer[tileLayerCount];
            for (int i = 0; i < tileLayerCount; i++)
            {
                TileLayers[i] = new TileLayer();
                TileLayers[i].Read(this, input);
            }

            // Determine the size of the tile array
            uint maxId = 0;
            for (int i = 0; i < tilesetCount; i++)
            {
                uint endId = Tilesets[i].FirstGid + (uint)Tilesets[i].Tiles.Length;
                if (endId > maxId)
                    maxId = endId;
            }

            // Now build the indexed list of tiles
            Tiles = new Tile[maxId];
            for (int i = 0; i < tilesetCount; i++)
            {
                for (int j = 0; j < Tilesets[i].Tiles.Length; j++)
                {
                    Tiles[Tilesets[i].FirstGid + j] = new Tile(Tilesets[i].DiffuseMap, Tilesets[i].Tiles[j]);
                }
            }

            // Read in the collision data
            int numStrips = input.ReadInt32();
            CollisionBoundaries = new Vector2[numStrips][];
            for (int boundaryIdx = 0; boundaryIdx < numStrips; ++boundaryIdx)
            {
                int numNodes = input.ReadInt32();
                CollisionBoundaries[boundaryIdx] = new Vector2[numNodes];
                for (int nodeIdx = 0; nodeIdx < numNodes; ++nodeIdx)
                {
                    CollisionBoundaries[boundaryIdx][nodeIdx] = input.ReadObject<Vector2>();
                }
            }

            // Read in the player spawn objects
            int numSpawnPoints = input.ReadInt32();
            PlayerSpawnPoints = new Vector2[numSpawnPoints];
            for (int i = 0; i < numSpawnPoints; ++i)
            {
                PlayerSpawnPoints[i] = input.ReadVector2();
            }

            // Read in the vectors describing the playable area
            PlayAreaMin = input.ReadVector2();
            PlayAreaMax = input.ReadVector2();

            // Read the wave information
            int numWaves = input.ReadInt32();
            Waves = new Wave[numWaves];

            for (int waveIdx = 0; waveIdx < numWaves; ++waveIdx)
            {
                int numEnemies = input.ReadInt32();
                Enemy[] enemies = new Enemy[numEnemies];
                for(int enemyIdx = 0; enemyIdx < numEnemies; ++enemyIdx)
                {
                    string name = input.ReadString();
                    Vector2 pos = input.ReadObject<Vector2>();

                    enemies[enemyIdx] = new Enemy(name, pos);
                }

                Waves[waveIdx] = new Wave(enemies);
            }


            // Build the patches
            // TODO: Move this to pipeline
            // Each layer consists of a list of patches
            int numLayers = TileLayers.Length;
            Patches = new List<MapData.Patch>[numLayers];
            for (int layerIdx = 0; layerIdx < numLayers; ++layerIdx)
            {
                Patches[layerIdx] = new List<MapData.Patch>();
                MapData.TileLayer tileLayer = TileLayers[layerIdx];

                int patchCountX = tileLayer.Dimensions.X / m_patchSize;
                for (int patchX = 0; patchX < patchCountX; ++patchX)
                {
                    int patchCountY = tileLayer.Dimensions.Y / m_patchSize;
                    for (int patchY = 0; patchY < patchCountX; ++patchY)
                    {
                        MapData.Patch patch = BuildPatch(tileLayer, patchX * m_patchSize, patchY * m_patchSize);
                        if (patch != null)
                        {
                            Patches[layerIdx].Add(patch);
                        }
                    }
                }
            }
        }


        private struct TileInfo
        {
            public TileInfo(MapData.Tile tile, int x, int y)
            {
                m_tile = tile;
                m_x = x;
                m_y = y;
            }

            public MapData.Tile m_tile;
            public int m_x;
            public int m_y;
        };

        private MapData.Patch BuildPatch(MapData.TileLayer tileLayer, int xMin, int yMin)
        {
            // Create a dictionary of all neccessary tile info
            // indexed by the texture
            Dictionary<Texture2D, List<TileInfo>> tileDict = new Dictionary<Texture2D, List<TileInfo>>();
            for (int x = xMin; x < (xMin + m_patchSize); ++x)
            {
                System.Diagnostics.Debug.Assert(x < Dimensions.X);

                for (int y = yMin; y < (yMin + m_patchSize); ++y)
                {
                    System.Diagnostics.Debug.Assert(x < Dimensions.Y);

                    MapData.Tile tile = Tiles[tileLayer.Indices[x + (y * tileLayer.Dimensions.X)]];

                    if (tile == null)
                        continue;

                    if (!tileDict.ContainsKey(tile.DiffuseMap))
                    {
                        tileDict.Add(tile.DiffuseMap, new List<TileInfo>());
                    }
                    tileDict[tile.DiffuseMap].Add(new TileInfo(tile, x, y));
                }
            }

            if (tileDict.Count > 0)
            {
                MapData.Patch patch = new MapData.Patch();

                // Now iterate the dictionary, building a mesh for each texture
                foreach (Texture2D texture in tileDict.Keys)
                {
                    List<TileInfo> tileList = tileDict[texture];

                    int numVerts = tileList.Count * 6;
                    VFormat[] vertList = new VFormat[numVerts];
                    int currentVert = 0;

                    for (int i = 0; i < tileList.Count; ++i)
                    {
                        TileInfo tileInfo = tileList[i];

                        float left = (tileInfo.m_x * TileDimensions.X);
                        float right = ((tileInfo.m_x + 1) * TileDimensions.X);
                        float top = (tileInfo.m_y * TileDimensions.Y);
                        float bottom = ((tileInfo.m_y + 1) * TileDimensions.Y);

                        const float kEpsilon = 0.0f; // used to prevent colour bleeding
                        float texLeft = (((float)tileInfo.m_tile.Source.Left + kEpsilon) / (float)texture.Width);
                        float texRight = (((float)tileInfo.m_tile.Source.Right - kEpsilon) / (float)texture.Width);
                        float texTop = (((float)tileInfo.m_tile.Source.Top + kEpsilon) / (float)texture.Height);
                        float texBottom = (((float)tileInfo.m_tile.Source.Bottom - kEpsilon) / (float)texture.Height);

                        vertList[currentVert].Position = new Vector3(left, top, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texLeft, texTop);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(right, top, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texRight, texTop);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(left, bottom, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texLeft, texBottom);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;


                        vertList[currentVert].Position = new Vector3(left, bottom, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texLeft, texBottom);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(right, top, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texRight, texTop);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;

                        vertList[currentVert].Position = new Vector3(right, bottom, 0.0f);
                        vertList[currentVert].TextureCoordinate = new Vector2(texRight, texBottom);
                        //vertList[currentVert].Color = Color.White;
                        ++currentVert;
                    }

                    patch.AddMesh(texture, vertList);
                }

                return patch;
            }
            else
            {
                return null;
            }
        }
    }




}
