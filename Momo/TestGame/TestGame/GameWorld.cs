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
using TestGame.Entities.AI;
using TestGame.Systems;

using WorldManager;
using Microsoft.Xna.Framework.Audio;



namespace TestGame
{
    public class GameWorld : World
    {
        public static World WorldCreator() { return new GameWorld(); }

        private OrthographicCameraNode m_camera = new OrthographicCameraNode("TestCamera");
        private CameraController m_cameraController = null;

        private Bin mBin = new Bin();
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
        private AiEntityManager mAiEntityManager = null;
        private OsdManager m_osdManager = null;
        private TriggerManager m_triggerManager = null;
        //private EventManager m_eventManager = null;
        //private SpawnPointManager m_spawnGroupManager = null;
        private CorpseManager m_corpseManager = null;
        private PathRouteManager m_pathRouteManager = null;
        //private PressurePlateManager m_pressurePlateManager = null;

        private TextBatchPrinter m_textPrinter = null;

        private PathIsland m_pathIsland = new PathIsland();

        private int m_updateTokenOffset = 0;

        private float m_elapsedTime = 0.0f;

#if !NO_SOUND
        private AudioEngine m_audioEngine = null;
        private WaveBank m_waveBank = null;
        private SoundBank m_soundBank = null;
#endif

        public OrthographicCameraNode Camera                { get { return m_camera; } }

        public Bin Bin                                      { get { return mBin; } }

        public PlayerManager PlayerManager                  { get { return m_playerManager; } }
        public WeaponManager WeaponManager                  { get { return m_weaponManager; } }
        public ProjectileManager ProjectileManager          { get { return m_projectileManager; } }
        public AiEntityManager EnemyManager                 { get { return mAiEntityManager; } }
        public TriggerManager TriggerManager                { get { return m_triggerManager; } }
        //public EventManager EventManager                  { get { return m_eventManager; } }
        //public SpawnPointManager SpawnPointManager        { get { return m_spawnGroupManager; } }
        public CorpseManager CorpseManager                  { get { return m_corpseManager; } }
        public PathRouteManager PathRouteManager            { get { return m_pathRouteManager; } }
        //public PressurePlateManager PressurePlateManager  { get { return m_pressurePlateManager; } }

        public TextBatchPrinter TextPrinter                 { get { return m_textPrinter; } }
        public Random Random                                { get { return m_random; } }
        public DebugRenderer DebugRenderer                  { get { return m_debugRenderer; } }

        public MapData.Map Map                              { get { return m_map; } }

        public float ElapsedTime                            { get { return m_elapsedTime; } }

#if !NO_SOUND
        private AudioEngine AudioEngine                     { get { return m_audioEngine; } }
        private WaveBank WaveBank                           { get { return m_waveBank; } }
        private SoundBank SoundBank                         { get { return m_soundBank; } }
#endif

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public GameWorld()
        {
            m_cameraController = new CameraController(this);
            m_weaponManager = new WeaponManager(this);
            m_projectileManager = new ProjectileManager(this, mBin);
            mAiEntityManager = new AiEntityManager(this, mBin);
            m_osdManager = new OsdManager(this);
            m_playerManager = new PlayerManager(this, mBin);
            m_triggerManager = new TriggerManager();
            //m_eventManager = new EventManager(this);
            //m_spawnGroupManager = new SpawnPointManager(this);
            m_corpseManager = new CorpseManager(this, mBin);
            m_pathRouteManager = new PathRouteManager();
            m_textPrinter = new TextBatchPrinter();
            //m_pressurePlateManager = new PressurePlateManager(this);
        }


        public override void Load()
        {
#if !NO_SOUND
            m_audioEngine = new AudioEngine("Content\\Audio\\audio.xgs");
            m_waveBank = new WaveBank(m_audioEngine, "Content\\Audio\\Wave Bank.xwb");
            m_soundBank = new SoundBank(m_audioEngine, "Content\\Audio\\Sound Bank.xsb");
#endif

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

            m_map = TestGame.Instance().ResourceManager.Get<MapData.Map>("maps/test_arena2/test_arena2");
            //m_map = TestGame.Instance().Content.Load<MapData.Map>("maps/tom_owes_me/tom_owes_me");

            mBin.Init(50, 50, m_map.PlayAreaMax + new Vector2(1000.0f, 1000.0f), BinLayers.kLayerCount, 6000, 1000, 1000);


            // ----------------------------------------------------------------
            // -- Init the pools
            // ----------------------------------------------------------------

            m_playerManager.Load();
            m_weaponManager.Load();
            m_projectileManager.Load();
            mAiEntityManager.Load();
            m_corpseManager.Load();


            m_playerManager.AddPlayer(TestGame.Instance().InputManager.GetInputWrapper(0));


            // TODO: Temp: Disconnects need to be handled in the InputManager
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.Two);
            if (gamePadState.IsConnected)
            {
                m_playerManager.AddPlayer(TestGame.Instance().InputManager.GetInputWrapper(1));
            }


            m_mapRenderer.Init(m_map, TestGame.Instance().GraphicsDevice, 16);


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
            m_pathIsland.SetRegions(regions);

            AddPathIslandToBin(m_pathIsland);

            m_pathRouteManager.Init(1000, 100, 200);

            CollisionHelpers.Init();
            PathFindingHelpers.Init(400.0f, 3, mBin);

            //m_eventManager.LoadEvents(m_map);
            //m_spawnGroupManager.LoadSpawnGroups(m_map);
            //m_pressurePlateManager.LoadPressurePoints(m_map);

