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
		Vector3 m_position = Vector3.Zero;

		public CameraController()
		{
			m_position.Z = 10.0f;
		}

		public void Update(GamePadState currentGamePadState, KeyboardState currentKeyboardState)
		{
			if (Camera != null)
			{
				m_position += m_cameraSpeed;

				Vector3 newPos = new Vector3(
					(float)Math.Floor(m_position.X),
					(float)Math.Floor(m_position.Y),
					(float)Math.Floor(m_position.Z));

                Matrix cameraMatrix = Matrix.Identity;
                // Lets rotate the camera 180 in the z so that the map world renders nicely.
                cameraMatrix.Right = new Vector3(1.0f, 0.0f, 0.0f);
                cameraMatrix.Up = new Vector3(0.0f, -1.0f, 0.0f);
                cameraMatrix.Translation = newPos;

                Camera.Matrix = cameraMatrix;

				const float kAnalogDeadzone = 0.3f;

				float kMaxSpeed = 20.0f;
				const float kCamAccel = 5.0f;
				float xAmount = currentGamePadState.ThumbSticks.Left.X;

				bool leftKey = currentKeyboardState.IsKeyDown(Keys.Left);
				bool rightKey = currentKeyboardState.IsKeyDown(Keys.Right);
				if (leftKey && !rightKey)
				{
					xAmount = -1.0f;
				}
				else if(rightKey && !leftKey)
				{
					xAmount = 1.0f;
				}

				if(Math.Abs(xAmount) > kAnalogDeadzone)
				{
					if (Math.Abs(m_cameraSpeed.X) < kMaxSpeed)
						m_cameraSpeed.X += (kCamAccel * xAmount);
				}




				float yAmount = currentGamePadState.ThumbSticks.Left.Y;

				bool upKey = currentKeyboardState.IsKeyDown(Keys.Up);
				bool downKey = currentKeyboardState.IsKeyDown(Keys.Down);
				if (upKey && !downKey)
				{
					yAmount = -1.0f;
				}
				else if (downKey && !upKey)
				{
					yAmount = 1.0f;
				}

				if ( Math.Abs(yAmount) > kAnalogDeadzone)
				{
					if (Math.Abs(m_cameraSpeed.Y) < kMaxSpeed)
						m_cameraSpeed.Y += (kCamAccel * yAmount);
				}

				const float kDampeningFactor = 0.85f;
				m_cameraSpeed *= kDampeningFactor;
			}
		}
	}
}
