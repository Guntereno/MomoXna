using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Momo.Core.Nodes.Cameras;
using Momo.Core.GameEntities;

namespace TestGame.Systems
{
	class CameraController
	{
		public OrthographicCameraNode Camera { get; set; }

		public BaseEntity TargetEntity { get; set; }

		Vector3 m_cameraVelocity = Vector3.Zero;

		private enum Behaviour
		{
			Debug,
			Follow
		}
		Behaviour m_behaviour = Behaviour.Debug;

		public CameraController()
		{

		}

		public void Update(GamePadState currentGamePadState, KeyboardState currentKeyboardState)
		{
			if (Camera == null)
				return;

			if (currentGamePadState.IsButtonDown(Buttons.LeftShoulder))
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
					UpdateDebug(ref currentGamePadState, ref currentKeyboardState);
					break;

				case Behaviour.Follow:
					UpdateFollow();
					break;
			}
		}

		private void UpdateFollow()
		{
			if (TargetEntity != null)
			{
				Vector3 pos = Camera.LocalTranslation;
				Vector3 oldPos = pos;

				Vector2 entityPosition = TargetEntity.GetPosition();
				pos.X = entityPosition.X;
				pos.Y = entityPosition.Y;

				Camera.LocalTranslation = new Vector3(
					(float)Math.Floor(pos.X),
					(float)Math.Floor(pos.Y),
					(float)Math.Floor(pos.Z));

				m_cameraVelocity = pos - oldPos;

				Matrix cameraMatrix = Matrix.Identity;
				// Lets rotate the camera 180 in the z so that the map world renders nicely.
				cameraMatrix.Right = new Vector3(1.0f, 0.0f, 0.0f);
				cameraMatrix.Up = new Vector3(0.0f, -1.0f, 0.0f);
				cameraMatrix.Translation = Camera.LocalTranslation;

				Camera.Matrix = cameraMatrix;
			}
		}

		private void UpdateDebug(ref GamePadState currentGamePadState, ref KeyboardState currentKeyboardState)
		{
			Vector3 pos = Camera.LocalTranslation;
			pos += m_cameraVelocity;

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
			else if (rightKey && !leftKey)
			{
				xAmount = 1.0f;
			}

			if (Math.Abs(xAmount) > kAnalogDeadzone)
			{
				if (Math.Abs(m_cameraVelocity.X) < kMaxSpeed)
					m_cameraVelocity.X += (kCamAccel * xAmount);
			}


			float yAmount = -currentGamePadState.ThumbSticks.Left.Y;

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

			if (Math.Abs(yAmount) > kAnalogDeadzone)
			{
				if (Math.Abs(m_cameraVelocity.Y) < kMaxSpeed)
					m_cameraVelocity.Y += (kCamAccel * yAmount);
			}

			const float kDampeningFactor = 0.85f;
			m_cameraVelocity *= kDampeningFactor;

			Camera.LocalTranslation = new Vector3(
				(float)Math.Floor(pos.X),
				(float)Math.Floor(pos.Y),
				(float)Math.Floor(pos.Z));

			Matrix cameraMatrix = Matrix.Identity;
			// Lets rotate the camera 180 in the z so that the map world renders nicely.
			cameraMatrix.Right = new Vector3(1.0f, 0.0f, 0.0f);
			cameraMatrix.Up = new Vector3(0.0f, -1.0f, 0.0f);
			cameraMatrix.Translation = Camera.LocalTranslation;

			Camera.Matrix = cameraMatrix;
		}
	}
}
