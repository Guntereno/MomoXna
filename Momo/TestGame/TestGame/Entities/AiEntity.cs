using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using Momo.Core.Triggers;
using Momo.Debug;

using Game.Ai.AiEntityStates;
using Game.Weapons;
using Momo.Core.StateMachine;



namespace Game.Entities
{
    public class AiEntity : LivingGameEntity, IWeaponUser, IStateMachineOwner
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
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
        public AiEntity(Zone zone): base(zone)
        {
            Random random = Zone.Random;

            FacingAngle = (float)random.NextDouble() * ((float)Math.PI * 2.0f);

            ContactRadiusInfo = new RadiusInfo(10.5f + ((float)random.NextDouble() * 1.0f));
            Mass = ContactRadiusInfo.Radius * 2.0f;

            CollidableGroupInfo.GroupMembership = new Flags((int)EntityGroups.Enemies);
            CollidableGroupInfo.CollidesWithGroups = new Flags((int)EntityGroups.AllEntities);

            OccludingBinLayer = BinLayers.kBoundaryOcclusionSmall;
            ObstructionBinLayer = BinLayers.kBoundaryObstructionSmall;

            StateMachine = new StateMachine(this);

            //mStateStunned = new Ai.AiEntityStates.TimedState(this);
            //mStateStunned.Length = 0.5f;

            mStateDying = new Ai.AiEntityStates.TimedState(this);
            mStateDying.DebugColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
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

        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            StateMachine.Update(ref frameTime, updateToken);

            if (StateMachine.CurrentState == null)
                Kill();
        }


        public void AddToBin()
        {
            AddToBin(Zone.Bin, GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, mBinLayer);
        }
        

        public void RemoveFromBin()
        {
            RemoveFromBin(Zone.Bin, mBinLayer);
        }


        public void UpdateBinEntry()
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Bin bin = Zone.Bin;

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

                        PrimaryDebugColor = new Color(0.4f, 0.4f, 0.4f, 0.25f);
                        StateMachine.CurrentState = mStateDying;
                    }
                    //else
                    //{
                    //    StateMachine.CurrentState = mStateStunned;
                    //}
                }
            }
        }


        //public void OnExplosionEvent(ref Explosion explosion, Vector2 force)
        //{
        //    AddForce(force);
        //}


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
