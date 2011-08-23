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
        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public PlayerEntity(GameWorld world) : base(world)
        {
            FacingAngle = 0.0f;
            SetContactRadiusInfo(new RadiusInfo(12.0f));
            DebugColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        }

        public void UpdateInput(ref Input.InputWrapper input)
        {
            m_movementInputVector = input.GetLeftStick();
            m_facingInputVector = input.GetRightStick();
            m_triggerState = input.GetRightTrigger();
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

            if (m_weapon != null)
            {
                m_weapon.Update(ref frameTime, m_triggerState, newPosition, FacingAngle);
            }
            else
            {
                m_weapon = GetWorld().GetWeaponManager().Create(Systems.WeaponManager.WeaponType.Shotgun);
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

        Vector2 m_movementInputVector = Vector2.Zero;
        Vector2 m_facingInputVector = Vector2.Zero;
        float m_triggerState = 0.0f;

        Weapons.Weapon m_weapon = null;

        System.Random m_random = new System.Random();
    }
}
