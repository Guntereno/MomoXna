using System;
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
    public class AiEntity : DynamicGameEntity, IWeaponUser
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private EntitySensoryData m_sensoryData = new EntitySensoryData((float)Math.PI, 500.0f, 150.0f);

        private State m_currentState = null;

        #region State Machine
        protected StunnedState m_stateStunned = null;
        protected DyingState m_stateDying = null;
        #endregion

        private int m_occludingBinLayer = -1;
        private int m_obstructionBinLayer = -1;

        private MapData.EnemyData m_data = null;

        private Trigger m_deathTrigger = null;

        //private SensedObject m_sensedPlayer = null;

        private Weapon m_weapon = null;

        private SpawnPoint m_ownedSpawnPoint = null;

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public AiEntity(GameWorld world): base(world)
        {
            Random random = GetWorld().GetRandom();

            FacingAngle = (float)random.NextDouble() * ((float)Math.PI * 2.0f);

            SetContactRadiusInfo(new RadiusInfo(16.0f));
            SetMass(GetContactRadiusInfo().Radius * 0.5f);

            SetOccludingBinLayer(BinLayers.kBoundaryOcclusionSmall);
            SetObstructionBinLayer(BinLayers.kBoundaryObstructionSmall);

            DebugColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }

        public virtual void Init(MapData.EnemyData data)
        {
            m_data = data;

            m_currentState = null;
            m_deathTrigger = null;
            //m_sensedPlayer = null;
            m_weapon = null;
            m_ownedSpawnPoint = null;
        }

        public EntitySensoryData SensoryData
        {
            get { return m_sensoryData; }
        }

        //public SensedObject SensedPlayer
        //{
        //    get { return m_sensedPlayer; }
        //}

        public MapData.EnemyData GetData() { return m_data; }

        public void TakeOwnershipOf(SpawnPoint spawnPoint)
        {
            System.Diagnostics.Debug.Assert(m_ownedSpawnPoint == null);
            m_ownedSpawnPoint = spawnPoint;
            spawnPoint.TakeOwnership(this);
        }

        public void SetDeathTrigger(Trigger trigger)
        {
            m_deathTrigger = trigger;
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            base.Update(ref frameTime, updateToken);

            m_sensoryData.Update(ref frameTime);

            //bool playerSensed = m_sensoryData.GetSensedObject(SensedType.kSeePlayer, ref m_sensedPlayer);

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


        public override void OnCollisionEvent(ref BulletEntity bullet)
        {
            if ( (bullet.GetFlags() & BulletEntity.Flags.HarmsEnemies) == BulletEntity.Flags.HarmsEnemies )
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


        public int GetObstructionBinLayer()
        {
            return m_obstructionBinLayer;
        }


        public void SetObstructionBinLayer(int layer)
        {
            m_obstructionBinLayer = layer;
        }


        internal void Kill()
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

            if (m_ownedSpawnPoint != null)
            {
                m_ownedSpawnPoint.RelinquishOwnership(this);
                m_ownedSpawnPoint = null;
            }

            GetWorld().GetCorpseManager().Create(this);

            GetWorld().GetEnemyManager().IncrementKillCount();
            DestroyItem();
        }

        // --------------------------------------------------------------------
        // -- IWeaponUser interface implementation
        // --------------------------------------------------------------------
        public BulletEntity.Flags GetBulletFlags()
        {
            return BulletEntity.Flags.HarmsPlayer;
        }

        public Weapon GetCurrentWeapon() { return m_weapon; }
        public void SetCurrentWeapon(Weapon value) { m_weapon = value; }
    }
}
