using System;
using System.Collections.Generic;
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
using TestGame.Systems;
using WorldManager;



namespace TestGame
{
    public class GameWorld : World
    {
        public static World WorldCreator() { return new GameWorld(); }

        private OrthographicCameraNode m_camera = new OrthographicCameraNode("TestCamera");
        private CameraController m_cameraController = null;

        private Bin m_bin = new Bin();
        private ContactList m_contactList = new ContactList(4000);
        private ContactResolver m_contactResolver = new ContactResolver();

        private MapData.Map m_map = null;
        private MapData.Renderer m_mapRenderer = new MapData.Renderer();

        private List<BoundaryEntity> m_boundaries = new List<BoundaryEntity>(2000);

        private Random m_random = new Random();

        // Debug
        private DebugRenderer m_debugRenderer = new DebugRenderer();
        private Font m_debugFont = null;
        private TextStyle m_debugTextStyle = null;
        private TextBatchPrinter m_debugTextPrinter = new TextBatchPrinter();


        private PlayerManager m_playerManager = null;
        private WeaponManager m_weaponManager = null;
        private ProjectileManager m_projectileManager = null;
        private EnemyManager m_enemyManager = null;
        private OsdManager m_osdManager = null;
        private TriggerManager m_triggerManager = null;
        private EventManager m_eventManager = null;
        private SpawnPointManager m_spawnGroupManager = null;

        private TextBatchPrinter m_textPrinter = new TextBatchPrinter();

        private PathIsland m_pathIsland = new PathIsland();
        private PathRouteManager m_pathRouteManager = new PathRouteManager();

        private int m_updateTokenOffset = 0;

        float m_elapsedTime = 0.0f;

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public GameWorld()
        {
            m_cameraController = new CameraController(this);
            m_weaponManager = new WeaponManager(this);
            m_projectileManager = new ProjectileManager(this, m_bin);
            m_enemyManager = new EnemyManager(this, m_bin);
            m_osdManager = new OsdManager(this);
            m_playerManager = new PlayerManager(this, m_bin);
            m_triggerManager = new TriggerManager();
            m_eventManager = new EventManager(this);
            m_spawnGroupManager = new SpawnPointManager(this);
        }



        public OrthographicCameraNode GetCamera()               { return m_camera; }

        public PlayerManager GetPlayerManager()                 { return m_playerManager; }
        public WeaponManager GetWeaponManager()                 { return m_weaponManager; }
        public ProjectileManager GetProjectileManager()         { return m_projectileManager; }
        public EnemyManager GetEnemyManager()                   { return m_enemyManager; }
        public TriggerManager GetTriggerManager()               { return m_triggerManager; }
        public EventManager GetEventManager()                   { return m_eventManager; }
        public SpawnPointManager GetSpawnPointManager()         { return m_spawnGroupManager; }

        public TextBatchPrinter GetTextPrinter()                { return m_textPrinter; }
        public Random GetRandom()                               { return m_random; }
        public DebugRenderer GetDebugRenderer()                 { return m_debugRenderer; }

        public MapData.Map GetMap()                             { return m_map; }

        public PathRouteManager GetPathRouteManager()           { return m_pathRouteManager; }

        public float GetElapsedTime()                           { return m_elapsedTime; }



