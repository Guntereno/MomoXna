using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using TestGame.Systems;

namespace TestGame.Events
{
    public class SpawnEvent : Event
    {
        MapData.SpawnGroup m_spawnGroup = null;

        MapData.SpawnEventData m_spawnData = null;

        int m_spawnCounter = 0;
        float m_spawnTimer = 0.0f;

        
        public SpawnEvent(GameWorld world) : base(world)
        {
        }


        public override void Begin(MapData.EventData data)
        {
            base.Begin(data);

            // For debugging simply use the first spawn group
            m_spawnGroup = GetWorld().GetMap().SpawnGroups[0];

            System.Diagnostics.Debug.Assert(GetData() != null);
            m_spawnData = (MapData.SpawnEventData)(GetData());

            m_spawnCounter = m_spawnData.GetSpawnCount();
            m_spawnTimer = 0.0f; // Ensure a spawn on the next frame
        }

        public override void Update(ref FrameTime frameTime)
        {
            m_spawnTimer -= frameTime.Dt;
            if (m_spawnTimer <= 0.0f)
            {
                SpawnEnemy();
                m_spawnTimer = m_spawnData.GetSpawnDelay();

                if (--m_spawnCounter <= 0)
                {
                    DestroyItem();
                }
            }
        }

        private void SpawnEnemy()
        {
            MapData.SpawnPoint[] spawnPoints = m_spawnGroup.GetEnemies();
            int pointIdx = GetWorld().GetRandom().Next(spawnPoints.Length);
            MapData.SpawnPoint spawnPoint = spawnPoints[pointIdx];

            EnemyManager enemyManager = GetWorld().GetEnemyManager();
            enemyManager.Create(spawnPoint.GetPosition());
        }
    }
}
