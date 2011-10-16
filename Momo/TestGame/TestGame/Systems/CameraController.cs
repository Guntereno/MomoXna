using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Momo.Core;
using Momo.Core.Nodes.Cameras;
using Momo.Core.GameEntities;
using Momo.Fonts;
using Momo.Debug;



namespace TestGame.Systems
{
    class CameraController
    {
        public OrthographicCameraNode Camera { get; set; }
        public Vector2 FollowPosition { get; set; }

        Vector3 m_cameraVelocity = Vector3.Zero;

        Momo.Maths.CriticallyDampenedSpring m_spring = new Momo.Maths.CriticallyDampenedSpring();

        private enum Behaviour
        {
            Debug,
            Follow
        }
        Behaviour m_behaviour = Behaviour.Debug;

        private  GameWorld m_world;


        const int kDebugStringLength = 64;
        protected MutableString m_debugString = new MutableString(kDebugStringLength);


        public CameraController(GameWorld world)
        {
            m_world = world;
        }

        public void Update(ref FrameTime frameTime, ref Input.InputWrapper input)
        {
            if (Camera == null)
                return;

            if (input.IsButtonDown(Buttons.LeftStick))
            {
                m_behaviour = Behaviour.Debug;
            }
            else
            {
                m_behaviour = Behaviour.Follow;
            }

            switch (m_behaviour)
            {
                case Behaviour.Debug:
                    UpdateDebug(ref input);
                    break;

                case Behaviour.Follow:
                    UpdateFollow();
                    break;
            }

            m_spring.Update(frameTime.Dt);

            Vector2 springPos = m_spring.GetCurrentValue();
            const float kCamHeight = 10.0f;
            Camera.LocalTranslation = new Vector3(
                (float)Math.Floor(springPos.X),
                (float)Math.Floor(springPos.Y),
                kCamHeight);

            Matrix cameraMatrix = Matrix.Identity;
            // Lets rotate the camera 180 in the z so that the map world renders nicely.
            cameraMatrix.Right = new Vector3(1.0f, 0.0f, 0.0f);
            cameraMatrix.Up = new Vector3(0.0f, -1.0f, 0.0f);
            cameraMatrix.Translation = Camera.LocalTranslation;

            Camera.Matrix = cameraMatrix;
        }

        public void DebugRender(DebugRenderer debugRenderer, TextBatchPrinter debugTextBatchPrinter, TextStyle debugTextStyle)
        {
            Vector2 curPos = m_spring.GetCurrentValue();

            m_debugString.Clear();
            m_debugString.Append("(");
            m_debugString.Append(curPos, 0);
            m_debugString.Append(")");
            m_debugString.EndAppend();

            debugTextBatchPrinter.AddToDrawList(m_debugString.GetCharacterArray(), Color.White, Color.Black, new Vector2(0.0f, 0.0f), debugTextStyle);
        }

        private void UpdateFollow()
        {
            PlayerManager playerManager = m_world.PlayerManager;

            m_spring.SetTarget(playerManager.GetAveragePosition());
        }

        private void UpdateDebug(ref Input.InputWrapper input)
        {
            Vector2 pos = m_spring.GetTarget();
            const float kMaxSpeed = 30.0f;
            Vector2 inputVector = input.GetRightStick();
            pos += inputVector * kMaxSpeed;
            m_spring.SetTarget(pos);
        }
    }
}
