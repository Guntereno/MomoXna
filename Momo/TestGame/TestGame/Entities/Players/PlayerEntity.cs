using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.StateMachine;
using Momo.Core.Spatial;

using Game.Weapons;



namespace Game.Entities.Players
{
    public partial class PlayerEntity : LivingGameEntity, IWeaponUser, IStateMachineOwner
    {
        #region Fields

        private const int kNumWeaponSlots = 3;
        private const float kPlayerHealth = 2000.0f;

        #region State Machine
        private StateMachine mStateMachine = null;
        private ActiveState mStateActive = null;
        private DeadState mStateDead = null;
        private DyingState mStateDying = null;
        #endregion

        public Vector2 mMovementVector = Vector2.Zero;
        public Vector2 mMovementInputVector = Vector2.Zero;
        public Vector2 mFacingInputVector = Vector2.Zero;
        public float mTriggerState = 0.0f;

        private Weapons.Weapon[] mArsenal = new Weapons.Weapon[kNumWeaponSlots];
        private Weapons.Weapon mCurrentWeapon = null;
        private int mCurrentWeaponIdx = -1;

        private Input.InputWrapper mInput = null;

        private Color mPlayerColour = Color.White;


        #endregion


        // --------------------------------------------------------------------
        // -- Properties
        // --------------------------------------------------------------------
        #region Properties

        public Vector2 InputMovement        { get { return mMovementInputVector; } }
        public Vector2 InputFacing          { get { return mFacingInputVector; } }
        public float TriggerState           { get { return mTriggerState; } }

        public Color PlayerColour
        {
            get { return mPlayerColour; }
            set {
                mPlayerColour = value;
                mStateActive.DebugColor = mPlayerColour;
            }
        }

        public Input.InputWrapper InputWrapper
        {
            get { return mInput; }
            set { mInput = value; }
        }


        #endregion



        // --------------------------------------------------------------------
        // -- Methods
        // --------------------------------------------------------------------
        public PlayerEntity(Zone zone) : base(zone)
        {
            ContactRadiusInfo = new RadiusInfo(12.0f);
            Mass = ContactRadiusInfo.Radius * 3.0f;

            CollidableGroupInfo.GroupMembership = new Flags((int)EntityGroups.Players);
            CollidableGroupInfo.CollidesWithGroups = new Flags((int)EntityGroups.AllEntities);

            FacingDirection = Vector2.UnitY;

            mStateMachine = new StateMachine(this);

            mStateActive = new ActiveState(this);
            mStateDying = new DyingState(this);
            mStateDead = new DeadState(this);

            mStateActive.DebugColor = PlayerColour;
                        
            mStateDying.Length = 0.5f;
            mStateDying.ExitState = mStateDead;
            Color color = Color.Gray;
            color.A = 128;
            mStateDying.DebugColor = color;
                        
            mStateDead.Length = 4.0f;
            mStateDead.ExitState = mStateActive;
            mStateDead.DebugColor = Color.Transparent;

            PrimaryDebugColor = new Color(0.0f, 1.0f, 0.0f, 0.25f);
        }

        public void Init()
        {
            Systems.WeaponManager weaponMan = Zone.WeaponManager;
            mArsenal[0] = weaponMan.Create(MapData.Weapon.Design.Pistol);
            mArsenal[1] = weaponMan.Create(MapData.Weapon.Design.Shotgun);
            mArsenal[2] = weaponMan.Create(MapData.Weapon.Design.Minigun);

            for (int i = 0; i < kNumWeaponSlots; ++i)
            {
                mArsenal[i].Owner = this;
            }

            mCurrentWeaponIdx = 0;

            mStateMachine.CurrentState = mStateActive;

            MaxHealth = kPlayerHealth;
            Health = kPlayerHealth;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            mStateMachine.Update(ref frameTime, updateToken);
        }

        public void AddToBin(Bin bin)
        {
            AddToBin(bin, GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, BinLayers.kPlayerEntity);
        }


