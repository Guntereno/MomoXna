using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using TestGame.Objects;
using Fonts;

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
            SetContactRadiusInfo(new RadiusInfo(16.0f));
            SetMass(GetContactRadiusInfo().Radius * 2.0f);
            DebugColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        }


        public void UpdateInput(ref Input.InputWrapper input)
        {
            // Handle weapon cycling
            if(input.IsButtonPressed(Buttons.RightShoulder))
            {
                ++m_currentWeapon ;
                if(m_currentWeapon >= kNumWeaponSlots)
                {
                    m_currentWeapon = 0;
                }
            }
            if(input.IsButtonPressed(Buttons.LeftShoulder))
            {
                --m_currentWeapon ;
                if(m_currentWeapon < 0)
                {
                    m_currentWeapon = kNumWeaponSlots-1;
                }
            }

            Weapons.Weapon currentWeapon = m_arsenal[m_currentWeapon];
            if (currentWeapon.GetAmmoInClip() < currentWeapon.GetParams().m_clipSize)
            {
                if (input.IsButtonPressed(Buttons.X) || (m_arsenal[m_currentWeapon].GetAmmoInClip() == 0))
                {
                    m_arsenal[m_currentWeapon].Reload();
                }
            }

            m_movementInputVector = input.GetLeftStick();
            m_facingInputVector = input.GetRightStick();
            m_triggerState = input.GetRightTrigger();
        }

        public void Init()
        {
            Systems.WeaponManager weaponMan = GetWorld().GetWeaponManager();
            m_arsenal[0] = weaponMan.Create(Systems.WeaponManager.WeaponType.Pistol);
            m_arsenal[1] = weaponMan.Create(Systems.WeaponManager.WeaponType.Shotgun);
            m_arsenal[2] = weaponMan.Create(Systems.WeaponManager.WeaponType.Minigun);

            m_currentWeapon = 0;
        }

        public override void Update(ref FrameTime frameTime)
        {
            base.Update(ref frameTime);

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

            if (m_currentWeapon >= 0)
            {
                Weapons.Weapon curWeapon = m_arsenal[m_currentWeapon];
                curWeapon.Update(ref frameTime, m_triggerState, newPosition, FacingAngle);

                AddForce(curWeapon.Recoil);

                if (m_ammoOsd != null)
                {
                    m_ammoString.Clear();

                    m_ammoString.Append(curWeapon.ToString());
                    m_ammoString.Append("\n");
                    m_ammoString.Append(curWeapon.GetAmmoInClip());
                    m_ammoString.Append("\n");
                    m_ammoString.Append(curWeapon.GetCurrentStateName());

                    m_ammoString.EndAppend();


                    m_ammoOsd.SetText(m_ammoString.GetCharacterArray());
                }
            }
            else
            {
                if (m_ammoOsd != null)
                {
                    m_ammoOsd.SetText("");
                }
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

        public void SetAmmoOsd(TextObject ammoOsd)
        {
            m_ammoOsd = ammoOsd;
        }


        Vector2 m_movementInputVector = Vector2.Zero;
        Vector2 m_facingInputVector = Vector2.Zero;
        float m_triggerState = 0.0f;

        static readonly int kNumWeaponSlots = 3;
        Weapons.Weapon[] m_arsenal = new Weapons.Weapon[kNumWeaponSlots];

        int m_currentWeapon = -1;

        TextObject m_ammoOsd = null;
        MutableString m_ammoString = new MutableString(40);
    }
}
