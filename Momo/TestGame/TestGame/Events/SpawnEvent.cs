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

        private Trigger m_killTrigger = null;
        private int m_killCount = 0;

        public SpawnEvent(GameWorld world, MapData.EventData data)
            : base(world, data)
        {
            string deathTriggerName = data.GetName() + "Kill";
            m_killTrigger = world.TriggerManager.RegisterTrigger(deathTriggerName);
            m_killTrigger.RegisterListener(this);
        }


        public override void Begin()
        {
            base.Begin();

            System.Diagnostics.Debug.Assert(GetData() != null);
            m_spawnData = (MapData.SpawnEventData)(GetData());

            m_spawnCounter = 0;

            m_spawnTimer = 0.0f; // Ensure a spawn on the next frame
        }

        public override void Update(ref FrameTime frameTime)
        {
            if (!GetIsActive())
                return;

            if (m_spawnCounter < m_spawnData.GetEnemies().Length)
            {
                m_spawnTimer -= frameTime.Dt;
                if (m_spawnTimer <= 0.0f)
                {
                    SpawnPoint spawnPoint = GetWorld().SpawnPointManager.GetNextSpawnPoint();
                    if (spawnPoint != null)
                    {
                        AiEntity enemy = SpawnEnemy(spawnPoint);
                        enemy.TakeOwnershipOf(spawnPoint);
                        m_spawnTimer = m_spawnData.GetSpawnDelay();
                    }
                }
            }
            else if (m_killCount >= m_spawnData.GetEnemies().Length)
            {
                End();
            }
        }

        private AiEntity SpawnEnemy(SpawnPoint spawnPoint)
        {
            EnemyManager enemyManager = GetWorld().EnemyManager;
            MapData.EnemyData enemyData = m_spawnData.GetEnemies()[m_spawnCounter];
            AiEntity enemy = enemyManager.Create(enemyData, spawnPoint.GetData().GetPosition());
            enemy.SetDeathTrigger(m_killTrigger);

            ++m_spawnCounter;

            return enemy;
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
