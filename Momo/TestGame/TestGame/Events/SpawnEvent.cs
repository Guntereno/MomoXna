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

        private SpawnGroup m_spawnGroup = null;

        private MapData.SpawnEventData m_spawnData = null;

        private int m_spawnCounter = 0;
        private float m_spawnTimer = 0.0f;

        private int[] m_spawnOrder = new int[kMaxSpawns];

        private Trigger m_killTrigger = null;
        private int m_killCount = 0;

        public SpawnEvent(GameWorld world, MapData.EventData data)
            : base(world, data)
        {
            string deathTriggerName = data.GetName() + "Kill";
            m_killTrigger = world.GetTriggerManager().GetTrigger(deathTriggerName);
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

        private void GenerateSpawnOrder()
        {
            for (int i = 0; i < kMaxSpawns; ++i)
            {
                m_spawnOrder[i] = i;
            }
            RandUtil.Shuffle<int>(m_spawnOrder, GetWorld().GetRandom());
        }

        public override void Update(ref FrameTime frameTime)
        {
            if (!GetIsActive())
                return;

            if (m_spawnGroup == null)
            {
                // For debugging simply use the first spawn group
                m_spawnGroup = GetWorld().GetSpawnPointManager().GetNextSpawnGroup();

                if (m_spawnGroup == null)
                {
                    // Try again next frame
                    return;
                }

                m_spawnGroup.TakeOwnership(this);
                GenerateSpawnOrder();
            }

            if (m_spawnCounter > 0)
            {
                m_spawnTimer -= frameTime.Dt;
                if (m_spawnTimer <= 0.0f)
                {
                    SpawnEnemy();
                    m_spawnTimer = m_spawnData.GetSpawnDelay();
                }
            }
            else if (m_killCount >= m_spawnData.GetSpawnCount())
            {
                m_spawnGroup.RelinquishOwnership(this);
                End();
            }
        }

        private void SpawnEnemy()
        {
            MapData.SpawnPoint[] spawnPoints = m_spawnGroup.GetData().GetSpawnPoints();
            int pointIdx = m_spawnOrder[m_spawnData.GetSpawnCount() - m_spawnCounter];
            MapData.SpawnPoint spawnPoint = spawnPoints[pointIdx];

            EnemyManager enemyManager = GetWorld().GetEnemyManager();
            AiEntity enemy = enemyManager.Create(spawnPoint.GetPosition());
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
