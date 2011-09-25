﻿using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using Momo.Core.Pathfinding;

using Momo.Debug;

using TestGame.Objects;
using TestGame.Ai.States;
using Momo.Core.Triggers;



namespace TestGame.Entities
{
    public class AiEntity : DynamicGameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private EntitySensoryData m_sensoryData = new EntitySensoryData((float)Math.PI, 500.0f, 150.0f);

        private State m_currentState = null;
        private int m_occludingBinLayer = -1;

        private RandomWanderState m_stateRandomWander = null;
        private FindState m_stateFind = null;
        private ChaseState m_stateChase = null;
        private StunnedState m_stateStunned = null;
        private DyingState m_stateDying = null;

        private Trigger m_deathTrigger = null;

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public EntitySensoryData SensoryData
        {
            get { return m_sensoryData; }
        }


        public AiEntity(GameWorld world): base(world)
        {
            Random random = GetWorld().GetRandom();

            FacingAngle = (float)random.NextDouble() * ((float)Math.PI * 2.0f);

            SetContactRadiusInfo(new RadiusInfo(14.0f + ((float)random.NextDouble() * 3.0f)));
            SetMass(GetContactRadiusInfo().Radius * 0.5f);

            SetOccludingBinLayer(BinLayers.kBoundaryViewSmall);

            DebugColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

            m_stateRandomWander = new RandomWanderState(this);
            m_stateFind = new FindState(this);
            m_stateChase = new ChaseState(this);
            m_stateStunned = new StunnedState(this);
            m_stateDying = new DyingState(this);

            m_stateRandomWander.Init(m_stateChase);
            m_stateFind.Init(m_stateChase);
            m_stateChase.Init(m_stateFind);
            m_stateStunned.Init(m_stateChase);
        }

        public void SetDeathTrigger(Trigger trigger)
        {
            m_deathTrigger = trigger;
        }

        public void Init()
        {
            SetCurrentState(m_stateFind);
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
        public void UpdateSensoryData(PlayerEntity[] players)
        {
            for(int i = 0; i < players.Length; ++i)
            {
                m_sensoryData.UpdateSensoryData(m_position, FacingDirection, players);
            }
        }


        public void AddToBin(Bin bin)
        {
            AddToBin(bin, GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), BinLayers.kAiEntity);
        }
        

        public void RemoveFromBin()
        {
            RemoveFromBin(BinLayers.kAiEntity);
        }


        public void UpdateBinEntry()
        {
            BinRegionUniform prevBinRegion = new BinRegionUniform();
            BinRegionUniform curBinRegion = new BinRegionUniform();
            Bin bin = GetBin();

            GetBinRegion(ref prevBinRegion);
            bin.GetBinRegionFromCentre(GetPosition(), GetContactRadiusInfo().Radius + GetContactDimensionPadding(), ref curBinRegion);

            bin.UpdateBinItem(this, ref prevBinRegion, ref curBinRegion, BinLayers.kAiEntity);

            SetBinRegion(curBinRegion);
        }


        public override void OnCollisionEvent(ref IDynamicCollidable collidable)
        {

        }


        public void OnCollisionEvent(ref BulletEntity bullet)
        {
            float damage = bullet.GetParams().m_damage;
            Vector2 direction = bullet.GetPositionDifferenceFromLastFrame();
            direction.Normalize();

            AddForce(direction * (damage * 500.0f));


            if (m_currentState != m_stateDying)
            {
                // Take damage from the bullet
                m_health -= damage;
                if (m_health <= 0.0f)
                {
                    m_health = 0.0f;

                    SetCurrentState(m_stateDying);
                }
                else
                {
                    SetCurrentState(m_stateStunned);
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

        public String GetCurrentStateName()
        {
            if (m_currentState == null)
            {
                return "";
            }

            return m_currentState.ToString();
        }


        public int GetOccludingBinLayer()
        {
            return m_occludingBinLayer;
        }


        public void SetOccludingBinLayer(int layer)
        {
            m_occludingBinLayer = layer;
        }



        internal void Kill()
        {
            if (m_deathTrigger != null)
            {
                m_deathTrigger.Activate();
                m_deathTrigger = null;
            }

            GetWorld().GetEnemyManager().IncrementKillCount();
            DestroyItem();
        }
    }
}
