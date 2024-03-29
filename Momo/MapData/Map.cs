
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Momo.Core;
using Momo.Core.Models;

namespace MapData
{
    public class Map
    {
        public int LayerCount { get; protected set; }

        public Tileset[] Tilesets { get; protected set; }

        public MapData.Patch[][] Patches { get; protected set; }

        public ModelInst[] ModelInstances { get; private set; }

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

            // Read in the scene object instances
            int numSceneObjects = input.ReadInt32();
            ModelInstances = new ModelInst[numSceneObjects];
            for (int sceneObjIdx = 0; sceneObjIdx < numSceneObjects; ++sceneObjIdx)
            {
                string modelName = input.ReadString();
                Matrix worldMatrix = input.ReadObject<Matrix>();

                Model model = ResourceManager.Instance.Get<Model>(modelName);

                ModelInstances[sceneObjIdx] = new ModelInst(model, worldMatrix);
            }

        }
    }

    public class MomoMap: Map
    {
        public Vector2[][] CollisionBoundaries { get; set; }

        public Vector2[] PlayerSpawnPoints { get; private set; }

        public Vector2 PlayAreaMin { get; private set; }
        public Vector2 PlayAreaMax { get; private set; }

        public SpawnPointData[] SpawnPoints { get; private set; }
        public SpawnPointData[] AmbientSpawnPoints { get; private set; }

        public TimerEventData[] TimerEvents { get; private set; }
        public SpawnEventData[] SpawnEvents { get; private set; }
        public TriggerCounterEventData[] TriggerCounterEvents { get; private set; }

        public PressurePlateData[] PressurePlates { get; private set; }

        public void Read(ContentReader input)
        {
            base.Read(input);

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
            int numPlayerSpawnPoints = input.ReadInt32();
            PlayerSpawnPoints = new Vector2[numPlayerSpawnPoints];
            for (int i = 0; i < numPlayerSpawnPoints; ++i)
            {
                PlayerSpawnPoints[i] = input.ReadVector2();
            }

            // Read the event spawn points information
            int numSpawnPoints = input.ReadInt32();
            SpawnPoints = new SpawnPointData[numSpawnPoints];
            for (int spawnPointIdx = 0; spawnPointIdx < numSpawnPoints; ++spawnPointIdx)
            {
                Vector2 pos = input.ReadVector2();
                float orientation = input.ReadSingle();
                SpawnPoints[spawnPointIdx] = new SpawnPointData(pos, orientation);
            }

            int numAmbientSpawnPoints = input.ReadInt32();
            AmbientSpawnPoints = new SpawnPointData[numAmbientSpawnPoints];
            for (int spawnPointIdx = 0; spawnPointIdx < numAmbientSpawnPoints; ++spawnPointIdx)
            {
                Vector2 pos = input.ReadVector2();
                float orientation = input.ReadSingle();
                AmbientSpawnPoints[spawnPointIdx] = new SpawnPointData(pos, orientation);
            }

            // Read in the vectors describing the playable area
            PlayAreaMin = input.ReadVector2();
            PlayAreaMax = input.ReadVector2();

            //// Read the pressure point information
            //int numPressurePlates = input.ReadInt32();
            //PressurePlates = new PressurePlateData[numPressurePlates];
            //for (int i = 0; i < numPressurePlates; ++i)
            //{
            //    PressurePlateData.Type type = (PressurePlateData.Type)(input.ReadInt32());
            //    string name = input.ReadString();
            //    Vector2 position = input.ReadVector2();
            //    float radius = input.ReadSingle();
            //    string trigger = input.ReadString();

            //    switch (type)
            //    {
            //        case PressurePlateData.Type.Normal:
            //            {
            //                PressurePlates[i] = new PressurePlateData(name, position, radius, trigger);
            //            }
            //            break;

            //        case PressurePlateData.Type.Interactive:
            //            {
            //                float interactTime = input.ReadSingle();
            //                PressurePlates[i] = new InteractivePressurePlateData(name, position, radius, trigger, interactTime);
            //            }
            //            break;
            //    }
            //}

            //// Read the event information
            //int numTimers = input.ReadInt32();
            //TimerEvents = new TimerEventData[numTimers];
            //for (int timerIdx = 0; timerIdx < numTimers; ++timerIdx)
            //{
            //    string name = input.ReadString();
            //    string startTrigger = input.ReadString();
            //    string endTrigger = input.ReadString();

            //    float time = input.ReadSingle();

            //    if (startTrigger == "")
            //        startTrigger = null;
            //    if (endTrigger == "")
            //        endTrigger = null;

            //    TimerEvents[timerIdx] = new TimerEventData(name, startTrigger, endTrigger, time);
            //}

            //int numSpawns = input.ReadInt32();
            //SpawnEvents = new SpawnEventData[numSpawns];
            //for (int spawnIdx = 0; spawnIdx < numSpawns; ++spawnIdx)
            //{
            //    string name = input.ReadString();
            //    string startTrigger = input.ReadString();
            //    string endTrigger = input.ReadString();

            //    float delay = input.ReadSingle();

            //    if (startTrigger == "")
            //        startTrigger = null;
            //    if (endTrigger == "")
            //        endTrigger = null;

            //    int enemyCount = input.ReadInt32();
            //    EnemyData[] enemies = new EnemyData[enemyCount];
            //    for (int i = 0; i < enemyCount; ++i)
            //    {
            //        EnemyData.Species species = (EnemyData.Species)(input.ReadInt32());
            //        Weapon.Design weapon = (Weapon.Design)(input.ReadInt32());
            //        enemies[i] = new EnemyData(species, weapon);
            //    }

            //    SpawnEvents[spawnIdx] = new SpawnEventData(name, startTrigger, endTrigger, delay, enemies);
            //}

            //int numTriggerCounters = input.ReadInt32();
            //TriggerCounterEvents = new TriggerCounterEventData[numTriggerCounters];
            //for (int triggerCounterIdx = 0; triggerCounterIdx < numTriggerCounters; ++triggerCounterIdx)
            //{
            //    string name = input.ReadString();
            //    string startTrigger = input.ReadString();
            //    string endTrigger = input.ReadString();

            //    string countTrigger = input.ReadString();
            //    int count = input.ReadInt32();

            //    if (startTrigger == "")
            //        startTrigger = null;

            //    TriggerCounterEvents[triggerCounterIdx] = new TriggerCounterEventData(name, startTrigger, endTrigger, countTrigger, count);
            //}
        }
    }




}
