using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Momo.Core;
using Momo.Debug;

using TestGame.Input;
using Momo.Fonts;



namespace TestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TestGame : Microsoft.Xna.Framework.Game
    {
        public static TestGame Instance() { return ms_instance; }
        private static TestGame ms_instance = null;

        public const int kBackBufferWidth = 1280;
        public const int kBackBufferHeight = 720;

        //public const int kBackBufferWidth = 1980;
        //public const int kBackBufferHeight = 1080;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Profiler m_profiler = new Profiler();
        private int m_cpuUpdateProfileItemId = 0;
        private int m_cpuRenderProfileItemId = 0;
        private int m_cpuDebugRenderProfileItemId = 0;

        private WorldManager.WorldManager m_worldManager = new WorldManager.WorldManager();

        public InputManager InputManager { get; private set; }



        public TestGame()
        {
            System.Diagnostics.Debug.Assert(ms_instance == null);
            ms_instance = this;

            InputManager = new Input.InputManager();

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = kBackBufferWidth;
            graphics.PreferredBackBufferHeight = kBackBufferHeight;

            graphics.PreparingDeviceSettings += PreparingDeviceSettings;

            Content.RootDirectory = "Content";

            if (GraphicsAdapter.Adapters.Count > 1)
            {
                graphics.IsFullScreen = true;
            }
        }

        public void PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            GraphicsAdapter currentAdapter = GraphicsAdapter.DefaultAdapter;

            if (GraphicsAdapter.Adapters.Count > 1)
            {
                currentAdapter = GraphicsAdapter.Adapters[1];
            }

            e.GraphicsDeviceInformation.Adapter = currentAdapter;
        }

        protected override void Initialize()
        {
            m_profiler.Init(10, TestGame.Instance().GraphicsDevice);
            m_cpuUpdateProfileItemId = m_profiler.RegisterProfileItem("Update", new Color(1.0f, 0.0f, 0.0f, 0.5f));
            m_cpuRenderProfileItemId = m_profiler.RegisterProfileItem("Render", new Color(0.5f, 0.0f, 0.0f, 0.5f));
            m_cpuDebugRenderProfileItemId = m_profiler.RegisterProfileItem("DebugRender", new Color(1.0f, 0.0f, 0.0f, 0.5f));

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
            m_profiler.StartProfile(m_cpuUpdateProfileItemId);

            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime((float)gameTime.ElapsedGameTime.TotalSeconds);

            InputManager.Update(ref frameTime);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            m_worldManager.Update(frameTime.Dt);

            base.Update(gameTime);

            m_profiler.EndProfile(m_cpuUpdateProfileItemId);
        }




        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SteelBlue);

            m_profiler.StartProfile(m_cpuRenderProfileItemId);
            m_worldManager.Render();
            m_profiler.EndProfile(m_cpuRenderProfileItemId);

            m_profiler.StartProfile(m_cpuDebugRenderProfileItemId);
            m_worldManager.DebugRender();
            m_profiler.EndProfile(m_cpuDebugRenderProfileItemId);

            m_profiler.Render(GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
