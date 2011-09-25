using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using TestGame.Objects;
using Momo.Fonts;
using TestGame.Weapons;


namespace TestGame.Entities
{
    public class PlayerEntity : DynamicGameEntity, IWeaponUser
    {
        static readonly int kNumWeaponSlots = 3;


        private Vector2 m_movementInputVector = Vector2.Zero;
        private Vector2 m_facingInputVector = Vector2.Zero;
        private float m_triggerState = 0.0f;

        private Weapons.Weapon[] m_arsenal = new Weapons.Weapon[kNumWeaponSlots];
        private Weapons.Weapon m_currentWeapon = null;
        private int m_currentWeaponIdx = -1;

        private Input.InputWrapper m_input = null;

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public PlayerEntity(GameWorld world) : base(world)
        {
            FacingAngle = 0.0f;
            SetContactRadiusInfo(new RadiusInfo(16.0f));
            SetMass(GetContactRadiusInfo().Radius * 2.0f);
            DebugColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        }

        public Input.InputWrapper GetInputWrapper() { return m_input; }

        public void SetInputWrapper(Input.InputWrapper value) { m_input = value; }

        public void Init()
        {
            Systems.WeaponManager weaponMan = GetWorld().GetWeaponManager();
            m_arsenal[0] = weaponMan.Create(Systems.WeaponManager.WeaponType.Pistol);
            m_arsenal[1] = weaponMan.Create(Systems.WeaponManager.WeaponType.Shotgun);
            m_arsenal[2] = weaponMan.Create(Systems.WeaponManager.WeaponType.Minigun);

            for (int i = 0; i < kNumWeaponSlots; ++i)
            {
                m_arsenal[i].SetOwner(this);
            }

            m_currentWeaponIdx = 0;
        }


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            base.Update(ref frameTime, updateToken);

            UpdateInput();

            const float kMaxSpeed = 200.0f;

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

            if (m_currentWeapon != null)
            {
                m_currentWeapon.Update(ref frameTime, m_triggerState, newPosition, FacingAngle);

                AddForce(m_currentWeapon.Recoil);
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
            AddForce(bullet.GetVelocity());
        }


        public void OnExplosionEvent(ref Explosion explosion, Vector2 force)
        {
            AddForce(force);
        }


        private void UpdateInput()
        {
            // Handle weapon cycling
            if (m_input.IsButtonPressed(Buttons.RightShoulder))
            {
                ++m_currentWeaponIdx;
                if (m_currentWeaponIdx >= kNumWeaponSlots)
                {
                    m_currentWeaponIdx = 0;
                }
            }

            if (m_input.IsButtonPressed(Buttons.LeftShoulder))
            {
                --m_currentWeaponIdx;
                if (m_currentWeaponIdx < 0)
                {
                    m_currentWeaponIdx = kNumWeaponSlots - 1;
                }
            }

            m_currentWeapon = m_arsenal[m_currentWeaponIdx];

            if (m_currentWeapon.GetAmmoInClip() < m_currentWeapon.GetParams().m_clipSize)
            {
                if (m_input.IsButtonPressed(Buttons.X) || (m_currentWeapon.GetAmmoInClip() == 0))
                {
                    m_currentWeapon.Reload();
                }
            }

            m_movementInputVector = m_input.GetLeftStick();
            m_facingInputVector = m_input.GetRightStick();
            m_triggerState = m_input.GetRightTrigger();
        }

        // --------------------------------------------------------------------
        // -- IWeaponUser interface implementation
        // --------------------------------------------------------------------
        public BulletEntity.Flags GetBulletFlags()
        {
            return BulletEntity.Flags.HarmsEnemies;
        }

        public Weapon GetCurrentWeapon() { return m_currentWeapon; }
        public void SetCurrentWeapon(Weapon value)
        {
            throw new System.Exception("It's not possible to set the player's weapon externally!");
        }

    }
}
