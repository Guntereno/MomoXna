#define NO_SOUND

using System;
using System.Collections.Generic;
using System.Threading;

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

using Game.Entities;
using Game.Entities.AI;
using Game.Systems;

using WorldManager;
using Microsoft.Xna.Framework.Audio;



namespace Game
{
    public class GameWorld : World
    {
        public static World WorldCreator() { return new GameWorld(); }

        private TextBatchPrinter mTextPrinter = new TextBatchPrinter();


        private PerspectiveCameraNode mCamera = new PerspectiveCameraNode("Camera");
        private CameraController mCameraController = null;


        private Zone mZone = null;


        // Debug
        private DebugRenderer mDebugRenderer = new DebugRenderer();
        private Font mDebugFont = null;
        private TextStyle mDebugTextStyle = null;
        private TextBatchPrinter mDebugTextPrinter = new TextBatchPrinter();

#if !NO_SOUND
        private AudioEngine mAudioEngine = null;
        private WaveBank mWaveBank = null;
        private SoundBank mSoundBank = null;
#endif


        public PerspectiveCameraNode Camera                 { get { return mCamera; } }
        public CameraController CameraController            { get { return mCameraController; } }

        public DebugRenderer DebugRenderer                  { get { return mDebugRenderer; } }
        public Font DebugFont                               { get { return mDebugFont; } }
        public TextStyle DebugTextStyle                     { get { return mDebugTextStyle; } }
        public TextBatchPrinter DebugTextPrinter            { get { return mDebugTextPrinter; } }
        public TextBatchPrinter TextPrinter                 { get { return mTextPrinter; } }

        public Director.Director Director                   { get; private set; }

#if !NO_SOUND
        private AudioEngine AudioEngine                     { get { return mAudioEngine; } }
        private WaveBank WaveBank                           { get { return mWaveBank; } }
        private SoundBank SoundBank                         { get { return mSoundBank; } }
#endif

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public GameWorld()
        {
            Director = new Director.Director();
        }


        public override void Load()
        {
            Effect textEffect = Game.Instance.Content.Load<Effect>("effects/text");

            // Debug
            mDebugRenderer.Init(50000, 1000, Game.Instance.GraphicsDevice);
            mDebugTextPrinter.Init(textEffect, new Vector2((float)Game.kBackBufferWidth, (float)Game.kBackBufferHeight), 500, 1000, 1);
            mDebugFont = Game.Instance.Content.Load<Font>("fonts/Consolas_24_o2");
            mDebugTextStyle = new TextStyle(mDebugFont, TextSecondaryDrawTechnique.kDropshadow);

            mTextPrinter.Init(textEffect, new Vector2((float)Game.kBackBufferWidth, (float)Game.kBackBufferHeight), 100, 1000, 1);


            mCameraController = new CameraController();

            /*
            mCamera.ViewWidth = TestGame.kBackBufferWidth;
            mCamera.ViewHeight = TestGame.kBackBufferHeight;
            */

            Camera.AspectRatio = (float)Game.kBackBufferWidth / (float)Game.kBackBufferHeight;

            mCamera.LocalTranslation = new Vector3(300.0f, 750.0f, 10.0f);
            mCameraController.Camera = mCamera;

#if !NO_SOUND
            mAudioEngine = new AudioEngine("Content\\Audio\\audio.xgs");
            mWaveBank = new WaveBank(mAudioEngine, "Content\\Audio\\Wave Bank.xwb");
            mSoundBank = new SoundBank(mAudioEngine, "Content\\Audio\\Sound Bank.xsb");
#endif

            mZone = new Zone(this);
            mZone.Load();

            Director.LoadZone(mZone);
        }


        public override void Enter()
        {
            mZone.Enter();
        }


        public override void Update(float dt)
        {
            // More time related numbers will eventually be added to the FrameTime structure. Its worth passing
            // it about instead of just dt, so we can easily refactor.
            FrameTime frameTime = new FrameTime(dt);
            
            Input.InputWrapper inputWrapper = Game.Instance.InputManager.GetInputWrapper(0);


            mZone.Update(dt);


            mCameraController.Update(ref frameTime, ref inputWrapper);

#if !NO_SOUND
            mAudioEngine.Update();
#endif
        }



        public override void Exit()
        {
            mZone.Exit();
        }


        public override void Flush()
        {
            mZone.Flush();
        }


        public override void PreRender()
        {
            mZone.PreRender();

            mCamera.PreRenderUpdate();
        }


        public override void Render()
        {
            mZone.Render();

            TextPrinter.Render(true, Game.Instance.GraphicsDevice);
            TextPrinter.ClearDrawList();
        }


        public override void PostRender()
        {
            mZone.PostRender();
        }


        public override void DebugRender()
        {
            Director.DebugRender(mDebugRenderer);

            mZone.DebugRender();

            mCameraController.DebugRender(mDebugRenderer, mDebugTextPrinter, mDebugTextStyle);

            DebugRenderer.Render(mCamera.ViewMatrix, mCamera.ProjectionMatrix, Game.Instance.GraphicsDevice);
            DebugRenderer.Clear();


            // Render any debug text objects that where added.
            DebugTextPrinter.Render(true, Game.Instance.GraphicsDevice);
            DebugTextPrinter.ClearDrawList();
        }


        public void PlaySoundQueue(string name)
        {
#if !NO_SOUND
            SoundBank.PlayCue(name);
#endif
        }
    }
}