        public void RemoveFromBin()
        {
            RemoveFromBin(Zone.Bin, BinLayers.kPlayerEntity);
        }


        public void UpdateBinEntry()
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Bin bin = Zone.Bin;

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

            if (mStateMachine.CurrentState != mStateDying)
            {
                // Take damage from the bullet
                Health -= damage;
                if (Health <= 0.0f)
                {
                    Health = 0.0f;

                    mStateMachine.CurrentState = mStateDying;
                }
            }
        }


        //public void OnExplosionEvent(ref Explosion explosion, Vector2 force)
        //{
        //    AddForce(force);
        //}


        public void UpdateInput()
        {
            // Handle weapon cycling
            if (mInput.IsButtonPressed(Buttons.RightShoulder))
            {
                ++mCurrentWeaponIdx;
                if (mCurrentWeaponIdx >= kNumWeaponSlots)
                {
                    mCurrentWeaponIdx = 0;
                }
            }

            if (mInput.IsButtonPressed(Buttons.LeftShoulder))
            {
                --mCurrentWeaponIdx;
                if (mCurrentWeaponIdx < 0)
                {
                    mCurrentWeaponIdx = kNumWeaponSlots - 1;
                }
            }

            mCurrentWeapon = mArsenal[mCurrentWeaponIdx];

            if (mCurrentWeapon.AmmoInClip < mCurrentWeapon.Parameters.m_clipSize)
            {
                if (mInput.IsButtonPressed(Buttons.X) || (mCurrentWeapon.AmmoInClip == 0))
                {
                    mCurrentWeapon.Reload();
                }
            }

            mMovementInputVector = mInput.GetLeftStick();
            float len = mMovementInputVector.Length();

            mFacingInputVector = mInput.GetRightStick();
            mTriggerState = mInput.GetRightTrigger();
        }


        internal void UpdateMovement(ref FrameTime frameTime)
        {
            const float kMaxSpeed = 130.0f;

            // If the player has a facing input, use it...
            if (mFacingInputVector.LengthSquared() > 0.0f)
            {
                FacingDirection = Vector2.Normalize(mFacingInputVector);
            }
            // If they're moving, update it from the movement vector
            else if (mMovementVector.LengthSquared() > 0.0f)
            {
                FacingDirection = Vector2.Normalize(mMovementVector);
            }


            Vector2 dMovement = mMovementInputVector - mMovementVector;
            mMovementVector += dMovement * 0.5f;

            Vector2 movementVectorNormalised = Vector2.Normalize(mMovementVector);

            float dotMovementFacing = Vector2.Dot(movementVectorNormalised, FacingDirection);

            float maxSpeedMod = 1.0f;


            // Back footing
            if (dotMovementFacing < -0.35f)
                maxSpeedMod = 0.77f;
            // Strafing
            else if (dotMovementFacing < 0.35f)
                maxSpeedMod = 0.85f;

            Vector2 newPosition = GetPosition() + ((mMovementVector * maxSpeedMod * kMaxSpeed) * frameTime.Dt);

            SetPosition(newPosition);
        }


        internal void UpdateWeapon(ref FrameTime frameTime)
        {
            if (mCurrentWeapon != null)
            {
                mCurrentWeapon.Update(ref frameTime, mTriggerState, GetPosition(), FacingAngle);

                AddForce(mCurrentWeapon.Recoil);
            }
        }

        internal void Kill()
        {
            RemoveFromBin(Zone.Bin, mBinLayer);
        }

        internal void Spawn()
        {
            AddToBin(Zone.Bin);
            Health = MaxHealth;
        }


        #region IWeaponUser

        public Weapon CurrentWeapon
        {
            get { return mCurrentWeapon; }
            set
            {
                mCurrentWeapon = value;
                throw new System.Exception("It's not possible to set the player's weapon externally!");
            }
        }

        public Flags BulletGroupMembership { get { return new Flags((int)EntityGroups.PlayerBullets); } }

        #endregion


        #region IStateMachineOwner

        public StateMachine StateMachine { get { return mStateMachine; } }

        #endregion
    }
}
