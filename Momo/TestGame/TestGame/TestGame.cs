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

		public const int kBackBufferWidth = 1200;
		public const int kBackBufferHeight = 900;


		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Input.InputWrapper m_inputWrapper = new Input.InputWrapper();
		WorldManager.WorldManager m_worldManager = new WorldManager.WorldManager();


        public Input.InputWrapper InputWrapper
        {
            get { return m_inputWrapper; }
        }



		public TestGame()
		{
			System.Diagnostics.Debug.Assert(ms_instance == null);
			ms_instance = this;

			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = kBackBufferWidth;
			graphics.PreferredBackBufferHeight = kBackBufferHeight;

			Content.RootDirectory = "Content";
		}


		protected override void Initialize()
		{
			m_worldManager.RegisterWorld("TestWorld", TestWorld.WorldCreator);
            m_worldManager.RegisterWorld("TestWorldPathFinding", TestWorldPathFinding.WorldCreator);

			base.Initialize();
		}


		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

            m_worldManager.LoadWorld("TestWorld");
            //m_worldManager.LoadWorld("TestWorldPathFinding");
		}


		protected override void UnloadContent()
		{
            m_worldManager.FlushWorld("TestWorld");
            //m_worldManager.LoadWorld("TestWorldPathFinding");
		}


		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();


            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();
            m_inputWrapper.Update(gamePadState, keyboardState);


            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime((float)gameTime.ElapsedGameTime.TotalSeconds);

            m_worldManager.Update(frameTime.Dt);

			base.Update(gameTime);
		}




		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.SteelBlue);

			m_worldManager.Render();

			base.Draw(gameTime);
		}
	}
}
