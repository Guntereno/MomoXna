using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core;
using TestGame.Systems;
using Momo.Maths;

namespace TestGame.Events
{
    public class SpawnEvent : Event
    {
        public static readonly int kMaxSpawns = 8;

        MapData.SpawnGroup m_spawnGroup = null;

        MapData.SpawnEventData m_spawnData = null;

        int m_spawnCounter = 0;
        float m_spawnTimer = 0.0f;

        int[] m_spawnOrder = new int[kMaxSpawns];

        public SpawnEvent(GameWorld world, MapData.EventData data)
            : base(world, data)
        {
        }


        public override void Begin()
        {
            base.Begin();

            // For debugging simply use the first spawn group
            m_spawnGroup = GetWorld().GetMap().SpawnGroups[0];

            GenerateSpawnOrder();

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

            m_spawnTimer -= frameTime.Dt;
            if (m_spawnTimer <= 0.0f)
            {
                SpawnEnemy();
                m_spawnTimer = m_spawnData.GetSpawnDelay();

                if (--m_spawnCounter <= 0)
                {
                    End();
                }
            }
        }

        private void SpawnEnemy()
        {
            MapData.SpawnPoint[] spawnPoints = m_spawnGroup.GetSpawnPoints();
            int pointIdx = m_spawnOrder[m_spawnData.GetSpawnCount() - m_spawnCounter];
            MapData.SpawnPoint spawnPoint = spawnPoints[pointIdx];

            EnemyManager enemyManager = GetWorld().GetEnemyManager();
            enemyManager.Create(spawnPoint.GetPosition());
        }
    }
}
