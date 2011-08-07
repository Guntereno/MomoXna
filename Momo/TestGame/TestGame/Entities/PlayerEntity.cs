using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.GameEntities;
using Momo.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Momo.Debug;
using Momo.Core.Spatial;
using Momo.Core.Collision2D;
using TestGame.Objects;

namespace TestGame.Entities
{
	class PlayerEntity : DynamicGameEntity
	{
		float m_facingAngle = 0.0f;

		Vector2 m_inputVector = Vector2.Zero;

		private RadiusInfo m_contactRadiusInfo;

		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public PlayerEntity()
		{
			m_facingAngle = 0.0f;
			m_contactRadiusInfo = new RadiusInfo(12.0f);
		}

		public RadiusInfo GetContactRadiusInfo()
		{
			return m_contactRadiusInfo;
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

			if (m_inputVector.Length() > 0.0f)
			{
				m_facingAngle = (float)Math.Sin((double)(m_inputVector.X));
			}
		}


		public override void DebugRender(DebugRenderer debugRenderer)
		{
			debugRenderer.DrawCircle(GetPosition(), m_contactRadiusInfo.Radius, new Color(0.0f, 0.0f, 1.0f, 0.4f), new Color(0.0f, 0.0f, 0.2f, 0.75f), true, 2, 8);
		}

		public void AddToBin(Bin bin)
		{
			BinRegionUniform curBinRegion = new BinRegionUniform();
			bin.GetBinRegionFromCentre(GetPosition(), m_contactRadiusInfo.Radius + GetContactDimensionPadding(), ref curBinRegion);

			bin.UpdateBinItem(this, ref curBinRegion, 0);

			SetBinRegion(curBinRegion);
		}


		public void UpdateBinEntry(Bin bin)
		{
			BinRegionUniform prevBinRegion = new BinRegionUniform();
			BinRegionUniform curBinRegion = new BinRegionUniform();

			GetBinRegion(ref prevBinRegion);
			bin.GetBinRegionFromCentre(GetPosition(), m_contactRadiusInfo.Radius + GetContactDimensionPadding(), ref curBinRegion);

			bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, 0);

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