            Random rand = new Random(101);
            for (int i = 0; i < 100; ++i)
            {
                float x = 1840.0f + ((float)rand.NextDouble() * 420.0f);
                float y = 3880.0f + ((float)rand.NextDouble() * 700.0f);
                mAiEntityManager.Create(typeof(Civilian), new Vector2(x, y));
            }

            for (int i = 0; i < 3; ++i)
            {
                float x = 1840.0f + ((float)rand.NextDouble() * 420.0f);
                float y = 3880.0f + ((float)rand.NextDouble() * 500.0f);
                mAiEntityManager.Create(typeof(Zombie), new Vector2(x, y));
            }
        }


        public override void Enter()
        {
            m_triggerManager.GetTrigger(TriggerManager.kDefaultTriggerName).Activate();
        }


        public override void Update(float dt)
        {
            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime(dt);

            m_elapsedTime += frameTime.Dt;

            ++m_updateTokenOffset;

            Input.InputWrapper inputWrapper = TestGame.Instance().InputManager.GetInputWrapper(0);

            m_playerManager.Update(ref frameTime);
            mAiEntityManager.Update(ref frameTime, m_updateTokenOffset);
            m_projectileManager.Update(ref frameTime);
            m_osdManager.Update(ref frameTime);

            m_weaponManager.Update(ref frameTime);

            m_pathRouteManager.Update(ref frameTime);

            //m_eventManager.Update(ref frameTime);
            m_corpseManager.Update(ref frameTime);


            // Collision detection/resolution
            m_contactList.StartAddingContacts();

            GenerateContacts();

            m_contactList.EndAddingContacts();

            m_contactResolver.ResolveContacts(frameTime.Dt, m_contactList);



            // Move the camera follow position
            // Update camera after collision resolution so its go the actual rendered player position.
            m_playerManager.UpdateAveragePosition();
            m_cameraController.FollowPosition = m_playerManager.AveragePlayerPosition;
            m_cameraController.Update(ref frameTime, ref inputWrapper);


            // End frame updates
            m_projectileManager.EndFrame();
        }


        static readonly int[] kProjectileEntityLayers = { BinLayers.kPlayerEntity, BinLayers.kEnemyEntities, BinLayers.kCivilianEntities };

        private void GenerateContacts()
        {
            // Check groups against each other
            CollisionHelpers.GenerateEntityContacts(m_playerManager.Players.ActiveItemList, m_playerManager.Players.ActiveItemListCount, 1.0f, mBin, BinLayers.kPlayerEntity, m_contactList);
            CollisionHelpers.GenerateEntityContacts(mAiEntityManager.Entities.ActiveItemList, mAiEntityManager.Entities.ActiveItemListCount, 0.9f, mBin, BinLayers.kEnemyEntities, m_contactList);
            CollisionHelpers.GenerateEntityContacts(mAiEntityManager.Entities.ActiveItemList, mAiEntityManager.Entities.ActiveItemListCount, 0.9f, mBin, BinLayers.kCivilianEntities, m_contactList);

            // Players against enemies
            CollisionHelpers.GenerateEntityContacts(m_playerManager.Players.ActiveItemList, m_playerManager.Players.ActiveItemListCount, 0.7f, mBin, BinLayers.kEnemyEntities, m_contactList);
            CollisionHelpers.GenerateEntityContacts(m_playerManager.Players.ActiveItemList, m_playerManager.Players.ActiveItemListCount, 0.7f, mBin, BinLayers.kCivilianEntities, m_contactList);

            // Check against boundaries
            CollisionHelpers.GenerateBoundaryContacts(m_playerManager.Players.ActiveItemList, m_playerManager.Players.ActiveItemListCount, mBin, BinLayers.kBoundary, m_contactList);
            CollisionHelpers.GenerateBoundaryContacts(mAiEntityManager.Entities.ActiveItemList, mAiEntityManager.Entities.ActiveItemListCount, mBin, BinLayers.kBoundary, m_contactList);


            // Check projectiles
            CollisionHelpers.GenerateProjectileContacts(m_projectileManager.Bullets.ActiveItemList, m_projectileManager.Bullets.ActiveItemListCount, mBin, kProjectileEntityLayers, BinLayers.kBoundary);
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
            for (int i = 0; i < m_boundaries.Count; ++i)
                m_boundaries[i].DebugRender(m_debugRenderer);

            //m_pressurePlateManager.DebugRender(m_debugRenderer);

            m_corpseManager.DebugRender(m_debugRenderer);

            m_playerManager.DebugRender(m_debugRenderer);

            mAiEntityManager.DebugRender(m_debugRenderer);
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
                        m_boundaries.Add(boundaryEntity);

                    lastPoint = pos;
                }
            }

            return extrudedCollisionBoundary;
        }


        private Vector2[][] ExtrudeCollisionBoundaries(float extrudeAmount, bool roundedCorners)
        {
            int numBoundries = m_map.CollisionBoundaries.Length;
            Vector2[][] extrudedCollisionBoundary = new Vector2[numBoundries][];

            for (int i = 0; i < numBoundries; ++i)
            {
                if(roundedCorners)
                    Momo.Maths.ExtendedMaths2D.ExtrudePointsAlongNormalRounded(m_map.CollisionBoundaries[i], m_map.CollisionBoundaries[i].Length, extrudeAmount, 0.55f, out extrudedCollisionBoundary[i]);
                else
                    Momo.Maths.ExtendedMaths2D.ExtrudePointsAlongNormal(m_map.CollisionBoundaries[i], m_map.CollisionBoundaries[i].Length, extrudeAmount, out extrudedCollisionBoundary[i]);
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

        public void PlaySoundQueue(string name)
        {
#if !NO_SOUND
            SoundBank.PlayCue(name);
#endif
        }
    }
}
