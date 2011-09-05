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


        public void Read(ContentReader input)
        {
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
        }
    }




}
