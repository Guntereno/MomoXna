using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using Momo.Core.Triggers;
using Momo.Debug;

using TestGame.Ai.AiEntityStates;
using TestGame.Objects;
using TestGame.Weapons;
using Momo.Core.StateMachine;



namespace TestGame.Entities
{
    public class AiEntity : LivingGameEntity, IWeaponUser, IStateMachineOwner
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private EntitySensoryData mSensoryData = new EntitySensoryData((float)Math.PI, 500.0f, 150.0f);

        #region State Machine
        //protected Ai.AiEntityStates.TimedState mStateStunned = null;
        protected Ai.AiEntityStates.TimedState mStateDying = null;
        #endregion

        private int mOccludingBinLayer = -1;
        private int mObstructionBinLayer = -1;

        private Weapon mWeapon = null;
        private Trigger mDeathTrigger = null;


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties

        public EntitySensoryData SensoryData        { get { return mSensoryData; } }

        public Weapon CurrentWeapon
        {
            get { return mWeapon; }
            set { mWeapon = value; }
        }

        public int OccludingBinLayer
        {
            get { return mOccludingBinLayer; }
            set { mOccludingBinLayer = value; }
        }

        public int ObstructionBinLayer
        {
            get { return mObstructionBinLayer; }
            set { mObstructionBinLayer = value; }
        }

        public StateMachine StateMachine { get; private set; }

        public virtual Flags BulletGroupMembership { get { return new Flags((int)EntityGroups.AllBullets); } }

        #endregion


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public AiEntity(GameWorld world): base(world)
        {
            Random random = World.Random;

            FacingAngle = (float)random.NextDouble() * ((float)Math.PI * 2.0f);

            ContactRadiusInfo = new RadiusInfo(16.0f);
            Mass = ContactRadiusInfo.Radius * 2.0f;

            CollidableGroupInfo.GroupMembership = new Flags((int)EntityGroups.Enemies);
            CollidableGroupInfo.CollidesWithGroups = new Flags((int)EntityGroups.AllEntities);

            OccludingBinLayer = BinLayers.kBoundaryOcclusionSmall;
            ObstructionBinLayer = BinLayers.kBoundaryObstructionSmall;

            StateMachine = new StateMachine(this);

            //mStateStunned = new Ai.AiEntityStates.TimedState(this);
            //mStateStunned.Length = 0.5f;

            mStateDying = new Ai.AiEntityStates.TimedState(this);
            mStateDying.DebugColor = Color.Gray;
            mStateDying.TimeInState = 1.5f;
        }

        public override void ResetItem()
        {
            base.ResetItem();

            StateMachine.CurrentState = null;
            mDeathTrigger = null;
            mWeapon = null;
        }

        public void SetDeathTrigger(Trigger trigger)
        {
            mDeathTrigger = trigger;
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            base.Update(ref frameTime, updateToken);

            mSensoryData.Update(ref frameTime);

            StateMachine.Update(ref frameTime, updateToken);

            //if (StateMachine.CurrentState == null)
            //    Kill();
        }


        // Temporary until we work out how the player visually will update the aiEntity
        public void UpdateSensoryData(Players.PlayerEntity[] players)
        {
            for(int i = 0; i < players.Length; ++i)
            {
                mSensoryData.UpdateSensoryData(this, players);
            }
        }


        public void AddToBin(Bin bin)
        {
            AddToBin(bin, GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, mBinLayer);
        }
        

        public void RemoveFromBin()
        {
            RemoveFromBin(mBinLayer);
        }


        public void UpdateBinEntry()
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Bin bin = GetBin();

            GetBinRegion(ref prevBinRegion);
            bin.GetBinRegionFromCentre(GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, mBinLayer);

            SetBinRegion(curBinRegion);
        }


        public override void OnCollisionEvent(ref IDynamicCollidable collidable)
        {

        }


        public override void OnCollisionEvent(ref BulletEntity bullet)
        {
            if (bullet.CollidableGroupInfo.GroupMembership.IsFlagSet((int)EntityGroups.PlayerBullets))
            {
                float damage = bullet.Params.m_damage;
                Vector2 direction = bullet.PositionDifferenceFromLastFrame;
                direction.Normalize();

                AddForce(direction * (damage * 500.0f));


                if (StateMachine.CurrentState != mStateDying)
                {
                    // Take damage from the bullet
                    Health -= damage;
                    if (Health <= 0.0f)
                    {
                        Health = 0.0f;

                        StateMachine.CurrentState = mStateDying;
                    }
                    //else
                    //{
                    //    StateMachine.CurrentState = mStateStunned;
                    //}
                }
            }
        }


        public void OnExplosionEvent(ref Explosion explosion, Vector2 force)
        {
            AddForce(force);
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            base.DebugRender(debugRenderer);

            //if (StateMachine.CurrentState != null)
            //    m_currentState.DebugRender(debugRenderer);
        }

        internal virtual void Kill()
        {
            if (mDeathTrigger != null)
            {
                mDeathTrigger.Activate();
                mDeathTrigger = null;
            }

            if (mWeapon != null)
            {
                mWeapon.DestroyItem();
            }

            DestroyItem();
        }
    }
}
