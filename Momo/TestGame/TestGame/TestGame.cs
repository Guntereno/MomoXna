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
using Momo.Debug;

using TestGame.Systems;
using TestGame.Entities;



namespace TestGame
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class TestGame : Microsoft.Xna.Framework.Game
	{
		const int kBackBufferWidth = 1200;
		const int kBackBufferHeight = 900;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		WorldManager.WorldManager m_worldManager = new WorldManager.WorldManager();
		DebugRenderer m_debugRenderer = new DebugRenderer();
		OrthographicCameraNode m_camera = new OrthographicCameraNode("TestCamera");
		CameraController m_cameraController = new CameraController();

		Bin m_bin = new Bin(50, 20, 5000, 2000, 5000, 5000);
        ContactList m_contactList = new ContactList(4000);
        ContactResolver m_contactResolver = new ContactResolver();

		Map.Map m_map = null;
		MapRenderer m_mapRenderer = new MapRenderer();

        List<AiEntity> m_ais = new List<AiEntity>(2000);
        List<BoundaryEntity> m_boundaries = new List<BoundaryEntity>(2000);


		public TestGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = kBackBufferWidth;
			graphics.PreferredBackBufferHeight = kBackBufferHeight;

			m_camera.ViewWidth = kBackBufferWidth;
			m_camera.ViewHeight = kBackBufferHeight;
			m_camera.LocalTranslation = new Vector3(0.0f, 0.0f, 10.0f);

			m_cameraController.Camera = m_camera;

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

			m_map = Content.Load<Map.Map>("maps/1_living_quarters/1_living_quarters");


            Random random = new Random();
            for (int i = 0; i < 100; ++i)
            {
                AiEntity ai = new AiEntity();
                Vector2 pos = new Vector2(100.0f + ((float)random.NextDouble() * 3300.0f),
                                            725.0f + ((float)random.NextDouble() * 65.0f));


                ai.SetPosition(pos);

                ai.AddToBin(m_bin);

                m_ais.Add(ai);
            }


			BuildCollisionBoundaries();

			m_mapRenderer.Init(m_map, GraphicsDevice, 16);
		}

		private void BuildCollisionBoundaries()
		{
			int numBoundries = m_map.CollisionBoundaries.Length;

			for (int boundryIdx = 0; boundryIdx < numBoundries; ++boundryIdx)
			{
				int numNodes = m_map.CollisionBoundaries[boundryIdx].Length;
				LineStripPrimitive2D lineStrip = new LineStripPrimitive2D(numNodes);
				lineStrip.StartAddingPoints();
				for (int nodeIdx = 0; nodeIdx < numNodes; ++nodeIdx)
				{
					Vector2 pos = new Vector2(
						(float)(m_map.CollisionBoundaries[boundryIdx][nodeIdx].X),
						(float)(m_map.CollisionBoundaries[boundryIdx][nodeIdx].Y)
					);
					lineStrip.AddPoint(pos);
				}
				lineStrip.EndAddingPoints();
                BoundaryEntity boundaryEntity = new BoundaryEntity(lineStrip);
                boundaryEntity.RecalculateBinRegion(m_bin);
                m_boundaries.Add(boundaryEntity);
			}
		}


		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
			m_worldManager.FlushWorld("TestWorld");
		}


		protected override void Update(GameTime gameTime)
		{
			GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
			KeyboardState keyboardState = Keyboard.GetState();

			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime((float)gameTime.ElapsedGameTime.TotalSeconds);


            m_worldManager.Update(frameTime.Dt);

            for (int i = 0; i < m_ais.Count; ++i)
            {
                m_ais[i].Update(ref frameTime);
                m_ais[i].UpdateBinEntry(m_bin);
            }

            m_contactList.StartAddingContacts();

            GameHelpers.GenerateContacts(m_ais, m_bin, m_contactList);
            GameHelpers.GenerateContacts(m_ais, m_boundaries, m_bin, m_contactList);

            m_contactList.EndAddingContacts();

            m_contactResolver.ResolveContacts(frameTime.Dt, m_contactList);


			m_cameraController.Update(gamePadState, keyboardState);

			base.Update(gameTime);
		}


		protected override void Draw(GameTime gameTime)
		{
			m_camera.PreRenderUpdate();

			GraphicsDevice.Clear(Color.SteelBlue);

			m_debugRenderer.Clear();

			m_worldManager.Render();

			// 3D
			//m_debugRenderer.DrawCircle(new Vector3(0.0f, 0.0f, 0.0f), 30.0f, Color.Red, Color.Black, 10.0f, new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 64);

			//// 2D
			//float y = 250.0f;
			//float x = 230.0f;
			//m_debugRenderer.DrawCircle(new Vector2(x + 0.0f, y), 30.0f, Color.Red, Color.Black, true, 0.0f, 64);
			//m_debugRenderer.DrawCircle(new Vector2(x + 60.0f, y), 30.0f, Color.Red, Color.Black, true, 10.0f, 64);
			//m_debugRenderer.DrawCircle(new Vector2(x + 120.0f, y), 30.0f, Color.Red, Color.Black, true, 20.0f, 64);

			//y -= 60;
			//m_debugRenderer.DrawOutlineCircle(new Vector2(x + 0.0f, y), 30.0f, Color.Black, 0.0f);
			//m_debugRenderer.DrawOutlineCircle(new Vector2(x + 60.0f, y), 30.0f, Color.Black, 10.0f);
			//m_debugRenderer.DrawOutlineCircle(new Vector2(x + 120.0f, y), 30.0f, Color.Black, 20.0f);

			//y -= 60;
			//m_debugRenderer.DrawFilledCircle(new Vector2(x + 0.0f, y), 30.0f, Color.Red);
			//m_debugRenderer.DrawFilledCircle(new Vector2(x + 60.0f, y), 30.0f, Color.Red);
			//m_debugRenderer.DrawFilledCircle(new Vector2(x + 120.0f, y), 30.0f, Color.Red);


			//// 2D
			//Vector2 p1 = new Vector2(-300.0f, 30.0f);
			//Vector2 p2 = new Vector2(-200.0f, 0.0f);
			//Vector2 p3 = new Vector2(-220.0f, -30.0f);
			//Vector2 p4 = new Vector2(-330.0f, -50.0f);

			//Vector2 offset = new Vector2(0.0f, -70.0f);
			//m_debugRenderer.DrawQuad(offset + p1, offset + p2, offset + p3, offset + p4, Color.Red, Color.Black, true, 30.0f);
			//offset = new Vector2(150.0f, -70.0f);
			//m_debugRenderer.DrawQuad(offset + p1, offset + p2, offset + p3, offset + p4, Color.Red, Color.Black, true, 10.0f);


			//p1 = new Vector2(-300.0f, -100.0f);
			//p2 = new Vector2(-180.0f, -100.0f);
			//p3 = new Vector2(-180.0f, -200.0f);
			//p4 = new Vector2(-300.0f, -200.0f);

			//offset = new Vector2(-20.0f, -50.0f);
			//m_debugRenderer.DrawQuad(offset + p1, offset + p2, offset + p3, offset + p4, Color.Red, Color.Black, true, 30.0f);
			//offset = new Vector2(120.0f, -50.0f);
			//m_debugRenderer.DrawQuad(offset + p1, offset + p2, offset + p3, offset + p4, Color.Red, Color.Black, true, 10.0f);


			//y = 270.0f;
			//m_debugRenderer.DrawFilledLineWithCaps(new Vector2(-350.0f, y - 0.0f), new Vector2(0.0f, y - 0.0f), Color.Black, 13.0f, 4);
			//m_debugRenderer.DrawFilledLineWithCaps(new Vector2(-350.0f, y - 30.0f), new Vector2(0.0f, y - 30.0f), Color.Black, 23.0f, 5);
			//m_debugRenderer.DrawFilledLineWithCaps(new Vector2(-350.0f, y - 85.0f), new Vector2(0.0f, y - 85.0f), Color.Black, 51.0f, 32);

			//y -= 140.0f;
			//m_debugRenderer.DrawFilledLine(new Vector2(-350.0f, y - 0.0f), new Vector2(0.0f, y - 0.0f), Color.Black, 13.0f);
			//m_debugRenderer.DrawFilledLine(new Vector2(-350.0f, y - 30.0f), new Vector2(0.0f, y - 30.0f), Color.Black, 23.0f);
			//m_debugRenderer.DrawFilledLine(new Vector2(-350.0f, y - 85.0f), new Vector2(0.0f, y - 85.0f), Color.Black, 51.0f);


			m_bin.DebugRender(m_debugRenderer, 6);

            for (int i = 0; i < m_boundaries.Count; ++i)
			{
                m_boundaries[i].DebugRender(m_debugRenderer);
			}

            for (int i = 0; i < m_ais.Count; ++i)
            {
                m_ais[i].DebugRender(m_debugRenderer);
            }

			m_mapRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, GraphicsDevice);
			m_debugRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, GraphicsDevice);

			base.Draw(gameTime);
		}
	}
}
