using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using TestGame.Systems;
using Momo.Maths;
using Momo.Core.Triggers;
using TestGame.Entities;

namespace TestGame.Events
{
    public class SpawnEvent : Event, ITriggerListener
    {
        public static readonly int kMaxSpawns = 8;

        private MapData.SpawnEventData m_spawnData = null;

        private int m_spawnCounter = 0;
        private float m_spawnTimer = 0.0f;

        private SpawnPoint[] m_ownedSpawnPoints = new SpawnPoint[kMaxSpawns];

        private Trigger m_killTrigger = null;
        private int m_killCount = 0;

        public SpawnEvent(GameWorld world, MapData.EventData data)
            : base(world, data)
        {
            string deathTriggerName = data.GetName() + "Kill";
            m_killTrigger = world.GetTriggerManager().RegisterTrigger(deathTriggerName);
            m_killTrigger.RegisterListener(this);
        }


        public override void Begin()
        {
            base.Begin();

            System.Diagnostics.Debug.Assert(GetData() != null);
            m_spawnData = (MapData.SpawnEventData)(GetData());

            m_spawnCounter = m_spawnData.GetSpawnCount();
            System.Diagnostics.Debug.Assert(m_spawnCounter <= kMaxSpawns);

            m_spawnTimer = 0.0f; // Ensure a spawn on the next frame
        }

        public override void Update(ref FrameTime frameTime)
        {
            if (!GetIsActive())
                return;

            if (m_spawnCounter > 0)
            {
                m_spawnTimer -= frameTime.Dt;
                if (m_spawnTimer <= 0.0f)
                {
                    SpawnPoint spawnPoint = GetWorld().GetSpawnPointManager().GetNextSpawnPoint();
                    if (spawnPoint != null)
                    {
                        m_ownedSpawnPoints[m_spawnCounter-1] = spawnPoint;
                        spawnPoint.TakeOwnership(this);
                        SpawnEnemy(spawnPoint);
                        m_spawnTimer = m_spawnData.GetSpawnDelay();
                    }
                }
            }
            else if (m_killCount >= m_spawnData.GetSpawnCount())
            {
                for(int i=0; i<kMaxSpawns; ++i)
                {
                    if(m_ownedSpawnPoints[i] != null)
                    {
                        m_ownedSpawnPoints[i].RelinquishOwnership(this);
                        m_ownedSpawnPoints[i] = null;
                    }
                }
                End();
            }
        }

        private void SpawnEnemy(SpawnPoint spawnPoint)
        {
            EnemyManager enemyManager = GetWorld().GetEnemyManager();
            AiEntity enemy = enemyManager.Create(spawnPoint.GetData().GetPosition());
            enemy.SetDeathTrigger(m_killTrigger);

            --m_spawnCounter;
        }

        // --------------------------------------------------------------------
        // -- ITriggerListener interface implementation
        // --------------------------------------------------------------------
        public void OnReceiveTrigger(Trigger trigger)
        {
            if (trigger == m_killTrigger)
            {
                ++m_killCount;
            }
        }
    }
}
