using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using TestGame.Objects;

namespace TestGame.Entities
{
	public class PlayerEntity : DynamicGameEntity
	{
		Vector2 m_inputVector = Vector2.Zero;

		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public PlayerEntity()
		{
			FacingAngle = 0.0f;
			SetContactRadiusInfo(new RadiusInfo(12.0f));
			DebugColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
		}

		public void UpdateInput(ref GamePadState currentGamePadState, ref KeyboardState currentKeyboardState)
		{
			const float kAnalogDeadzone = 0.3f;
			const float kAnalogDeadzoneSq = kAnalogDeadzone * kAnalogDeadzone;

			m_inputVector = Vector2.Zero;


			Vector2 padVector = new Vector2(
									currentGamePadState.ThumbSticks.Left.X,
									currentGamePadState.ThumbSticks.Left.Y);

			if (padVector.LengthSquared() > kAnalogDeadzoneSq)
			{
				m_inputVector = padVector;
				m_inputVector.Y *= -1.0f;
			}

			bool leftKey = currentKeyboardState.IsKeyDown(Keys.Left);
			bool rightKey = currentKeyboardState.IsKeyDown(Keys.Right);
			if (leftKey && !rightKey)
			{
				m_inputVector.X = -1.0f;
			}
			else if (rightKey && !leftKey)
			{
				m_inputVector.X = 1.0f;
			}

			bool upKey = currentKeyboardState.IsKeyDown(Keys.Up);
			bool downKey = currentKeyboardState.IsKeyDown(Keys.Down);
			if (upKey && !downKey)
			{
				m_inputVector.Y = -1.0f;
			}
			else if (downKey && !upKey)
			{
				m_inputVector.Y = 1.0f;
			}
		}

		public override void Update(ref FrameTime frameTime)
		{
			base.Update(ref frameTime);

			const float kMaxSpeed = 160.0f;

			Vector2 newPosition = GetPosition() + ((m_inputVector * kMaxSpeed) * frameTime.Dt);

			SetPosition(newPosition);

			float len = m_inputVector.Length();
			if (len > 0.0f)
			{
				FacingAngle = (float)Math.Atan2(m_inputVector.X, m_inputVector.Y);
			}
		}

		public void AddToBin(Bin bin)
		{
			BinRegionUniform curBinRegion = new BinRegionUniform();
			bin.GetBinRegionFromCentre(GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), ref curBinRegion);

			//bin.UpdateBinItem(this, ref curBinRegion, 0);

			SetBinRegion(curBinRegion);
		}


		public void UpdateBinEntry(Bin bin)
		{
			//BinRegionUniform prevBinRegion = new BinRegionUniform();
			BinRegionUniform curBinRegion = new BinRegionUniform();

			//GetBinRegion(ref prevBinRegion);
			bin.GetBinRegionFromCentre(GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), ref curBinRegion);

			//bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, 0);

			SetBinRegion(curBinRegion);
		}



		public override void OnCollisionEvent(ref IDynamicCollidable collidable)
		{

		}


		public void OnCollisionEvent(ref BulletEntity bullet)
		{
			SetForce(bullet.GetVelocity() * 10.0f);
		}


		public void OnExplosionEvent(ref Explosion explosion, Vector2 force)
		{
			SetForce(force);
		}
	}
}
