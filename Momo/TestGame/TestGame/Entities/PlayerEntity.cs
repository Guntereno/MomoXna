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
		Vector2 m_movementInputVector = Vector2.Zero;
		Vector2 m_facingInputVector = Vector2.Zero;
		bool m_fireHeld = false;

		System.Random m_random = new System.Random();

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
			m_movementInputVector = input.GetLeftStick();
			m_facingInputVector = input.GetRightStick();
			m_fireHeld = input.IsButtonDown(Buttons.RightShoulder);
		}

		public override void Update(ref FrameTime frameTime)
		{
			base.Update(ref frameTime);

			const float kMaxSpeed = 160.0f;

			Vector2 newPosition = GetPosition() + ((m_movementInputVector * kMaxSpeed) * frameTime.Dt);

			SetPosition(newPosition);


			// If the player has a facing input, use it...
			if (m_facingInputVector != Vector2.Zero)
			{
				FacingAngle = (float)Math.Atan2(m_facingInputVector.X, m_facingInputVector.Y);
			}
			else
			{
				// If they're moving, update it from the movement vector
				float len = m_movementInputVector.Length();
				if (len > 0.0f)
				{
					FacingAngle = (float)Math.Atan2(m_movementInputVector.X, m_movementInputVector.Y);
				}
			}

			if (m_fireHeld)
			{
				Vector2 startPos = new Vector2(GetPosition().X, GetPosition().Y);

				const float kVariance = 0.1f;
				float angle = FacingAngle + (((float)m_random.NextDouble() * kVariance) - (0.5f * kVariance));
				Vector2 velocity = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
				velocity *= 750.0f;

				TestGame.Instance().AddBullet(startPos, velocity);
			}

		}


        public void AddToBin(Bin bin)
        {
            SetBin(bin);
        }


        public void RemoveFromBin()
        {
            SetBin(null);
        }


		public void UpdateBinEntry()
		{
			BinRegionUniform curBinRegion = new BinRegionUniform();

			GetBin().GetBinRegionFromCentre(GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), ref curBinRegion);

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
