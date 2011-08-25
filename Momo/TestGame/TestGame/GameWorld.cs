using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

using Fonts;



namespace TestGame
{
    public class GameWorld : World
    {
        public static World WorldCreator() { return new GameWorld(); }


        OrthographicCameraNode m_camera = new OrthographicCameraNode("TestCamera");
        CameraController m_cameraController = new CameraController();

        Bin m_bin = new Bin(100, 100, 10000, 10000, 3, 10000, 5000, 1000);
        ContactList m_contactList = new ContactList(4000);
        ContactResolver m_contactResolver = new ContactResolver();

        Map.Map m_map = null;
        MapRenderer m_mapRenderer = new MapRenderer();

        Pool<PlayerEntity> m_players = new Pool<PlayerEntity>(4);
        Pool<AiEntity> m_ais = new Pool<AiEntity>(2000);

        List<BoundaryEntity> m_boundaries = new List<BoundaryEntity>(2000);
        //List<Explosion> m_explosions = new List<Explosion>(100);


        static Random ms_random = new Random();
        DebugRenderer m_debugRenderer = new DebugRenderer();

        Systems.WeaponManager m_weaponManager = null;
        Systems.ProjectileManager m_projectileManager = null;

        TextBatchPrinter m_textPrinter = new TextBatchPrinter();
        Font m_debugFont = null;
        TextObject m_text1 = null;
        TextObject m_text2 = null;
        List<TextObject> m_textList = new List<TextObject>(10);


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public GameWorld()
        {
            m_weaponManager = new Systems.WeaponManager(this);
            m_projectileManager = new Systems.ProjectileManager(this, m_bin);
        }


        public Systems.WeaponManager GetWeaponManager()         { return m_weaponManager; }
        public Systems.ProjectileManager GetProjectileManager() { return m_projectileManager; }
        public TextBatchPrinter GetTextPrinter()                { return m_textPrinter; }
        public Font GetDebugFont()                              { return m_debugFont; }
        public Random GetRandom()                               { return ms_random; }


        public override void Load()
        {
            m_debugRenderer.Init(50000, 1000, TestGame.Instance().GraphicsDevice);

            Effect textEffect = TestGame.Instance().Content.Load<Effect>("effects/text");
            m_textPrinter.Init(textEffect, new Vector2((float)TestGame.kBackBufferWidth, (float)TestGame.kBackBufferHeight));
            m_debugFont = TestGame.Instance().Content.Load<Font>("fonts/Calibri_24_b_o4");

            m_text1 = new TextObject("Hello1", m_debugFont, 500, 10, 1);
            m_text1.Position = new Vector2(100.0f, 100.0f);

            m_text2 = new TextObject("Hello2", m_debugFont, 500, 10, 1);
            m_text2.Position = new Vector2(200.0f, 100.0f);

            m_textList.Add(m_text1);
            m_textList.Add(m_text2);


            m_camera.ViewWidth = TestGame.kBackBufferWidth;
            m_camera.ViewHeight = TestGame.kBackBufferHeight;
            m_camera.LocalTranslation = new Vector3(300.0f, 750.0f, 10.0f);

            m_cameraController.Camera = m_camera;

            m_map = TestGame.Instance().Content.Load<Map.Map>("maps/1_living_quarters/1_living_quarters");


            // ----------------------------------------------------------------
            // -- Init the pools
            // ----------------------------------------------------------------
            for (int i = 0; i < 1; ++i)
            {
                PlayerEntity player = new PlayerEntity(this);
                player.AddToBin(m_bin);
                m_players.AddItem(player, true);
            }

            for (int i = 0; i < 200; ++i)
            {
                AiEntity ai = new AiEntity(this);
                Vector2 pos = new Vector2(150.0f + ((float)ms_random.NextDouble() * 3300.0f),
                                            725.0f + ((float)ms_random.NextDouble() * 65.0f));

                ai.SetPosition(pos);

                ai.AddToBin(m_bin);

                m_ais.AddItem(ai, true);
            }

            m_weaponManager.Load();
            m_projectileManager.Load();

            m_players[0].SetPosition(new Vector2(416.0f, 320.0f));
            m_players[0].Init();

            m_cameraController.TargetEntity = m_players[0];

            BuildCollisionBoundaries();

            m_mapRenderer.Init(m_map, TestGame.Instance().GraphicsDevice, 16);
        }


