using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using WorldManager;
using Momo.Debug;

using Momo.Core.Nodes.Cameras;
using Momo.Core.Spatial;



namespace TestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TestGame : Microsoft.Xna.Framework.Game
    {
        const int kBackBufferWidth = 1000;
        const int kBackBufferHeight = 800;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        WorldManager.WorldManager m_worldManager = new WorldManager.WorldManager();
        DebugRenderer m_debugRenderer = new DebugRenderer();
        OrthographicCameraNode m_camera = new OrthographicCameraNode("TestCamera");

        Bin m_bin = new Bin(25, 25, 1000, 1000, 100);



        public TestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = kBackBufferWidth;
            graphics.PreferredBackBufferHeight = kBackBufferHeight;

            m_camera.ViewWidth = kBackBufferWidth;
            m_camera.ViewHeight = kBackBufferHeight;

            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            m_debugRenderer.Init(50000, 1000, GraphicsDevice);

            m_worldManager.RegisterWorld("TestWorld", TestWorld.WorldCreator);

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            m_worldManager.LoadWorld("TestWorld");
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            m_worldManager.FlushWorld("TestWorld");
        }


        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            m_worldManager.Update(gameTime.ElapsedGameTime.Seconds);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            m_camera.LocalTranslation = new Vector3(0.0f, 0.0f, 10.0f);
            m_camera.PreRenderUpdate();

            GraphicsDevice.Clear(Color.OldLace);

            m_debugRenderer.Clear();

            m_worldManager.Render();

            // 3D
            //m_debugRenderer.DrawCircle(new Vector3(0.0f, 0.0f, 0.0f), 30.0f, Color.Red, Color.Black, 10.0f, new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 64);

            // 2D
            float y = 250.0f;
            float x = 230.0f;
            m_debugRenderer.DrawCircle(new Vector2(x + 0.0f, y), 30.0f, Color.Red, Color.Black, true, 0.0f, 64);
            m_debugRenderer.DrawCircle(new Vector2(x + 60.0f, y), 30.0f, Color.Red, Color.Black, true, 10.0f, 64);
            m_debugRenderer.DrawCircle(new Vector2(x + 120.0f, y), 30.0f, Color.Red, Color.Black, true, 20.0f, 64);

            y -= 60;
            m_debugRenderer.DrawOutlineCircle(new Vector2(x + 0.0f, y), 30.0f, Color.Black, 0.0f);
            m_debugRenderer.DrawOutlineCircle(new Vector2(x + 60.0f, y), 30.0f, Color.Black, 10.0f);
            m_debugRenderer.DrawOutlineCircle(new Vector2(x + 120.0f, y), 30.0f, Color.Black, 20.0f);

            y -= 60;
            m_debugRenderer.DrawFilledCircle(new Vector2(x + 0.0f, y), 30.0f, Color.Red);
            m_debugRenderer.DrawFilledCircle(new Vector2(x + 60.0f, y), 30.0f, Color.Red);
            m_debugRenderer.DrawFilledCircle(new Vector2(x + 120.0f, y), 30.0f, Color.Red);


            // 2D
            Vector2 p1 = new Vector2(-300.0f, 30.0f);
            Vector2 p2 = new Vector2(-200.0f, 0.0f);
            Vector2 p3 = new Vector2(-220.0f, -30.0f);
            Vector2 p4 = new Vector2(-330.0f, -50.0f);

            Vector2 offset = new Vector2(0.0f, -70.0f);
            m_debugRenderer.DrawQuad(offset + p1, offset + p2, offset + p3, offset + p4, Color.Red, Color.Black, true, 30.0f);
            offset = new Vector2(150.0f, -70.0f);
            m_debugRenderer.DrawQuad(offset + p1, offset + p2, offset + p3, offset + p4, Color.Red, Color.Black, true, 10.0f);


            p1 = new Vector2(-300.0f, -100.0f);
            p2 = new Vector2(-180.0f, -100.0f);
            p3 = new Vector2(-180.0f, -200.0f);
            p4 = new Vector2(-300.0f, -200.0f);

            offset = new Vector2(-20.0f, -50.0f);
            m_debugRenderer.DrawQuad(offset + p1, offset + p2, offset + p3, offset + p4, Color.Red, Color.Black, true, 30.0f);
            offset = new Vector2(120.0f, -50.0f);
            m_debugRenderer.DrawQuad(offset + p1, offset + p2, offset + p3, offset + p4, Color.Red, Color.Black, true, 10.0f);


            y = 270.0f;
            m_debugRenderer.DrawFilledLineWithCaps(new Vector2(-350.0f, y - 0.0f), new Vector2(0.0f, y - 0.0f), Color.Black, 13.0f, 4);
            m_debugRenderer.DrawFilledLineWithCaps(new Vector2(-350.0f, y - 30.0f), new Vector2(0.0f, y - 30.0f), Color.Black, 23.0f, 5);
            m_debugRenderer.DrawFilledLineWithCaps(new Vector2(-350.0f, y - 85.0f), new Vector2(0.0f, y - 85.0f), Color.Black, 51.0f, 32);

            y -= 140.0f;
            m_debugRenderer.DrawFilledLine(new Vector2(-350.0f, y - 0.0f), new Vector2(0.0f, y - 0.0f), Color.Black, 13.0f);
            m_debugRenderer.DrawFilledLine(new Vector2(-350.0f, y - 30.0f), new Vector2(0.0f, y - 30.0f), Color.Black, 23.0f);
            m_debugRenderer.DrawFilledLine(new Vector2(-350.0f, y - 85.0f), new Vector2(0.0f, y - 85.0f), Color.Black, 51.0f);


            m_bin.DebugRender(m_debugRenderer, 6);


            m_debugRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