        public override void Load()
        {
            Effect textEffect = TestGame.Instance().Content.Load<Effect>("effects/text");

            // Debug
            m_debugRenderer.Init(50000, 1000, TestGame.Instance().GraphicsDevice);
            m_debugTextPrinter.Init(textEffect, new Vector2((float)TestGame.kBackBufferWidth, (float)TestGame.kBackBufferHeight), 500, 1000, 1);
            m_debugFont = TestGame.Instance().Content.Load<Font>("fonts/Consolas_24_o2");
            m_debugTextStyle = new TextStyle(m_debugFont, TextSecondaryDrawTechnique.kDropshadow);

            m_textPrinter.Init(textEffect, new Vector2((float)TestGame.kBackBufferWidth, (float)TestGame.kBackBufferHeight), 100, 1000, 1);


            m_camera.ViewWidth = TestGame.kBackBufferWidth;
            m_camera.ViewHeight = TestGame.kBackBufferHeight;
            m_camera.LocalTranslation = new Vector3(300.0f, 750.0f, 10.0f);

            m_cameraController.Camera = m_camera;

            m_map = TestGame.Instance().Content.Load<MapData.Map>("maps/test_arena2/test_arena2");
            //m_map = TestGame.Instance().Content.Load<MapData.Map>("maps/tom_owes_me/tom_owes_me");

            m_bin.Init(50, 50, m_map.PlayAreaMax + new Vector2(1000.0f, 1000.0f), BinLayers.kLayerCount, 6000, 1000, 1000);


            // ----------------------------------------------------------------
            // -- Init the pools
            // ----------------------------------------------------------------

            m_playerManager.Load();
            m_weaponManager.Load();
            m_projectileManager.Load();

            m_playerManager.AddPlayer(TestGame.Instance().InputManager.GetInputWrapper(0));

            // TODO: Temp: Disconnects need to be handled in the InputManager
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.Two);
            if (gamePadState.IsConnected)
            {
                m_playerManager.AddPlayer(TestGame.Instance().InputManager.GetInputWrapper(1));
            }


            m_mapRenderer.Init(m_map, TestGame.Instance().GraphicsDevice, 16);


            float smallPathNodeRadius = 25.0f;
            BuildCollisionBoundaries(0.0f, BinLayers.kBoundary);

            BuildCollisionBoundaries(13.0f, BinLayers.kBoundaryViewSmall);
            BuildCollisionBoundaries(18.0f, BinLayers.kBoundaryPathFindingSmall);
            BuildCollisionBoundaries(smallPathNodeRadius - 1, BinLayers.kBoundaryNodeConnectingSmall);

            Vector2[][] extrudeBoundariesSmallPath = ExtrudeCollisionBoundaries(smallPathNodeRadius);
   

            // Path stuff
            PathRegion[] regions = new PathRegion[1];
            regions[0] = new PathRegion(new Vector2(75.0f, 75.0f), new Vector2(2000.0f, 2000.0f));
            regions[0].GenerateNodesFromBoundaries(smallPathNodeRadius, 30, true, extrudeBoundariesSmallPath);
            regions[0].GenerateNodePaths(m_bin, BinLayers.kBoundaryNodeConnectingSmall);
            m_pathIsland.SetRegions(regions);

            AddPathIslandToBin(m_pathIsland);

            m_pathRouteManager.Init(1000, 100, 200);

            CollisionHelpers.Init();
            PathFindingHelpers.Init(400.0f, 3, m_bin);

            m_eventManager.LoadEvents(m_map);
            m_spawnGroupManager.LoadSpawnGroups(m_map);
        }


        public override void Enter()
        {
            m_triggerManager.GetTrigger(TriggerManager.kDefaultTriggerName).Activate();
        }


        public override void Update(float dt)
        {
            m_elapsedTime += dt;

            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime(dt);

            ++m_updateTokenOffset;

            Input.InputWrapper inputWrapper = TestGame.Instance().InputManager.GetInputWrapper(0);

            m_playerManager.Update(ref frameTime);

            // Move the camera follow position
            m_cameraController.FollowPosition = m_playerManager.GetAveragePosition();

            m_enemyManager.Update(ref frameTime, m_updateTokenOffset);
            m_projectileManager.Update(ref frameTime);
            m_osdManager.Update(ref frameTime);

            m_weaponManager.Update(ref frameTime);

            m_pathRouteManager.Update(ref frameTime);

            m_eventManager.Update(ref frameTime);

            // Collision detection/resolution
            m_contactList.StartAddingContacts();
            CollisionHelpers.GenerateContacts(m_enemyManager.GetMeleeEnemies().ActiveItemList, m_enemyManager.GetMeleeEnemies().ActiveItemListCount, m_bin, m_contactList);
            CollisionHelpers.GenerateContacts(m_playerManager.GetPlayers().ActiveItemList, m_playerManager.GetPlayers().ActiveItemListCount, m_bin, m_contactList);
            CollisionHelpers.UpdateBulletContacts(m_projectileManager.GetBullets().ActiveItemList, m_projectileManager.GetBullets().ActiveItemListCount, m_bin);
            m_contactList.EndAddingContacts();

            m_contactResolver.ResolveContacts(frameTime.Dt, m_contactList);

            m_projectileManager.EndFrame();

            m_cameraController.Update(ref frameTime, ref inputWrapper);
        }


