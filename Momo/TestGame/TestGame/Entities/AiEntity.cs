using System;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.Collision2D;
using Momo.Core.Spatial;
using Momo.Core.Pathfinding;

using Momo.Debug;

using TestGame.Objects;
using TestGame.Ai.States;



namespace TestGame.Entities
{
    public class AiEntity : DynamicGameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private static readonly int kUpdatePathFrameFrequency = 3;


        private float m_turnVelocity = 0.0f;
        private EntitySensoryData m_sensoryData = new EntitySensoryData((float)Math.PI, 500.0f, 150.0f);

        private State m_currentState = null;

        private RandomWanderState m_stateRandomWander = null;
        private ChaseState m_stateChase = null;
        private StunnedState m_stateStunned = null;
        private DyingState m_stateDying = null;

        private PathRoute m_routeToPlayer = null;



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

            SetContactRadiusInfo(new RadiusInfo(13.0f + ((float)random.NextDouble() * 3.0f)));
            SetMass(GetContactRadiusInfo().Radius * 0.5f);

            SetOccludingBinLayer(BinLayers.kBoundaryExtrudedSmallUnit);

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


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            base.Update(ref frameTime, updateToken);

            m_sensoryData.Update(ref frameTime);


            if (!m_sensoryData.SensePlayer)
            {
                PathNode myPathNode = null;
                PathNode goalPathNode = null;

                PathFindingHelpers.GetClosestPathNode(GetPosition(), GetBin(), BinLayers.kPathNodes, BinLayers.kBoundaryExtrudedSmallUnit, ref myPathNode);
                PathFindingHelpers.GetClosestPathNode(GetWorld().GetPlayerManager().GetAveragePosition(), GetBin(), BinLayers.kPathNodes, BinLayers.kBoundaryExtrudedSmallUnit, ref goalPathNode);

                //System.Diagnostics.Debug.Assert(myPathNode != null);
                //System.Diagnostics.Debug.Assert(goalPathNode != null);

                if (myPathNode != null && goalPathNode != null)
                {
                    bool cacheOnly = ((updateToken % kUpdatePathFrameFrequency) != 0);
                    GetWorld().GetPathRouteManager().GetPathRoute(myPathNode, goalPathNode, ref m_routeToPlayer, cacheOnly);
                }
            }


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

            if(m_routeToPlayer != null)
                m_routeToPlayer.DebugRender(debugRenderer);
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
            GetWorld().GetEnemyManager().IncrementKillCount();
            DestroyItem();
        }
    }
}
