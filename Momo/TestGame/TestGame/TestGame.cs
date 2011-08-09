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
using Momo.Debug;

using TestGame.Systems;
using TestGame.Entities;
using TestGame.Objects;



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
        
        static Random ms_random = new Random();

		WorldManager.WorldManager m_worldManager = new WorldManager.WorldManager();
		DebugRenderer m_debugRenderer = new DebugRenderer();
		OrthographicCameraNode m_camera = new OrthographicCameraNode("TestCamera");
		CameraController m_cameraController = new CameraController();

		Bin m_bin = new Bin(100, 100, 10000, 10000, 3, 10000, 5000, 1000);
        ContactList m_contactList = new ContactList(4000);
        ContactResolver m_contactResolver = new ContactResolver();

		Map.Map m_map = null;
		MapRenderer m_mapRenderer = new MapRenderer();

        PathIsland m_pathIsland = new PathIsland();

        List<AiEntity> m_ais = new List<AiEntity>(2000);
        List<BoundaryEntity> m_boundaries = new List<BoundaryEntity>(2000);
        List<BulletEntity> m_bullets = new List<BulletEntity>(2000);
        List<Explosion> m_explosions = new List<Explosion>(100);

		PlayerEntity m_player = new PlayerEntity();

		public TestGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = kBackBufferWidth;
			graphics.PreferredBackBufferHeight = kBackBufferHeight;

			m_camera.ViewWidth = kBackBufferWidth;
			m_camera.ViewHeight = kBackBufferHeight;
			m_camera.LocalTranslation = new Vector3(300.0f, 750.0f, 10.0f);

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


            for (int i = 0; i < 200; ++i)
            {
                AiEntity ai = new AiEntity();
                Vector2 pos = new Vector2(150.0f + ((float)ms_random.NextDouble() * 3300.0f),
                                            725.0f + ((float)ms_random.NextDouble() * 65.0f));

                ai.SetPosition(pos);

                ai.AddToBin(m_bin);

                m_ais.Add(ai);
            }

			m_player.SetPosition(new Vector2(416.0f, 320.0f));

            PathRegion[] regions = new PathRegion[2];
            regions[0] = new PathRegion(new Vector2(100.0f, 650.0f), new Vector2(900.0f, 800.0f));
            regions[1] = new PathRegion(new Vector2(200.0f, 150.0f), new Vector2(700.0f, 550.0f));

            regions[0].GenerateUniformGridOfNodes(10.0f, 50.0f);
            regions[1].GenerateUniformGridOfNodes(10.0f, 50.0f);

            m_pathIsland.SetRegions(regions);



			m_cameraController.TargetEntity = m_player;

			BuildCollisionBoundaries();

			m_mapRenderer.Init(m_map, GraphicsDevice, 16);
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

            m_explosions.Clear();

            m_worldManager.Update(frameTime.Dt);


            if (ms_random.NextDouble() < 0.1f)
            {
                BulletEntity bullet = new BulletEntity();
                bullet.SetPosition(new Vector2(100.0f, 750.0f));

                Vector2 velocity = new Vector2(1.0f, ((float)ms_random.NextDouble() - 0.5f) * 0.25f);
                velocity.Normalize();
                bullet.SetVelocity(velocity * 750.0f);

                bullet.AddToBin(m_bin);
                m_bullets.Add(bullet);
            }

            // Works for the debug rendering is hard on the eye.
			if (ms_random.NextDouble() < 0.02f)
			{
				Explosion explosion = new Explosion(new Vector2(350.0f, 750.0f), 150.0f, 25000.0f);
				m_explosions.Add(explosion);
			}


            for (int i = 0; i < m_ais.Count; ++i)
            {
                m_ais[i].Update(ref frameTime);
                m_ais[i].UpdateBinEntry(m_bin);
            }

            for (int i = 0; i < m_bullets.Count; ++i)
            {
                m_bullets[i].Update(ref frameTime);
                m_bullets[i].UpdateBinEntry(m_bin);
            }


			m_player.UpdateInput(ref gamePadState, ref keyboardState);
			m_player.Update(ref frameTime);
            m_player.UpdateBinEntry(m_bin);

            m_contactList.StartAddingContacts();

            CollisionHelpers.GenerateContacts(m_ais, m_bin, m_contactList);
            CollisionHelpers.GenerateContacts(m_ais, m_boundaries, m_bin, m_contactList);
            CollisionHelpers.GenerateContacts(m_player, m_bin, m_contactList);
            CollisionHelpers.GenerateContacts(m_player, m_boundaries, m_bin, m_contactList);
            CollisionHelpers.UpdateBulletContacts(m_ais, m_bullets, m_bin);
            CollisionHelpers.UpdateBulletContacts(m_bullets, m_boundaries, m_bin);
            CollisionHelpers.UpdateExplosions(m_ais, m_explosions, m_bin);

            m_contactList.EndAddingContacts();

            m_contactResolver.ResolveContacts(frameTime.Dt, m_contactList);

            // Destroying dead entities/objects
            for (int i = 0; i < m_bullets.Count; ++i)
            {
                if (m_bullets[i].NeedsDestroying())
                {
                    m_bullets[i].RemoveFromBin(m_bin);
                    m_bullets.RemoveAt(i);
                    --i;
                }
            }


			m_cameraController.Update(gamePadState, keyboardState);

			base.Update(gameTime);
		}


		protected override void Draw(GameTime gameTime)
		{
			m_camera.PreRenderUpdate();

			GraphicsDevice.Clear(Color.SteelBlue);

			m_debugRenderer.Clear();

			m_worldManager.Render();


            for (int i = 0; i < m_boundaries.Count; ++i)
			{
                m_boundaries[i].DebugRender(m_debugRenderer);
			}

            for (int i = 0; i < m_ais.Count; ++i)
            {
                m_ais[i].DebugRender(m_debugRenderer);
            }

            for (int i = 0; i < m_bullets.Count; ++i)
            {
                m_bullets[i].DebugRender(m_debugRenderer);
            }

            for (int i = 0; i < m_explosions.Count; ++i)
            {
                m_explosions[i].DebugRender(m_debugRenderer);
            }

            //m_pathIsland.DebugRender(m_debugRenderer);


			m_player.DebugRender(m_debugRenderer);

			m_mapRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, GraphicsDevice);
			m_debugRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, GraphicsDevice);

			base.Draw(gameTime);
		}
	}
}
