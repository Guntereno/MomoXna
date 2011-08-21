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



namespace TestGame
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class TestGame : Microsoft.Xna.Framework.Game
	{
		public static TestGame Instance() { return ms_instance; }
		private static TestGame ms_instance = null;

		const int kBackBufferWidth = 1200;
		const int kBackBufferHeight = 900;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
        
        static Random ms_random = new Random();

		Input.InputWrapper m_inputWrapper = new Input.InputWrapper();

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

        Pool<PlayerEntity> m_players = new Pool<PlayerEntity>(4);
        Pool<AiEntity> m_ais = new Pool<AiEntity>(2000);
        Pool<BulletEntity> m_bullets = new Pool<BulletEntity>(2000);

        List<BoundaryEntity> m_boundaries = new List<BoundaryEntity>(2000);
        List<Explosion> m_explosions = new List<Explosion>(100);





		public TestGame()
		{
			System.Diagnostics.Debug.Assert(ms_instance == null);
			ms_instance = this;

			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = kBackBufferWidth;
			graphics.PreferredBackBufferHeight = kBackBufferHeight;

			m_camera.ViewWidth = kBackBufferWidth;
			m_camera.ViewHeight = kBackBufferHeight;
			m_camera.LocalTranslation = new Vector3(300.0f, 750.0f, 10.0f);

			m_cameraController.Camera = m_camera;

			Content.RootDirectory = "Content";
		}

  
		public void AddBullet(Vector2 startPos, Vector2 velocity)
		{
            BulletEntity bullet = m_bullets.CreateItem();
			bullet.SetPosition(startPos);
			bullet.SetVelocity(velocity);

			bullet.AddToBin(m_bin);
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


            // ----------------------------------------------------------------
            // -- Init the pools
            // ----------------------------------------------------------------
            for (int i = 0; i < 1; ++i)
            {
                PlayerEntity player = new PlayerEntity();
                player.AddToBin(m_bin);
                m_players.AddItem(player, true);
            }

            for (int i = 0; i < 200; ++i)
            {
                AiEntity ai = new AiEntity();
                Vector2 pos = new Vector2(150.0f + ((float)ms_random.NextDouble() * 3300.0f),
                                            725.0f + ((float)ms_random.NextDouble() * 65.0f));

                ai.SetPosition(pos);

                ai.AddToBin(m_bin);

                m_ais.AddItem(ai, true);
            }

            for (int i = 0; i < 1000; ++i)
            {
                m_bullets.AddItem(new BulletEntity(), false);
            }




			m_players[0].SetPosition(new Vector2(416.0f, 320.0f));

            PathRegion[] regions = new PathRegion[2];
            regions[0] = new PathRegion(new Vector2(100.0f, 650.0f), new Vector2(900.0f, 800.0f));
            regions[1] = new PathRegion(new Vector2(200.0f, 150.0f), new Vector2(700.0f, 550.0f));

            regions[0].GenerateUniformGridOfNodes(10.0f, 50.0f);
            regions[1].GenerateUniformGridOfNodes(10.0f, 50.0f);

            m_pathIsland.SetRegions(regions);



			m_cameraController.TargetEntity = m_players[0];

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
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime((float)gameTime.ElapsedGameTime.TotalSeconds);

            m_explosions.Clear();

            m_worldManager.Update(frameTime.Dt);

            // Works for the debug rendering is hard on the eye.
            //if (ms_random.NextDouble() < 0.02f)
            //{
            //    Explosion explosion = new Explosion(new Vector2(350.0f, 750.0f), 150.0f, 25000.0f);
            //    m_explosions.Add(explosion);
            //}


            for (int i = 0; i < m_players.ActiveItemListCount; ++i)
            {
                m_players[i].UpdateInput(ref m_inputWrapper);
                m_players[i].Update(ref frameTime);
                m_players[i].UpdateBinEntry();
            }

            for (int i = 0; i < m_ais.ActiveItemListCount; ++i)
            {
                m_ais[i].Update(ref frameTime);
                m_ais[i].UpdateBinEntry();
            }

            for (int i = 0; i < m_bullets.ActiveItemListCount; ++i)
            {
                m_bullets[i].Update(ref frameTime);
                m_bullets[i].UpdateBinEntry();
            }

			GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
			KeyboardState keyboardState = Keyboard.GetState();
			m_inputWrapper.Update(gamePadState, keyboardState);




            m_contactList.StartAddingContacts();

            CollisionHelpers.GenerateContacts(m_ais.ActiveItemList, m_ais.ActiveItemListCount, m_bin, m_contactList);
            CollisionHelpers.GenerateContacts(m_players.ActiveItemList, m_players.ActiveItemListCount, m_bin, m_contactList);

            CollisionHelpers.UpdateBulletContacts(m_bullets.ActiveItemList, m_bullets.ActiveItemListCount, m_bin);

            CollisionHelpers.UpdateExplosions(m_explosions, m_bin);

            m_contactList.EndAddingContacts();

            m_contactResolver.ResolveContacts(frameTime.Dt, m_contactList);

            // Destroying dead entities/objects
            m_bullets.CoalesceActiveList(false);


			m_cameraController.Update(ref m_inputWrapper);

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

            for (int i = 0; i < m_players.ActiveItemListCount; ++i)
            {
                m_players[i].DebugRender(m_debugRenderer);
            }

            for (int i = 0; i < m_ais.ActiveItemListCount; ++i)
            {
                m_ais[i].DebugRender(m_debugRenderer);
            }

            for (int i = 0; i < m_bullets.ActiveItemListCount; ++i)
            {
                m_bullets[i].DebugRender(m_debugRenderer);
            }

            for (int i = 0; i < m_explosions.Count; ++i)
            {
                m_explosions[i].DebugRender(m_debugRenderer);
            }

            //m_pathIsland.DebugRender(m_debugRenderer);


			m_mapRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, GraphicsDevice);
			m_debugRenderer.Render(m_camera.ViewMatrix, m_camera.ProjectionMatrix, GraphicsDevice);

			base.Draw(gameTime);
		}
	}
}
