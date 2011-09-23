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
    using Momo.Core;

    public class Map
    {
        public int LayerCount { get; private set; }

        public Tileset[] Tilesets { get; private set; }

        public Vector2[][] CollisionBoundaries { get; set; }

        public Vector2[] PlayerSpawnPoints { get; private set; }

        public Vector2 PlayAreaMin { get; private set; }
        public Vector2 PlayAreaMax { get; private set; }

        public SpawnGroupData[] SpawnGroups { get; private set; }

        public MapData.Patch[][] Patches { get; private set; }

        public TimerEventData[] TimerEvents { get; private set; }
        public SpawnEventData[] SpawnEvents { get; private set; }
        public TriggerCounterEventData[] TriggerCounterEvents { get; private set; }

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

            // Read the spawn group information
            int numSpawnGroups = input.ReadInt32();
            SpawnGroups = new SpawnGroupData[numSpawnGroups];
            for (int spawnGroupIdx = 0; spawnGroupIdx < numSpawnGroups; ++spawnGroupIdx)
            {
                Vector2 center = input.ReadObject<Vector2>();
                float boundingRadius = input.ReadSingle();
                RadiusInfo radiusInfo = new RadiusInfo(boundingRadius);

                int numEnemies = input.ReadInt32();
                SpawnPoint[] spawnPoints = new SpawnPoint[numEnemies];
                for(int enemyIdx = 0; enemyIdx < numEnemies; ++enemyIdx)
                {
                    Vector2 pos = input.ReadObject<Vector2>();
                    spawnPoints[enemyIdx] = new SpawnPoint(pos);
                }

                SpawnGroups[spawnGroupIdx] = new SpawnGroupData(spawnPoints, center, radiusInfo);
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

            // Read the event information
            int numTimers = input.ReadInt32();
            TimerEvents = new TimerEventData[numTimers];
            for (int timerIdx = 0; timerIdx < numTimers; ++timerIdx)
            {
                string name = input.ReadString();
                string startTrigger = input.ReadString();
                string endTrigger = input.ReadString();

                float time = input.ReadSingle();

                if (startTrigger == "")
                    startTrigger = null;
                if (endTrigger == "")
                    endTrigger = null;

                TimerEvents[timerIdx] = new TimerEventData(name, startTrigger, endTrigger, time);
            }

            int numSpawns = input.ReadInt32();
            SpawnEvents = new SpawnEventData[numSpawns];
            for (int spawnIdx = 0; spawnIdx < numSpawns; ++spawnIdx)
            {
                string name = input.ReadString();
                string startTrigger = input.ReadString();
                string endTrigger = input.ReadString();

                int count = input.ReadInt32();
                float delay = input.ReadSingle();

                if (startTrigger == "")
                    startTrigger = null;
                if (endTrigger == "")
                    endTrigger = null;

                SpawnEvents[spawnIdx] = new SpawnEventData(name, startTrigger, endTrigger, count, delay);
            }

            int numTriggerCounters = input.ReadInt32();
            TriggerCounterEvents = new TriggerCounterEventData[numTriggerCounters];
            for (int triggerCounterIdx = 0; triggerCounterIdx < numTriggerCounters; ++triggerCounterIdx)
            {
                string name = input.ReadString();
                string startTrigger = input.ReadString();
                string endTrigger = input.ReadString();

                string countTrigger = input.ReadString();
                int count = input.ReadInt32();

                if (startTrigger == "")
                    startTrigger = null;

                TriggerCounterEvents[triggerCounterIdx] = new TriggerCounterEventData(name, startTrigger, endTrigger, countTrigger, count);
            }
        }
    }




}
