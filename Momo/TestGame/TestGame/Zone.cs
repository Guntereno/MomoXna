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

using TestGame.Entities;
using TestGame.Entities.AI;
using TestGame.Systems;

using WorldManager;
using Microsoft.Xna.Framework.Audio;



namespace TestGame
{
    public class Zone
    {
        private GameWorld mWorld = null;

        private Bin mBin = new Bin();
        private ContactList mContactList = new ContactList(4000);
        private ContactResolver mContactResolver = new ContactResolver();

        private MapData.Map mMap = null;
        private MapData.Renderer mMapRenderer = new MapData.Renderer();

        private List<BoundaryEntity> mBoundaries = new List<BoundaryEntity>(2000);

        private Random mRandom = new Random();


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

        private float mElapsedTime = 0.0f;


        public GameWorld World                              { get { return mWorld; } }
        public Bin Bin                                      { get { return mBin; } }

        public PlayerManager PlayerManager                  { get { return mPlayerManager; } }
        public WeaponManager WeaponManager                  { get { return mWeaponManager; } }
        public ProjectileManager ProjectileManager          { get { return mProjectileManager; } }
        public AiEntityManager EnemyManager                 { get { return mAiEntityManager; } }
        public TriggerManager TriggerManager                { get { return mTriggerManager; } }
        public CorpseManager CorpseManager                  { get { return mCorpseManager; } }
        public PathRouteManager PathRouteManager            { get { return mPathRouteManager; } }

        public Random Random                                { get { return mRandom; } }

        public MapData.Map Map                              { get { return mMap; } }

        public float ElapsedTime                            { get { return mElapsedTime; } }



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
            mMap = TestGame.Instance().ResourceManager.Get<MapData.Map>("maps/test_arena2/test_arena2");

            mBin.Init(50, 50, mMap.PlayAreaMax + new Vector2(1000.0f, 1000.0f), BinLayers.kLayerCount, 6000, 1000, 1000);


            // ----------------------------------------------------------------
            // -- Init the pools
            // ----------------------------------------------------------------
            mPlayerManager.Load();
            mWeaponManager.Load();
            mProjectileManager.Load();
            mAiEntityManager.Load();
            mCorpseManager.Load();


            mPlayerManager.AddPlayer(TestGame.Instance().InputManager.GetInputWrapper(0));


            // TODO: Temp: Disconnects need to be handled in the InputManager
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.Two);
            if (gamePadState.IsConnected)
            {
                mPlayerManager.AddPlayer(TestGame.Instance().InputManager.GetInputWrapper(1));
            }


            mMapRenderer.Init(mMap, TestGame.Instance().GraphicsDevice, 16);


            float smallPathNodeRadius = 25.0f;
            BuildCollisionBoundaries(0.0f, BinLayers.kBoundary, false, true);
            BuildCollisionBoundaries(5.0f, BinLayers.kBoundaryOcclusionSmall, true, false);
            BuildCollisionBoundaries(12.0f, BinLayers.kBoundaryObstructionSmall, true, false);


            // Path stuff
            PathRegion[] regions = new PathRegion[1];
            Vector2[][] extrudeBoundariesSmallPath = ExtrudeCollisionBoundaries(smallPathNodeRadius, false);
            regions[0] = new PathRegion(new Vector2(75.0f, 75.0f), new Vector2(2000.0f, 2000.0f));
            regions[0].GenerateNodesFromBoundaries(smallPathNodeRadius, 30, true, extrudeBoundariesSmallPath);
            regions[0].GenerateNodePaths(mBin, BinLayers.kBoundaryObstructionSmall);
            mPathIsland.SetRegions(regions);

            AddPathIslandToBin(mPathIsland);

            mPathRouteManager.Init(1000, 100, 200);

            CollisionHelpers.Init();
            PathFindingHelpers.Init(400.0f, 3, mBin);

            for (int i = 0; i < 0; ++i)
            {
                float x = 1840.0f + ((float)mRandom.NextDouble() * 420.0f);
                float y = 3880.0f + ((float)mRandom.NextDouble() * 700.0f);
                mAiEntityManager.Create(typeof(Civilian), new Vector2(x, y));
            }

            for (int i = 0; i < 100; ++i)
            {
                float x = 1840.0f + ((float)mRandom.NextDouble() * 250.0f);
                float y = 4000.0f + ((float)mRandom.NextDouble() * 750.0f);
                mAiEntityManager.Create(typeof(Zombie), new Vector2(x, y));
            }
        }


        public void Enter()
        {
            mTriggerManager.GetTrigger(TriggerManager.kDefaultTriggerName).Activate();
        }


        public void Update(float dt)
        {
            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime(dt);
            
            Input.InputWrapper inputWrapper = TestGame.Instance().InputManager.GetInputWrapper(0);

            int updateIterationCnt = 1;
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                updateIterationCnt = 15;
            }

            for (int i = 0; i < updateIterationCnt; ++i)
            {
                mElapsedTime += frameTime.Dt;

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
            mMapRenderer.Render(World.Camera.ViewMatrix, World.Camera.ProjectionMatrix, TestGame.Instance().GraphicsDevice);

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

            //m_pathIsland.DebugRender(m_debugRenderer);
            //m_pathRouteManager.DebugRender(m_debugRenderer, m_debugTextPrinter, m_debugTextStyle);

            //for (int i = 0; i < m_pathIsland.GetRegions()[0].GetNodeCount(); ++i)
            //{
            //    PathNode node = m_pathIsland.GetRegions()[0].GetNodes()[i];

            //    Vector2 worldPos2d = node.GetPosition();
            //    Vector3 worldPos = new Vector3(worldPos2d.X, worldPos2d.Y, 0.0f);
            //    Vector2 screenPos = GetCamera().GetScreenPosition(worldPos);

            //    m_debugTextPrinter.AddToDrawList(node.GetUniqueId().ToString(), Color.White, Color.Black, screenPos, m_debugTextStyle);
            //}


            //m_bin.DebugRender(m_debugRenderer, 5, BinLayers.kPathNodes);
            //m_bin.DebugRender(m_debugRenderer, PathFindingHelpers.ms_circularSearchRegions[0], new Color(0.20f, 0.0f, 0.0f, 0.5f));
            //m_bin.DebugRender(m_debugRenderer, PathFindingHelpers.ms_circularSearchRegions[1], new Color(0.40f, 0.0f, 0.0f, 0.5f));
            //m_bin.DebugRender(m_debugRenderer, PathFindingHelpers.ms_circularSearchRegions[2], new Color(0.60f, 0.0f, 0.0f, 0.5f));
            //m_bin.DebugRender(m_debugRenderer, PathFindingHelpers.ms_circularSearchRegions[3], new Color(0.80f, 0.0f, 0.0f, 0.5f));
            //m_bin.DebugRenderGrid(m_debugRenderer, Color.Orange, Color.DarkRed);


            //m_spawnGroupManager.DebugRender(m_debugRenderer);
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
