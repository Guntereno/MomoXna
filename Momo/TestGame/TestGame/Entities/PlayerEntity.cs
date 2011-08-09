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

		public void UpdateInput(ref Input.InputWrapper input)
		{
			m_inputVector = input.GetLeftStick();
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
