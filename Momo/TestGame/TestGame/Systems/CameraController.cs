using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Momo.Core.Nodes.Cameras;
using Momo.Core.GameEntities;
using Momo.Core;
using Momo.Fonts;

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
        private TextObject m_debugText = null;


        public CameraController(GameWorld world)
        {
            m_world = world;

            m_debugText = new TextObject("", TestGame.Instance().GetDebugFont(), 500, kDebugStringLength, 1);
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

        public void DebugRender()
        {
            Vector2 curPos = m_spring.GetCurrentValue();

            m_debugString.Clear();
            m_debugString.Append("(");
            m_debugString.Append(curPos.X, 2);
            m_debugString.Append(",");
            m_debugString.Append(curPos.Y, 2);
            m_debugString.Append(")");
            m_debugString.EndAppend();

            m_debugText.SetText(m_debugString.GetCharacterArray());

            m_world.GetTextPrinter().AddToDrawList(m_debugText);
        }

        private void UpdateFollow()
        {
            PlayerManager playerManager = m_world.GetPlayerManager();

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
