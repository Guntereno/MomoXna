﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Momo.Maths;
using Momo.Core;
using Momo.Core.Graphics;
using Momo.Core.Nodes.Cameras;
using Momo.Core.Spatial;
using Momo.Core.Collision2D;
using Momo.Core.Primitive2D;
using Momo.Core.GameEntities;
using Momo.Core.Pathfinding;
using Momo.Core.ObjectPools;
using Momo.Debug;

using TestGame.Systems;
using TestGame.Entities;
using TestGame.Objects;

using WorldManager;

using Momo.Fonts;



namespace TestGame
{
    public class GameWorld : World
    {
        public static World WorldCreator() { return new GameWorld(); }



        OrthographicCameraNode m_camera = new OrthographicCameraNode("TestCamera");
        CameraController m_cameraController = null;

        Bin m_bin = new Bin();
        ContactList m_contactList = new ContactList(4000);
        ContactResolver m_contactResolver = new ContactResolver();

        Map.Map m_map = null;
        MapRenderer m_mapRenderer = new MapRenderer();

        List<BoundaryEntity> m_boundaries = new List<BoundaryEntity>(2000);

        Random m_random = new Random();
        DebugRenderer m_debugRenderer = new DebugRenderer();

        PlayerManager m_playerManager = null;
        WeaponManager m_weaponManager = null;
        ProjectileManager m_projectileManager = null;
        EnemyManager m_enemyManager = null;
        OsdManager m_osdManager = null;

        TextBatchPrinter m_textPrinter = new TextBatchPrinter();

        PathIsland m_pathIsland = new PathIsland();
        PathRoute m_pathRoute = new PathRoute();



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
        }


        public PlayerManager GetPlayerManager()                 { return m_playerManager; }
        public WeaponManager GetWeaponManager()                 { return m_weaponManager; }
        public ProjectileManager GetProjectileManager()         { return m_projectileManager; }
        public EnemyManager GetEnemyManager()                   { return m_enemyManager; }

        public TextBatchPrinter GetTextPrinter()                { return m_textPrinter; }
        public Random GetRandom()                               { return m_random; }
        public DebugRenderer GetDebugRenderer()                 { return m_debugRenderer; }

        public Map.Map GetMap()                                 { return m_map; }


        public override void Load()
        {
            m_debugRenderer.Init(50000, 1000, TestGame.Instance().GraphicsDevice);


            Effect textEffect = TestGame.Instance().Content.Load<Effect>("effects/text");
            m_textPrinter.Init(textEffect, new Vector2((float)TestGame.kBackBufferWidth, (float)TestGame.kBackBufferHeight), 100, 1000, 1);

            m_camera.ViewWidth = TestGame.kBackBufferWidth;
            m_camera.ViewHeight = TestGame.kBackBufferHeight;
            m_camera.LocalTranslation = new Vector3(300.0f, 750.0f, 10.0f);

            m_cameraController.Camera = m_camera;

            m_map = TestGame.Instance().Content.Load<Map.Map>("maps/test_arena/test_arena");


            m_bin.Init(50, 50, 5, new Vector2(2250.0f, 2250.0f), 4, 6000, 1000, 1000);


            // ----------------------------------------------------------------
            // -- Init the pools
            // ----------------------------------------------------------------

            m_playerManager.Load();

            // Create the enemies
            float mapWidth = (float)(m_map.Dimensions.X * m_map.TileDimensions.X);
            float mapHeight = (float)(m_map.Dimensions.Y* m_map.TileDimensions.Y);
            const float kSpawnBoxWidth = 1600.0f;
            const float kSpawnBoxHeight = 400.0f;
            float minX = (mapWidth - kSpawnBoxWidth) * 0.5f;
            float minY = (mapHeight - kSpawnBoxHeight) * 0.5f;
            for (int i = 0; i < 200; ++i)
            {
                Vector2 pos = new Vector2(minX + ((float)m_random.NextDouble() * kSpawnBoxWidth),
                                            minY + ((float)m_random.NextDouble() * kSpawnBoxHeight));
                AiEntity ai = m_enemyManager.Create(pos);
            }

            m_weaponManager.Load();
            m_projectileManager.Load();

            m_playerManager.AddPlayer(TestGame.Instance().InputManager.GetInputWrapper(0));

            // TODO: Temp: Disconnects need to be handled in the InputManager
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.Two);
            if (gamePadState.IsConnected)
            {
                m_playerManager.AddPlayer(TestGame.Instance().InputManager.GetInputWrapper(1));
            }

            BuildCollisionBoundaries();

            m_mapRenderer.Init(m_map, TestGame.Instance().GraphicsDevice, 16);


