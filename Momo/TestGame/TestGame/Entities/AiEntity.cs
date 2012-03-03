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
        private EntitySensoryData m_sensoryData = new EntitySensoryData((float)Math.PI, 500.0f, 150.0f);

        #region State Machine
        protected Ai.AiEntityStates.TimedState m_stateStunned = null;
        protected Ai.AiEntityStates.TimedState m_stateDying = null;
        #endregion

        private int m_occludingBinLayer = -1;
        private int m_obstructionBinLayer = -1;

        private Weapon m_weapon = null;
        private Trigger m_deathTrigger = null;


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties

        public EntitySensoryData SensoryData        { get { return m_sensoryData; } }

        public Weapon CurrentWeapon
        {
            get { return m_weapon; }
            set { m_weapon = value; }
        }

        public int OccludingBinLayer
        {
            get { return m_occludingBinLayer; }
            set { m_occludingBinLayer = value; }
        }

        public int ObstructionBinLayer
        {
            get { return m_obstructionBinLayer; }
            set { m_obstructionBinLayer = value; }
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

            m_stateStunned = new Ai.AiEntityStates.TimedState(this);
            m_stateStunned.Length = 0.5f;

            m_stateDying = new Ai.AiEntityStates.TimedState(this);
            m_stateDying.Length = 1.5f;
        }

        public virtual void Init()
        {
            StateMachine.CurrentState = null;
            m_deathTrigger = null;
            m_weapon = null;
        }

        public void SetDeathTrigger(Trigger trigger)
        {
            m_deathTrigger = trigger;
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            base.Update(ref frameTime, updateToken);

            m_sensoryData.Update(ref frameTime);

            StateMachine.Update(ref frameTime, updateToken);

            if (StateMachine.CurrentState == null)
                Kill();
        }


        // Temporary until we work out how the player visually will update the aiEntity
        public void UpdateSensoryData(Players.PlayerEntity[] players)
        {
            for(int i = 0; i < players.Length; ++i)
            {
                m_sensoryData.UpdateSensoryData(this, players);
            }
        }


        public void AddToBin(Bin bin)
        {
            AddToBin(bin, GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, BinLayers.kEnemyEntities);
        }
        

        public void RemoveFromBin()
        {
            RemoveFromBin(BinLayers.kEnemyEntities);
        }


        public void UpdateBinEntry()
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Bin bin = GetBin();

            GetBinRegion(ref prevBinRegion);
            bin.GetBinRegionFromCentre(GetPosition(), ContactRadiusInfo.Radius + ContactDimensionPadding, ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, BinLayers.kEnemyEntities);

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


                if (StateMachine.CurrentState != m_stateDying)
                {
                    // Take damage from the bullet
                    Health -= damage;
                    if (Health <= 0.0f)
                    {
                        Health = 0.0f;

                        StateMachine.CurrentState = m_stateDying;
                    }
                    else
                    {
                        StateMachine.CurrentState = m_stateStunned;
                    }
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
            if (m_deathTrigger != null)
            {
                m_deathTrigger.Activate();
                m_deathTrigger = null;
            }

            if (m_weapon != null)
            {
                m_weapon.DestroyItem();
            }

            DestroyItem();
        }
    }
}