        public override void Exit()
        {

        }


        public override void Flush()
        {

        }


        public override void PreRender()
        {
            m_camera.PreRenderUpdate();
        }


        public override void Render()
        {
            m_mapRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, TestGame.Instance().GraphicsDevice);

            m_osdManager.Render();

            m_textPrinter.Render(true, TestGame.Instance().GraphicsDevice);
            m_textPrinter.ClearDrawList();
        }


        public override void PostRender()
        {

        }


        public override void DebugRender()
        {
            //for (int i = 0; i < m_boundaries.Count; ++i)
            //{
            //    m_boundaries[i].DebugRender(m_debugRenderer);
            //}

            for (int i = 0; i < m_playerManager.GetPlayers().ActiveItemListCount; ++i)
            {
                m_playerManager.GetPlayers()[i].DebugRender(m_debugRenderer);
            }

            m_enemyManager.DebugRender(m_debugRenderer);
            m_projectileManager.DebugRender(m_debugRenderer);
            m_osdManager.DebugRender(m_debugRenderer);

            //m_triggerController.DebugRender(m_debugRenderer, m_debugTextPrinter, m_debugTextStyle);
            m_cameraController.DebugRender(m_debugRenderer, m_debugTextPrinter, m_debugTextStyle);

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

            m_debugRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, TestGame.Instance().GraphicsDevice);
            m_debugRenderer.Clear();

            // Render any debug text objects that where added.
            m_debugTextPrinter.Render(true, TestGame.Instance().GraphicsDevice);
            m_debugTextPrinter.ClearDrawList();
        }


        private Vector2[][] BuildCollisionBoundaries(float extrudeAmount, int binLayer)
        {
            int numBoundries = m_map.CollisionBoundaries.Length;
            Vector2[][] extrudedCollisionBoundary = ExtrudeCollisionBoundaries(extrudeAmount);


            for (int i = 0; i < numBoundries; ++i)
            {
                int numNodes = m_map.CollisionBoundaries[i].Length;

                Vector2 lastPoint = extrudedCollisionBoundary[i][0];


                for (int nodeIdx = 1; nodeIdx < numNodes; ++nodeIdx)
                {
                    Vector2 pos = extrudedCollisionBoundary[i][nodeIdx];

                    LinePrimitive2D lineStrip = new LinePrimitive2D(lastPoint, pos);
                    BoundaryEntity boundaryEntity = new BoundaryEntity(lineStrip);
                    boundaryEntity.AddToBin(m_bin, binLayer);
                    m_boundaries.Add(boundaryEntity);

                    lastPoint = pos;
                }
            }

            return extrudedCollisionBoundary;
        }


        private Vector2[][] ExtrudeCollisionBoundaries(float extrudeAmount)
        {
            int numBoundries = m_map.CollisionBoundaries.Length;
            Vector2[][] extrudedCollisionBoundary = new Vector2[numBoundries][];

            for (int i = 0; i < numBoundries; ++i)
            {
                Momo.Maths.ExtendedMaths2D.ExtrudePointsAlongNormal(m_map.CollisionBoundaries[i], m_map.CollisionBoundaries[i].Length, true, extrudeAmount, out extrudedCollisionBoundary[i]);
                //Momo.Maths.ExtendedMaths2D.ExtrudePointsAlongNormalRounded(m_map.CollisionBoundaries[i], m_map.CollisionBoundaries[i].Length, true, extrudeAmount, 0.45f, out extrudedCollisionBoundary[i]);
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
                    nodes[j].AddToBin(m_bin, nodes[j].GetPosition(), nodes[j].GetRadius(), BinLayers.kPathNodes);
                }
            }
        }
    }
}
