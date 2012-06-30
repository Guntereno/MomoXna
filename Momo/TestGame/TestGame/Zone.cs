using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.GameEntities;
using Momo.Core.Nodes.Cameras;
using Momo.Core.Pathfinding;
using Momo.Core.Primitive2D;
using Momo.Core.Spatial;
using Momo.Debug;
using Momo.Fonts;

using Game.Entities;
using Game.Entities.AI;
using Game.Systems;

using WorldManager;
using Microsoft.Xna.Framework.Audio;


namespace Game
{
    public class Zone
    {
        private GameWorld mWorld = null;

        private Bin mBin = new Bin();
        private BinTimeStamps mBinPlayerHeatMap = new BinTimeStamps();
        private ContactList mContactList = new ContactList(4000);
        private ContactResolver mContactResolver = new ContactResolver();

        private MapData.Map mMap = null;
        private MapData.Renderer mMapRenderer = new MapData.Renderer();

        private List<BoundaryEntity> mBoundaries = new List<BoundaryEntity>(2000);

        private Random mRandom = new Random(1);


        private PlayerManager mPlayerManager = null;
        private WeaponManager mWeaponManager = null;
        private ProjectileManager mProjectileManager = null;
        private AiEntityManager mAiEntityManager = null;
        private OsdManager mOsdManager = null;
        private TriggerManager mTriggerManager = null;
        private CorpseManager mCorpseManager = null;
        private PathRouteManager mPathRouteManager = null;


        private PathIsland mPathIsland = new PathIsland();

        private uint mUpdateTokenOffset = 0;


        public GameWorld World                              { get { return mWorld; } }
        public Bin Bin                                      { get { return mBin; } }
        public BinTimeStamps BinPlayerHeatMap               { get { return mBinPlayerHeatMap; } }

        public PlayerManager PlayerManager                  { get { return mPlayerManager; } }
        public WeaponManager WeaponManager                  { get { return mWeaponManager; } }
        public ProjectileManager ProjectileManager          { get { return mProjectileManager; } }
        public AiEntityManager EnemyManager                 { get { return mAiEntityManager; } }
        public TriggerManager TriggerManager                { get { return mTriggerManager; } }
        public CorpseManager CorpseManager                  { get { return mCorpseManager; } }
        public PathRouteManager PathRouteManager            { get { return mPathRouteManager; } }

        public Random Random                                { get { return mRandom; } }

        public MapData.Map Map                              { get { return mMap; } }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public Zone(GameWorld world)
        {
            mWorld = world;

            mWeaponManager = new WeaponManager(this);
            mProjectileManager = new ProjectileManager(this);
            mAiEntityManager = new AiEntityManager(this);
            mOsdManager = new OsdManager(this);
            mPlayerManager = new PlayerManager(this);
            mTriggerManager = new TriggerManager();
            mCorpseManager = new CorpseManager(this);
            mPathRouteManager = new PathRouteManager();
        }


        public void Load()
        {
            mMap = ResourceManager.Instance.Get<MapData.Map>("maps/zone01/zone01");

            mBin.Init(50, 50, mMap.PlayAreaMax + new Vector2(1000.0f, 1000.0f), BinLayers.kLayerCount, 8000, 1000, 1000);
            mBinPlayerHeatMap.Init(mBin);


            // ----------------------------------------------------------------
            // -- Init the pools
            // ----------------------------------------------------------------
            mPlayerManager.Load();
            mWeaponManager.Load();
            mProjectileManager.Load();
            mAiEntityManager.Load();
            mCorpseManager.Load();


            mPlayerManager.AddPlayer(Game.Instance.InputManager.GetInputWrapper(0));


            // TODO: Temp: Disconnects need to be handled in the InputManager
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.Two);
            if (gamePadState.IsConnected)
            {
                mPlayerManager.AddPlayer(Game.Instance.InputManager.GetInputWrapper(1));
            }


