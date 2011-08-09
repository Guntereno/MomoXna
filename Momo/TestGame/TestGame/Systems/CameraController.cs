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

		public void Update(ref Input.InputWrapper input)
		{
			if (Camera == null)
				return;

			if (input.IsButtonDown(Buttons.LeftShoulder))
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

		private void UpdateDebug(ref Input.InputWrapper input)
		{
			Vector3 pos = Camera.LocalTranslation;
			pos += m_cameraVelocity;

			// Calculate the acceleration based on the pad input
			const float kMaxAccel = 5.0f;
			Vector2 inputVector = input.GetLeftStick();
			Vector3 camAccel = new Vector3(
				inputVector.X * kMaxAccel,
				inputVector.Y * -kMaxAccel,
				0.0f
			);

			// Apply the acceleration
			m_cameraVelocity += camAccel;
			
			// Clamp the velocity
			float speed = m_cameraVelocity.Length();
			float kMaxSpeed = 20.0f;
			if (speed > kMaxSpeed)
			{
				m_cameraVelocity *= (kMaxSpeed / speed);
			}

			// Dampen the velocity
			const float kDampeningFactor = 0.85f;
			m_cameraVelocity *= kDampeningFactor;

			Camera.LocalTranslation = pos;
			
			Vector3 clampedPos = new Vector3(
				 (float)Math.Floor(pos.X),
				 (float)Math.Floor(pos.Y),
				 (float)Math.Floor(pos.Z));

			Matrix cameraMatrix = Matrix.Identity;
			// Lets rotate the camera 180 in the z so that the map world renders nicely.
			cameraMatrix.Right = new Vector3(1.0f, 0.0f, 0.0f);
			cameraMatrix.Up = new Vector3(0.0f, -1.0f, 0.0f);
			cameraMatrix.Translation = clampedPos;

			Camera.Matrix = cameraMatrix;
		}
	}
}
