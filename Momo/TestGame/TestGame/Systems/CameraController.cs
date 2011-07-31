using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Momo.Core.Nodes.Cameras;

namespace TestGame.Systems
{
    class CameraController
    {
        public OrthographicCameraNode Camera { get; set; }
        Vector3 m_cameraSpeed = Vector3.Zero;

        public CameraController()
        {

        }

        public void Update(GamePadState currentGamePadState)
        {
            if (Camera != null)
            {
                Vector3 trans = Camera.LocalTranslation;
                trans += m_cameraSpeed;
                Camera.LocalTranslation = trans;

                const float kAnalogDeadzone = 0.3f;

                float kMaxSpeed = 20.0f;
                const float kCamAccel = 5.0f;
                float xLeftStick = currentGamePadState.ThumbSticks.Left.X;

                if(Math.Abs(xLeftStick) > kAnalogDeadzone)
                {
                    if (Math.Abs(m_cameraSpeed.X) < kMaxSpeed)
                        m_cameraSpeed.X += (kCamAccel * xLeftStick);
                }

                float yLeftStick = currentGamePadState.ThumbSticks.Left.Y;
                if ( Math.Abs(yLeftStick) > kAnalogDeadzone)
                {
                    if (Math.Abs(m_cameraSpeed.Y) < kMaxSpeed)
                        m_cameraSpeed.Y += (kCamAccel * yLeftStick);
                }

                const float kDampeningFactor = 0.85f;
                m_cameraSpeed *= kDampeningFactor;
            }
        }
    }
}
