using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.StateMachine;
using Momo.Core.Spatial;
using TestGame.Objects;
using TestGame.Weapons;


namespace TestGame.Entities.Players
{
    public class PlayerEntity : DynamicGameEntity, IWeaponUser
    {
        static readonly int kNumWeaponSlots = 3;

        static readonly float kPlayerHealth = 2000.0f;

        #region State Machine
        private StateMachine m_stateMachine = null;
        private ActiveState m_stateActive = null;
        private DeadState m_stateDead = null;
        private DyingState m_stateDying = null;
        #endregion

        public Vector2 m_movementInputVector = Vector2.Zero;
        public Vector2 m_facingInputVector = Vector2.Zero;
        public float m_triggerState = 0.0f;

        private Weapons.Weapon[] m_arsenal = new Weapons.Weapon[kNumWeaponSlots];
        private Weapons.Weapon m_currentWeapon = null;
        private int m_currentWeaponIdx = -1;

        private Input.InputWrapper m_input = null;

        private Color m_playerColour = Color.White;

        Bin m_bin = null;

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public PlayerEntity(GameWorld world) : base(world)
        {
            FacingAngle = 0.0f;
            SetContactRadiusInfo(new RadiusInfo(16.0f));
            SetMass(GetContactRadiusInfo().Radius * 2.0f);
            DebugColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);

            m_stateMachine = new StateMachine(this);
            m_stateActive = new ActiveState(m_stateMachine);
            m_stateDying = new DyingState(m_stateMachine);
            m_stateDead = new DeadState(m_stateMachine);

            m_stateDying.SetLength(0.5f);
            m_stateDying.SetExitState(m_stateDead);

            m_stateDead.SetLength(4.0f);
            m_stateDead.SetExitState(m_stateActive);
        }

        public Input.InputWrapper GetInputWrapper() { return m_input; }
        public Vector2 GetInputMovement() { return m_movementInputVector; }
        public Vector2 GetInputFacing() { return m_facingInputVector; }
        public float GetTriggerState() { return m_triggerState; }

        public Color GetPlayerColour() { return m_playerColour; }
        public void SetPlayerColour(Color value) { m_playerColour = value; }

        public void SetInputWrapper(Input.InputWrapper value) { m_input = value; }

        public void Init()
        {
            Systems.WeaponManager weaponMan = GetWorld().WeaponManager;
            m_arsenal[0] = weaponMan.Create(MapData.Weapon.Design.Pistol);
            m_arsenal[1] = weaponMan.Create(MapData.Weapon.Design.Shotgun);
            m_arsenal[2] = weaponMan.Create(MapData.Weapon.Design.Minigun);

            for (int i = 0; i < kNumWeaponSlots; ++i)
            {
                m_arsenal[i].SetOwner(this);
            }

            m_currentWeaponIdx = 0;

            m_stateMachine.SetCurrentState(m_stateActive);

            m_maxHealth = m_health = kPlayerHealth;
        }


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            base.Update(ref frameTime, updateToken);

            m_stateMachine.Update(ref frameTime);
        }

        public void AddToBin(Bin bin)
        {
            m_bin = bin;
            AddToBin(bin, GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), BinLayers.kPlayerEntity);
        }


        public void RemoveFromBin()
        {
            RemoveFromBin(BinLayers.kPlayerEntity);
        }


        public void UpdateBinEntry()
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Bin bin = GetBin();

            GetBinRegion(ref prevBinRegion);
            bin.GetBinRegionFromCentre(GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, BinLayers.kPlayerEntity);

            SetBinRegion(curBinRegion);
        }


        public override void OnCollisionEvent(ref IDynamicCollidable collidable)
        {

        }


        public override void OnCollisionEvent(ref BulletEntity bullet)
        {
            float damage = bullet.GetParams().m_damage;
            Vector2 direction = bullet.GetPositionDifferenceFromLastFrame();
            direction.Normalize();

            AddForce(direction * (damage * 500.0f));

            if (m_stateMachine.GetCurrentState() != m_stateDying)
            {
                // Take damage from the bullet
                m_health -= damage;
                if (m_health <= 0.0f)
                {
                    m_health = 0.0f;

                    m_stateMachine.SetCurrentState(m_stateDying);
                }
            }
        }


        public void OnExplosionEvent(ref Explosion explosion, Vector2 force)
        {
            AddForce(force);
        }


        public void UpdateInput()
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


        internal void UpdateMovement(ref FrameTime frameTime)
        {
            const float kMaxSpeed = 200.0f;

            Vector2 newPosition = GetPosition() + ((m_movementInputVector * kMaxSpeed) * frameTime.Dt);

            SetPosition(newPosition);

            // If the player has a facing input, use it...
            Vector2 facing = m_facingInputVector;
            if (facing != Vector2.Zero)
            {
                FacingAngle = (float)Math.Atan2(facing.X, facing.Y);
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
        }

        internal void UpdateWeapon(ref FrameTime frameTime)
        {
            if (m_currentWeapon != null)
            {
                m_currentWeapon.Update(ref frameTime, m_triggerState, GetPosition(), FacingAngle);

                AddForce(m_currentWeapon.Recoil);
            }
        }

        internal void Kill()
        {
            RemoveFromBin();
        }

        internal void Spawn()
        {
            AddToBin(m_bin);
            m_health = m_maxHealth;
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