            mMapRenderer.Init(mMap, Game.Instance.GraphicsDevice, 16);



            BuildCollisionBoundaries(0.0f, BinLayers.kBoundary, false, true);
            BuildCollisionBoundaries(10.0f, BinLayers.kBoundaryOcclusionSmall, true, false);
            BuildCollisionBoundaries(11.0f, BinLayers.kBoundaryObstructionSmall, true, false);


            // Path stuff
            float smallPathNodeRadius = 25.0f;
            PathRegion[] regions = new PathRegion[1];
            Vector2[][] extrudeBoundariesSmallPath = ExtrudeCollisionBoundaries(smallPathNodeRadius, false);
            regions[0] = new PathRegion(new Vector2(75.0f, 75.0f), new Vector2(2000.0f, 2000.0f));
            //regions[0].GenerateNodesFromBoundaries(smallPathNodeRadius, 80, true, extrudeBoundariesSmallPath);
            regions[0].GenerateNodePaths(mBin, BinLayers.kBoundaryObstructionSmall);
            mPathIsland.SetRegions(regions);

            AddPathIslandToBin(mPathIsland);

            mPathRouteManager.Init(1000, 100, 200);

            CollisionHelpers.Init();
            PathFindingHelpers.Init(400.0f, 3, mBin);


            for (int i = 0; i < 0; ++i)
            {
                float x = 1000.0f + 1984.0f + ((float)mRandom.NextDouble() * 128.0f);
                float y = 1000.0f + 192.0f + ((float)mRandom.NextDouble() * 3520.0f);
                mAiEntityManager.Create(typeof(Civilian), new Vector2(x, y));
            }

            for (int i = 0; i < 0; ++i)
            {
                float x = 1000.0f + 1984.0f + ((float)mRandom.NextDouble() * 128.0f);
                float y = 1000.0f + 192.0f + ((float)mRandom.NextDouble() * 3520.0f);
                mAiEntityManager.Create(typeof(Zombie), new Vector2(x, y));
            }

            // Add the ambient spawn objects to the bin
            const float kSpawnPointRadius = 16.0f;
            for (int i = 0; i < Map.AmbientSpawnPoints.Length; ++i)
            {
                MapData.SpawnPointData spawnPoint = Map.AmbientSpawnPoints[i];
                Bin.AddBinItem(spawnPoint, spawnPoint.Position, kSpawnPointRadius, BinLayers.kAmbientSpawnPoints);
            }
        }


        public void Enter()
        {
            mTriggerManager.GetTrigger(TriggerManager.kDefaultTriggerName).Activate();
        }


        public void Update(ref FrameTime frameTime)
        {
            Input.InputWrapper inputWrapper = Game.Instance.InputManager.GetInputWrapper(0);

            int updateIterationCnt = 1;
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                updateIterationCnt = 10;
            }

            for (int i = 0; i < updateIterationCnt; ++i)
            {
                ++mUpdateTokenOffset;


                mPlayerManager.Update(ref frameTime);
                mAiEntityManager.Update(ref frameTime, mUpdateTokenOffset);
                mProjectileManager.Update(ref frameTime);
                mOsdManager.Update(ref frameTime);

                mWeaponManager.Update(ref frameTime);

                mPathRouteManager.Update(ref frameTime);

                //m_eventManager.Update(ref frameTime);
                mCorpseManager.Update(ref frameTime);


                // Collision detection/resolution
                mContactList.StartAddingContacts();

                GenerateContacts();

                mContactList.EndAddingContacts();

                mContactResolver.ResolveContacts(frameTime.Dt, mContactList);



                // Move the camera follow position
                // Update camera after collision resolution so its go the actual rendered player position.
                mPlayerManager.UpdateAveragePosition();
   
                mWeaponManager.PostUpdate();
                mProjectileManager.PostUpdate();
                mAiEntityManager.PostUpdate();
                mPlayerManager.PostUpdate();
                mCorpseManager.PostUpdate();
            }