        public override void Enter()
        {

        }


        public override void Update(float dt)
        {
            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime(dt);

            Input.InputWrapper inputWrapper = TestGame.Instance().InputWrapper;


            //m_explosions.Clear();


            // Works for the debug rendering is hard on the eye.
            //if (ms_random.NextDouble() < 0.02f)
            //{
            //    Explosion explosion = new Explosion(new Vector2(350.0f, 750.0f), 150.0f, 25000.0f);
            //    m_explosions.Add(explosion);
            //}


            for (int i = 0; i < m_players.ActiveItemListCount; ++i)
            {
                m_players[i].UpdateInput(ref inputWrapper);
                m_players[i].Update(ref frameTime);
                m_players[i].UpdateBinEntry();
            }

            for (int i = 0; i < m_ais.ActiveItemListCount; ++i)
            {
                m_ais[i].Update(ref frameTime);
                m_ais[i].UpdateBinEntry();
            }

            m_contactList.StartAddingContacts();

            CollisionHelpers.GenerateContacts(m_ais.ActiveItemList, m_ais.ActiveItemListCount, m_bin, m_contactList);
            CollisionHelpers.GenerateContacts(m_players.ActiveItemList, m_players.ActiveItemListCount, m_bin, m_contactList);

            m_projectileManager.Update(ref frameTime);

            CollisionHelpers.UpdateBulletContacts(m_projectileManager.GetBullets().ActiveItemList, m_projectileManager.GetBullets().ActiveItemListCount, m_bin);

            //CollisionHelpers.UpdateExplosions(m_explosions, m_bin);

            m_contactList.EndAddingContacts();

            m_contactResolver.ResolveContacts(frameTime.Dt, m_contactList);

            m_projectileManager.EndFrame();

            m_cameraController.Update(ref inputWrapper);
        }


        public override void Exit()
        {

        }


        public override void Flush()
        {

        }


        public override void PreRender()
        {
            m_debugRenderer.Clear();
            m_camera.PreRenderUpdate();
        }

        public override void Render()
        {
            m_mapRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, TestGame.Instance().GraphicsDevice);

            m_textPrinter.Render(m_textList, true, TestGame.Instance().GraphicsDevice);
        }

        public override void PostRender()
        {

        }

        public override void DebugRender()
        {
            for (int i = 0; i < m_boundaries.Count; ++i)
            {
                m_boundaries[i].DebugRender(m_debugRenderer);
            }

            for (int i = 0; i < m_players.ActiveItemListCount; ++i)
            {
                m_players[i].DebugRender(m_debugRenderer);
            }

            for (int i = 0; i < m_ais.ActiveItemListCount; ++i)
            {
                m_ais[i].DebugRender(m_debugRenderer);
            }

            m_projectileManager.DebugRender(m_debugRenderer);

            //for (int i = 0; i < m_explosions.Count; ++i)
            //{
            //    m_explosions[i].DebugRender(m_debugRenderer);
            //}

            //m_pathIsland.DebugRender(m_debugRenderer);
            //m_bin.DebugRender(m_debugRenderer, 10, 2);

            m_debugRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, TestGame.Instance().GraphicsDevice);
        }

        private void BuildCollisionBoundaries()
        {
            int numBoundries = m_map.CollisionBoundaries.Length;

            for (int boundryIdx = 0; boundryIdx < numBoundries; ++boundryIdx)
            {
                int numNodes = m_map.CollisionBoundaries[boundryIdx].Length;

                Vector2 lastPoint = new Vector2(
                                        (float)(m_map.CollisionBoundaries[boundryIdx][0].X),
                                        (float)(m_map.CollisionBoundaries[boundryIdx][0].Y));

                for (int nodeIdx = 1; nodeIdx < numNodes; ++nodeIdx)
                {
                    Vector2 pos = new Vector2(
                        (float)(m_map.CollisionBoundaries[boundryIdx][nodeIdx].X),
                        (float)(m_map.CollisionBoundaries[boundryIdx][nodeIdx].Y));


                    LinePrimitive2D lineStrip = new LinePrimitive2D(lastPoint, pos);
                    BoundaryEntity boundaryEntity = new BoundaryEntity(lineStrip);
                    boundaryEntity.AddToBin(m_bin);
                    m_boundaries.Add(boundaryEntity);

                    lastPoint = pos;
                }
            }
        }
    }
}
