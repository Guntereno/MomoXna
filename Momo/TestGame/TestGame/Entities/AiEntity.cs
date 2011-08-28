using System;
using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using TestGame.Objects;
using TestGame.Ai.States;

namespace TestGame.Entities
{
    public class AiEntity : DynamicGameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private float m_turnVelocity = 0.0f;
        private EntitySensoryData m_sensoryData = new EntitySensoryData((float)Math.PI, 400.0f, 150.0f);

        private State m_currentState = null;

        private RandomWanderState m_stateRandomWander = null;
        private ChaseState m_stateChase = null;
        private StunnedState m_stateStunned = null;
        private DyingState m_stateDying = null;


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

            SetContactRadiusInfo(new RadiusInfo(9.0f + ((float)random.NextDouble() * 6.0f)));
            SetMass(GetContactRadiusInfo().Radius * 0.5f);

            DebugColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

            m_stateRandomWander = new RandomWanderState(this);
            m_stateChase = new ChaseState(this);
            m_stateStunned = new StunnedState(this);
            m_stateDying = new DyingState(this);

            m_stateRandomWander.Init(m_stateChase);
            m_stateChase.Init(m_stateRandomWander);
            m_stateStunned.Init(m_stateRandomWander);
        }


        public void Init()
        {
            m_currentState = m_stateChase;
        }


        public override void Update(ref FrameTime frameTime)
        {
            base.Update(ref frameTime);

            m_sensoryData.Update(ref frameTime);

            if (m_currentState != null)
            {
                m_currentState.Update(ref frameTime);
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
            AddForce(bullet.GetVelocity() * 50.0f);


            if (m_currentState != m_stateDying)
            {
                // Take damage from the bullet
                m_health -= bullet.GetParams().m_damage;
                if (m_health < 0.0f)
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

        public float GetTurnVelocity() { return m_turnVelocity; }
        public void SetTurnVelocity(float value) { m_turnVelocity = value; }

        internal void Kill()
        {
            DestroyItem();
        }
    }
}