            BinRegionUniform binRegion = new BinRegionUniform();
            mPlayerManager.Players[0].GetBinRegion(ref binRegion);

            BinRegionUniform heatRegion = new BinRegionUniform( new BinLocation(binRegion.MinLocation.X - 4, binRegion.MinLocation.Y - 3), new BinLocation(binRegion.MinLocation.X + 4, binRegion.MinLocation.Y + 3));
            mBinPlayerHeatMap.UpdateHeatMap(ref heatRegion, World.CurrentTimeStamp);

            // Move the camera follow position
            // Update camera after collision resolution so its go the actual rendered player position.
            World.CameraController.FollowPosition = mPlayerManager.AveragePlayerPosition;
        }


        static readonly int[] kProjectileEntityLayers = { BinLayers.kPlayerEntity, BinLayers.kEnemyEntities, BinLayers.kCivilianEntities };

        private void GenerateContacts()
        {
            // Check groups against each other
            CollisionHelpers.GenerateEntityContacts(mPlayerManager.Players.ActiveItemList, mPlayerManager.Players.ActiveItemListCount, 1.0f, mBin, BinLayers.kPlayerEntity, mContactList);
            CollisionHelpers.GenerateEntityContacts(mAiEntityManager.Entities.ActiveItemList, mAiEntityManager.Entities.ActiveItemListCount, 0.9f, mBin, BinLayers.kEnemyEntities, mContactList);
            CollisionHelpers.GenerateEntityContacts(mAiEntityManager.Entities.ActiveItemList, mAiEntityManager.Entities.ActiveItemListCount, 0.9f, mBin, BinLayers.kCivilianEntities, mContactList);

            // Players against enemies
            CollisionHelpers.GenerateEntityContacts(mPlayerManager.Players.ActiveItemList, mPlayerManager.Players.ActiveItemListCount, 0.7f, mBin, BinLayers.kEnemyEntities, mContactList);
            CollisionHelpers.GenerateEntityContacts(mPlayerManager.Players.ActiveItemList, mPlayerManager.Players.ActiveItemListCount, 0.7f, mBin, BinLayers.kCivilianEntities, mContactList);

            // Check against boundaries
            CollisionHelpers.GenerateBoundaryContacts(mPlayerManager.Players.ActiveItemList, mPlayerManager.Players.ActiveItemListCount, mBin, BinLayers.kBoundary, mContactList);
            CollisionHelpers.GenerateBoundaryContacts(mAiEntityManager.Entities.ActiveItemList, mAiEntityManager.Entities.ActiveItemListCount, mBin, BinLayers.kBoundary, mContactList);


            // Check projectiles
            CollisionHelpers.GenerateProjectileContacts(mProjectileManager.Bullets.ActiveItemList, mProjectileManager.Bullets.ActiveItemListCount, mBin, kProjectileEntityLayers, BinLayers.kBoundary);
        }


        public void Exit()
        {

        }


        public void Flush()
        {

        }


        public void PreRender()
        {

        }


        public void Render()
        {
            mMapRenderer.Render(World.Camera, Game.Instance.GraphicsDevice);
            mAiEntityManager.Render(World.Camera, Game.Instance.GraphicsDevice);
            mPlayerManager.Render(World.Camera, Game.Instance.GraphicsDevice);

            mOsdManager.Render();
        }


        public void PostRender()
        {

        }


        public void DebugRender()
        {
            for (int i = 0; i < mBoundaries.Count; ++i)
                mBoundaries[i].DebugRender(World.DebugRenderer);

            mCorpseManager.DebugRender(World.DebugRenderer);

            mPlayerManager.DebugRender(World.DebugRenderer);

            mAiEntityManager.DebugRender(World.DebugRenderer);
            mProjectileManager.DebugRender(World.DebugRenderer);

            mOsdManager.DebugRender(World.DebugRenderer);

            //m_pathIsland.DebugRender(World.DebugRenderer);
            //m_pathRouteManager.DebugRender(World.DebugRenderer, m_debugTextPrinter, m_debugTextStyle);

            //for (int i = 0; i < m_pathIsland.GetRegions()[0].GetNodeCount(); ++i)
            //{
            //    PathNode node = m_pathIsland.GetRegions()[0].GetNodes()[i];

            //    Vector2 worldPos2d = node.GetPosition();
            //    Vector3 worldPos = new Vector3(worldPos2d.X, worldPos2d.Y, 0.0f);
            //    Vector2 screenPos = GetCamera().GetScreenPosition(worldPos);

            //    m_debugTextPrinter.AddToDrawList(node.GetUniqueId().ToString(), Color.White, Color.Black, screenPos, m_debugTextStyle);
            //}


            //mBin.DebugRender(World.DebugRenderer, 5, BinLayers.kPathNodes);
            //mBin.DebugRender(World.DebugRenderer, PathFindingHelpers.ms_circularSearchRegions[0], new Color(0.20f, 0.0f, 0.0f, 0.5f));
            //mBin.DebugRender(World.DebugRenderer, PathFindingHelpers.ms_circularSearchRegions[1], new Color(0.40f, 0.0f, 0.0f, 0.5f));
            //mBin.DebugRender(World.DebugRenderer, PathFindingHelpers.ms_circularSearchRegions[2], new Color(0.60f, 0.0f, 0.0f, 0.5f));
            //mBin.DebugRender(World.DebugRenderer, PathFindingHelpers.ms_circularSearchRegions[3], new Color(0.80f, 0.0f, 0.0f, 0.5f));
            //mBin.DebugRenderGrid(World.DebugRenderer, Color.Orange, Color.DarkRed);

            Color spawnPointEdge = new Color(0.0f, 1.0f, 0.0f, 0.7f);
            Color spawnPointFill = spawnPointEdge;
            spawnPointEdge.A = 76;

            for (int i = 0; i < Map.AmbientSpawnPoints.Length; ++i)
            {
                MapData.SpawnPointData spawnPoint = Map.AmbientSpawnPoints[i];

                Vector2 pos = spawnPoint.Position;
                double orient = (double)spawnPoint.Orientation;
                const float kDirLen = 24.0f;
                Vector2 dir = new Vector2(-(float)Math.Sin(orient), -(float)Math.Cos(orient));

                World.DebugRenderer.DrawCircle(pos, 16.0f, spawnPointEdge, spawnPointFill, true, 0.01f, 6);
                World.DebugRenderer.DrawLine(pos, pos + (dir * kDirLen), spawnPointEdge);
            }

            //mBinPlayerHeatMap.DebugRender(World.DebugRenderer, World.CurrentTimeStamp, 500);
        }


        // Returns true if the list was populated with the requested entities. You must call AddToBin() and SetPosition()
        // when bringing the objects into the world.
        // If you decide you dont want some of the entities, call DestroyItem().
        public bool RequestEntities(Type entityType, int count, ref AiEntity[] outEntities)
        {
            const float kOffscreenDistance = 800.0f;
            const float kOffscreenDistanceSqrd = kOffscreenDistance * kOffscreenDistance;
            const int kTargetEntityCnt = 150;


            // Work out how many spare entities we have.
            int spareEntityCnt = kTargetEntityCnt - mAiEntityManager.Entities.ActiveItemListCount;
            if (spareEntityCnt < 0)
            {
                spareEntityCnt = 0;
            }

            int requirdRecycleCnt = count - spareEntityCnt;
            if (requirdRecycleCnt < 0)
            {
                requirdRecycleCnt = 0;
            }

            int recycledCnt = 0;
            int returnEntityCnt = 0;


            // Try and recycle the required amount.
            if (requirdRecycleCnt > 0)
            {
                Vector3 cameraPos3 = World.Camera.Matrix.Translation;
                Vector2 cameraPos = new Vector2(cameraPos3.X, cameraPos3.Y);

                for (int i = 0; i < mAiEntityManager.Entities.ActiveItemListCount; ++i)
                {
                    AiEntity entity = mAiEntityManager.Entities[i];

                    Vector2 dCameraEntityPos = cameraPos - entity.GetPosition();
                    float dCameraEntityPosLenSqrd = dCameraEntityPos.LengthSquared();

                    if (dCameraEntityPosLenSqrd > kOffscreenDistanceSqrd)
                    {
                        outEntities[recycledCnt] = entity;
                        ++recycledCnt;

                        if (recycledCnt == requirdRecycleCnt)
                        {
                            break;
                        }
                    }
                }
            }

            if (recycledCnt == requirdRecycleCnt)
            {
                // Destroy the old ones
                for (int i = 0; i < recycledCnt; ++i)
                {
                    outEntities[i].DestroyItem();
                }

                // Make some new ones
                while (returnEntityCnt < count)
                {
                    outEntities[returnEntityCnt] = mAiEntityManager.Create(entityType);
                    ++returnEntityCnt;
                }

                return true;
            }
            else
            {
                // Reset the attempted recycle list as we did not get enough.
                for (int i = 0; i < recycledCnt; ++i)
                {
                    outEntities[i] = null;
                }

                return false;
            }
        }



        private Vector2[][] BuildCollisionBoundaries(float extrudeAmount, int binLayer, bool roundedCorners, bool addToList)
        {
            Vector2[][] extrudedCollisionBoundary = ExtrudeCollisionBoundaries(extrudeAmount, roundedCorners);
            int numBoundries = extrudedCollisionBoundary.Length;


            for (int i = 0; i < numBoundries; ++i)
            {
                int numNodes = extrudedCollisionBoundary[i].Length;
                Vector2 lastPoint = extrudedCollisionBoundary[i][0];


                for (int nodeIdx = 1; nodeIdx < numNodes; ++nodeIdx)
                {
                    Vector2 pos = extrudedCollisionBoundary[i][nodeIdx];

                    LinePrimitive2D lineStrip = new LinePrimitive2D(lastPoint, pos);
                    BoundaryEntity boundaryEntity = new BoundaryEntity(lineStrip);
                    boundaryEntity.AddToBin(mBin, binLayer);

                    if(addToList)
                        mBoundaries.Add(boundaryEntity);

                    lastPoint = pos;
                }
            }

            return extrudedCollisionBoundary;
        }


        private Vector2[][] ExtrudeCollisionBoundaries(float extrudeAmount, bool roundedCorners)
        {
            int numBoundries = mMap.CollisionBoundaries.Length;
            Vector2[][] extrudedCollisionBoundary = new Vector2[numBoundries][];

            for (int i = 0; i < numBoundries; ++i)
            {
                if(roundedCorners)
                    Momo.Maths.ExtendedMaths2D.ExtrudePointsAlongNormalRounded(mMap.CollisionBoundaries[i], mMap.CollisionBoundaries[i].Length, extrudeAmount, 0.55f, out extrudedCollisionBoundary[i]);
                else
                    Momo.Maths.ExtendedMaths2D.ExtrudePointsAlongNormal(mMap.CollisionBoundaries[i], mMap.CollisionBoundaries[i].Length, extrudeAmount, out extrudedCollisionBoundary[i]);
            }

            return extrudedCollisionBoundary;
        }


        private void AddPathIslandToBin(PathIsland island)
        {
            PathRegion[] regions = island.GetRegions();

            for (int i = 0; i < regions.Length; ++i)
            {
                PathNode[] nodes = regions[i].GetNodes();
                int nodeCnt = regions[i].GetNodeCount();

                for (int j = 0; j < nodeCnt; ++j)
                {
                    nodes[j].AddToBin(mBin, nodes[j].GetPosition(), nodes[j].GetRadius(), BinLayers.kPathNodes);
                }
            }
        }
    }
}
