﻿using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using Momo.Core.Triggers;
using Momo.Debug;

using TestGame.Ai.States;
using TestGame.Objects;
using TestGame.Weapons;



namespace TestGame.Entities
{
    public class AiEntity : LivingGameEntity, IWeaponUser
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private EntitySensoryData m_sensoryData = new EntitySensoryData((float)Math.PI, 500.0f, 150.0f);

        #region State Machine
        private State m_currentState = null;
        protected StunnedState m_stateStunned = null;
        protected DyingState m_stateDying = null;
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

            SecondaryDebugColor = new Color(1.0f, 0.0f, 0.0f);
        }

        public virtual void Init()
        {
            m_currentState = null;
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

            if (m_currentState != null)
            {
                m_currentState.Update(ref frameTime, updateToken);
            }
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


                if (m_currentState != m_stateDying)
                {
                    // Take damage from the bullet
                    Health -= damage;
                    if (Health <= 0.0f)
                    {
                        Health = 0.0f;

                        SetCurrentState(m_stateDying);
                    }
                    else
                    {
                        SetCurrentState(m_stateStunned);
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

            if (m_currentState != null)
                m_currentState.DebugRender(debugRenderer);
        }


        public void SetCurrentState(State state)
        {
            if (m_currentState != null)
            {
                m_currentState.OnExit();
            }

            m_currentState = state;

            if (m_currentState != null)
            {
                m_currentState.OnEnter();
            }
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