            PathRegion[] regions = new PathRegion[1];
            regions[0] = new PathRegion(new Vector2(75.0f, 75.0f), new Vector2(2000.0f, 2000.0f));
            regions[0].GenerateNodesFromBoundaries(15.0f, m_map.CollisionBoundaries);
            regions[0].GenerateNodePaths(10.0f, m_bin, BinLayers.kBoundary);

            m_pathIsland.SetRegions(regions);

            AddPathIslandToBin(m_pathIsland);


            CollisionHelpers.Init();
            PathFindingHelpers.Init(500.0f, 3, m_bin);
        }


        public override void Enter()
        {

        }


        public override void Update(float dt)
        {
            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime(dt);

            Input.InputWrapper inputWrapper = TestGame.Instance().InputManager.GetInputWrapper(0);

            m_playerManager.Update(ref frameTime);

            // Move the camera follow position
            m_cameraController.FollowPosition = m_playerManager.GetAveragePosition();

            m_enemyManager.Update(ref frameTime);
            m_projectileManager.Update(ref frameTime);
            m_osdManager.Update(ref frameTime);


            m_contactList.StartAddingContacts();
            CollisionHelpers.GenerateContacts(m_enemyManager.GetEnemies().ActiveItemList, m_enemyManager.GetEnemies().ActiveItemListCount, m_bin, m_contactList);
            CollisionHelpers.GenerateContacts(m_playerManager.GetPlayers().ActiveItemList, m_playerManager.GetPlayers().ActiveItemListCount, m_bin, m_contactList);
            CollisionHelpers.UpdateBulletContacts(m_projectileManager.GetBullets().ActiveItemList, m_projectileManager.GetBullets().ActiveItemListCount, m_bin);
            m_contactList.EndAddingContacts();

            m_contactResolver.ResolveContacts(frameTime.Dt, m_contactList);


            m_projectileManager.EndFrame();

            m_cameraController.Update(ref inputWrapper);


            PathFindingHelpers.CreatePath(m_playerManager.GetPlayers()[0].GetPosition(), new Vector2(1000.0f, 1000.0f), m_bin, ref m_pathRoute);
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
        }


        public override void PostRender()
        {
            m_textPrinter.ClearDrawList();
        }


        public override void DebugRender()
        {
            for (int i = 0; i < m_boundaries.Count; ++i)
            {
                m_boundaries[i].DebugRender(m_debugRenderer);
            }

            for (int i = 0; i < m_playerManager.GetPlayers().ActiveItemListCount; ++i)
            {
                m_playerManager.GetPlayers()[i].DebugRender(m_debugRenderer);
            }

            m_enemyManager.DebugRender(m_debugRenderer);
            m_projectileManager.DebugRender(m_debugRenderer);
            m_osdManager.DebugRender(m_debugRenderer);

            //m_pathIsland.DebugRender(m_debugRenderer);
            //m_pathRoute.DebugRender(m_debugRenderer);
            
            //m_bin.DebugRender(m_debugRenderer, 5, BinLayers.kAiEntity);
            //m_bin.DebugRender(m_debugRenderer, PathFindingHelpers.ms_circularSearchRegions[0], new Color(0.20f, 0.0f, 0.0f, 0.5f));
            //m_bin.DebugRender(m_debugRenderer, PathFindingHelpers.ms_circularSearchRegions[1], new Color(0.40f, 0.0f, 0.0f, 0.5f));
            //m_bin.DebugRenderGrid(m_debugRenderer, Color.Orange, Color.DarkRed);


            //BinLocation centre = new BinLocation(12, 20);
            //Vector2 centrePos = m_bin.GetCentrePositionOfBin(centre);
            //m_debugRenderer.DrawOutlineCircle(centrePos, 50.0f, Color.Red, 2.0f);
            //m_debugRenderer.DrawOutlineCircle(centrePos, 50.0f, Color.Red, 2.0f);

            m_debugRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, TestGame.Instance().GraphicsDevice);
            m_debugRenderer.Clear();
        }


        private void BuildCollisionBoundaries()
        {
            int numBoundries = m_map.CollisionBoundaries.Length;


            for (int boundaryIdx = 0; boundaryIdx < numBoundries; ++boundaryIdx)
            {
                int numNodes = m_map.CollisionBoundaries[boundaryIdx].Length;


                Vector2 lastPoint = m_map.CollisionBoundaries[boundaryIdx][0];
                                      

                for (int nodeIdx = 1; nodeIdx < numNodes; ++nodeIdx)
                {
                    Vector2 pos = m_map.CollisionBoundaries[boundaryIdx][nodeIdx];

                    LinePrimitive2D lineStrip = new LinePrimitive2D(lastPoint, pos);
                    BoundaryEntity boundaryEntity = new BoundaryEntity(lineStrip);
                    boundaryEntity.AddToBin(m_bin);
                    m_boundaries.Add(boundaryEntity);

                    lastPoint = pos;
                }
            }
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
