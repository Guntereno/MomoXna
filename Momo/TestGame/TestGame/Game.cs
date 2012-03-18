using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Momo.Core;
using Momo.Debug;

using Game.Input;
using Momo.Fonts;



namespace Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static Game Instance { get { return msInstance; } }
        private static Game msInstance = null;

        public const int kBackBufferWidth = 1280;
        public const int kBackBufferHeight = 720;

        //public const int kBackBufferWidth = 1980;
        //public const int kBackBufferHeight = 1080;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Profiler mProfiler = new Profiler();
        private int mCpuUpdateProfileItemId = 0;
        private int mCpuRenderProfileItemId = 0;
        private int mCpuDebugRenderProfileItemId = 0;

        private WorldManager.WorldManager m_worldManager = new WorldManager.WorldManager();

        public InputManager InputManager { get; private set; }

        public Game()
        {
            if (msInstance != null)
                throw new System.Exception("Attempt to instantiate Singleton twice!");
            msInstance = this;

            new ResourceManager(Content);

            InputManager = new Input.InputManager();

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = kBackBufferWidth;
            graphics.PreferredBackBufferHeight = kBackBufferHeight;

            graphics.PreparingDeviceSettings += PreparingDeviceSettings;

            Content.RootDirectory = "Content";

            //if (GraphicsAdapter.Adapters.Count > 1)
            //{
            //    graphics.IsFullScreen = true;
            //}
        }

        public void PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            GraphicsAdapter currentAdapter = GraphicsAdapter.DefaultAdapter;

            //if (GraphicsAdapter.Adapters.Count > 1)
            //{
            //    currentAdapter = GraphicsAdapter.Adapters[1];
            //}

            e.GraphicsDeviceInformation.Adapter = currentAdapter;
        }

        protected override void Initialize()
        {
            mProfiler.Init(10, Game.Instance.GraphicsDevice);
            mCpuUpdateProfileItemId = mProfiler.RegisterProfileItem("Update", new Color(1.0f, 0.0f, 0.0f, 0.5f));
            mCpuRenderProfileItemId = mProfiler.RegisterProfileItem("Render", new Color(0.5f, 0.0f, 0.0f, 0.5f));
            mCpuDebugRenderProfileItemId = mProfiler.RegisterProfileItem("DebugRender", new Color(1.0f, 0.0f, 0.0f, 0.5f));

            m_worldManager.RegisterWorld("TestWorld", GameWorld.WorldCreator);

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            m_worldManager.LoadWorld("TestWorld");
        }


        protected override void UnloadContent()
        {
            m_worldManager.FlushWorld("TestWorld");
        }


        protected override void Update(GameTime gameTime)
        {
            mProfiler.StartProfile(mCpuUpdateProfileItemId);

            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime((float)gameTime.ElapsedGameTime.TotalSeconds);

            InputManager.Update(ref frameTime);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            m_worldManager.Update(frameTime.Dt);

            base.Update(gameTime);

            mProfiler.EndProfile(mCpuUpdateProfileItemId);
        }




        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SteelBlue);

            mProfiler.StartProfile(mCpuRenderProfileItemId);
            m_worldManager.Render();
            mProfiler.EndProfile(mCpuRenderProfileItemId);

            mProfiler.StartProfile(mCpuDebugRenderProfileItemId);
            m_worldManager.DebugRender();
            mProfiler.EndProfile(mCpuDebugRenderProfileItemId);

            mProfiler.Render(GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
