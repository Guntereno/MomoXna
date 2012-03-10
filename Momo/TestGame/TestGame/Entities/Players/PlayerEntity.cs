﻿using System;
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
    public partial class PlayerEntity : LivingGameEntity, IWeaponUser, IStateMachineOwner
    {
        #region Fields

        private const int kNumWeaponSlots = 3;
        private const float kPlayerHealth = 2000.0f;

        #region State Machine
        private StateMachine m_stateMachine = null;
        private ActiveState m_stateActive = null;
        private DeadState m_stateDead = null;
        private DyingState m_stateDying = null;
        #endregion

        public Vector2 m_movementVector = Vector2.Zero;
        public Vector2 m_movementInputVector = Vector2.Zero;
        public Vector2 m_facingInputVector = Vector2.Zero;
        public float m_triggerState = 0.0f;

        private Weapons.Weapon[] m_arsenal = new Weapons.Weapon[kNumWeaponSlots];
        private Weapons.Weapon m_currentWeapon = null;
        private int m_currentWeaponIdx = -1;

        private Input.InputWrapper m_input = null;

        private Color m_playerColour = Color.White;

        private Bin m_bin = null;

        #endregion


        // --------------------------------------------------------------------
        // -- Properties
        // --------------------------------------------------------------------
        #region Properties

        public Vector2 InputMovement        { get { return m_movementInputVector; } }
        public Vector2 InputFacing          { get { return m_facingInputVector; } }
        public float TriggerState           { get { return m_triggerState; } }

        public Color PlayerColour
        {
            get { return m_playerColour; }
            set {
                m_playerColour = value;
                m_stateActive.DebugColor = m_playerColour;
            }
        }

        public Input.InputWrapper InputWrapper
        {
            get { return m_input; }
            set { m_input = value; }
        }


        #endregion



        // --------------------------------------------------------------------
        // -- Methods
        // --------------------------------------------------------------------
        public PlayerEntity(GameWorld world) : base(world)
        {
            ContactRadiusInfo = new RadiusInfo(14.0f);
            Mass = ContactRadiusInfo.Radius * 3.0f;

            CollidableGroupInfo.GroupMembership = new Flags((int)EntityGroups.Players);
            CollidableGroupInfo.CollidesWithGroups = new Flags((int)EntityGroups.AllEntities);

            FacingDirection = Vector2.UnitY;

            SecondaryDebugColor = new Color(0.0f, 1.0f, 0.0f);

            m_stateMachine = new StateMachine(this);

            m_stateActive = new ActiveState(this);
            m_stateDying = new DyingState(this);
            m_stateDead = new DeadState(this);

            m_stateActive.DebugColor = PlayerColour;
                        
            m_stateDying.Length = 0.5f;
            m_stateDying.ExitState = m_stateDead;
            Color color = Color.Gray;
            color.A = 128;
            m_stateDying.DebugColor = color;
                        
            m_stateDead.Length = 4.0f;
            m_stateDead.ExitState = m_stateActive;
            m_stateDead.DebugColor = Color.Transparent;
        }

        public void Init()
        {
            Systems.WeaponManager weaponMan = World.WeaponManager;
            m_arsenal[0] = weaponMan.Create(MapData.Weapon.Design.Pistol);
            m_arsenal[1] = weaponMan.Create(MapData.Weapon.Design.Shotgun);
            m_arsenal[2] = weaponMan.Create(MapData.Weapon.Design.Minigun);

            for (int i = 0; i < kNumWeaponSlots; ++i)
            {
                m_arsenal[i].Owner = this;
            }

            m_currentWeaponIdx = 0;

            m_stateMachine.CurrentState = m_stateActive;

            MaxHealth = kPlayerHealth;
            Health = kPlayerHealth;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            m_stateMachine.Update(ref frameTime, updateToken);
        }

        public void AddToBin(Bin bin)
        {
            m_bin = bin;
            AddToBin(bin, GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, BinLayers.kPlayerEntity);
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
            bin.GetBinRegionFromCentre(GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, BinLayers.kPlayerEntity);

            SetBinRegion(curBinRegion);
        }


        public override void OnCollisionEvent(ref IDynamicCollidable collidable)
        {

        }


        public override void OnCollisionEvent(ref BulletEntity bullet)
        {
            float damage = bullet.Params.m_damage;
            Vector2 direction = bullet.PositionDifferenceFromLastFrame;
            direction.Normalize();

            AddForce(direction * (damage * 500.0f));

            if (m_stateMachine.CurrentState != m_stateDying)
            {
                // Take damage from the bullet
                Health -= damage;
                if (Health <= 0.0f)
                {
                    Health = 0.0f;

                    m_stateMachine.CurrentState = m_stateDying;
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

            if (m_currentWeapon.AmmoInClip < m_currentWeapon.Parameters.m_clipSize)
            {
                if (m_input.IsButtonPressed(Buttons.X) || (m_currentWeapon.AmmoInClip == 0))
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
            const float kMaxSpeed = 225.0f;

            // If the player has a facing input, use it...
            if (m_facingInputVector.LengthSquared() > 0.0f)
            {
                FacingDirection = Vector2.Normalize(m_facingInputVector);
            }
            // If they're moving, update it from the movement vector
            else if (m_movementInputVector.LengthSquared() > 0.0f)
            {
                FacingDirection = Vector2.Normalize(m_movementVector);
            }


            Vector2 dMovement = m_movementInputVector - m_movementVector;
            m_movementVector += dMovement * 0.5f;

            Vector2 movementVectorNormalised = Vector2.Normalize(m_movementVector);

            float dotMovementFacing = Vector2.Dot(movementVectorNormalised, FacingDirection);

            float maxSpeedMod = 1.0f;


            // Back footing
            if (dotMovementFacing < -0.35f)
                maxSpeedMod = 0.77f;
            // Strafing
            else if (dotMovementFacing < 0.35f)
                maxSpeedMod = 0.85f;

            Vector2 newPosition = GetPosition() + ((m_movementVector * maxSpeedMod * kMaxSpeed) * frameTime.Dt);

            SetPosition(newPosition);
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
            Health = MaxHealth;
        }


        #region IWeaponUser

        public Weapon CurrentWeapon
        {
            get { return m_currentWeapon; }
            set
            {
                m_currentWeapon = value;
                throw new System.Exception("It's not possible to set the player's weapon externally!");
            }
        }

        public Flags BulletGroupMembership { get { return new Flags((int)EntityGroups.PlayerBullets); } }

        #endregion


        #region IStateMachineOwner

        public StateMachine StateMachine { get { return m_stateMachine; } }

        #endregion
    }
}
