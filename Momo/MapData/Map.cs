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
        public int LayerCount { get; private set; }

        public Tileset[] Tilesets { get; private set; }

        public Vector2[][] CollisionBoundaries { get; set; }

        public Vector2[] PlayerSpawnPoints { get; private set; }

        public Vector2 PlayAreaMin { get; private set; }
        public Vector2 PlayAreaMax { get; private set; }

        public Wave[] Waves { get; private set; }

        public MapData.Patch[][] Patches { get; private set; }

        public Trigger[] Triggers { get; private set; }

        public enum TriggerType
        {
            Invalid = -1,

            KillCount = 0
        }

        public void Read(ContentReader input)
        {
            LayerCount = input.ReadInt32();

            int tilesetCount = input.ReadInt32();
            Tilesets = new Tileset[tilesetCount];
            for (int i = 0; i < tilesetCount; i++)
            {
                Tilesets[i] = new Tileset();
                Tilesets[i].Read(this, input);
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

            // Read the patch information
            Patches = new Patch[LayerCount][];
            for (int layerIdx = 0; layerIdx < LayerCount; ++layerIdx)
            {
                int numPatches = input.ReadInt32();
                Patches[layerIdx] = new Patch[numPatches];
                for (int patchIdx = 0; patchIdx < numPatches; ++patchIdx)
                {
                    Patch patch = new Patch();
                    patch.Read(this, input);

                    Patches[layerIdx][patchIdx] = patch;
                }
            }

            // Read the trigger information
            int numTriggers = input.ReadInt32();
            Triggers = new Trigger[numTriggers];
            for (int triggerIdx = 0; triggerIdx < numTriggers; ++triggerIdx)
            {
                int type = input.ReadInt32();
                string name = input.ReadString();
                Vector2 pos = input.ReadObject<Vector2>();
                float downTime = input.ReadSingle();
                float triggerTime = input.ReadSingle();

                switch (type)
                {
                    case (int)(TriggerType.KillCount):
                        {
                            int threshold = input.ReadInt32();
                            Triggers[triggerIdx] = new KillCountTrigger(name, pos, downTime, triggerTime, threshold);
                        }
                        break;

                    default:
                        Debug.Assert(false, "Invalid trigger type found!");
                        break;
                };
            }
        }
    }




}
