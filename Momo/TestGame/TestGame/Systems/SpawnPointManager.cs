using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Entities;
using Momo.Core;
using Momo.Debug;

namespace TestGame.Systems
{
    public class SpawnPointManager
    {
        private GameWorld m_world = null;
        private SpawnPoint[] m_spawnPoints = null;


        internal SpawnPointManager(GameWorld world)
        {
            m_world = world;
        }

        internal void LoadSpawnGroups(MapData.Map map)
        {
            // Add the objects
            int count = map.SpawnPoints.Length;
            m_spawnPoints = new SpawnPoint[count];
            for (int i = 0; i < count; ++i)
            {
                m_spawnPoints[i] = new SpawnPoint(m_world, map.SpawnPoints[i]);
            }
        }

        internal SpawnPoint GetNextSpawnPoint()
        {
            if (m_spawnPoints == null)
                return null;

            const float kMaxDistance = 500.0f;
            const float kMaxDistanceSq = kMaxDistance * kMaxDistance;

            const float kMinDistance = 300.0f;
            const float kMinDistanceSq = kMinDistance * kMinDistance;

            float closestValidDistance = float.MaxValue;
            int closestValidIndex = -1;
            for (int i = 0; i < m_spawnPoints.Length; ++i)
            {
                if (m_spawnPoints[i].IsOwned())
                    continue;

                float sqDist = m_spawnPoints[i].GetSquaredDistanceToPlayers();

                if ((sqDist < kMinDistanceSq) || (sqDist > kMaxDistanceSq))
                    continue;

                if (sqDist < closestValidDistance)
                {
                    closestValidDistance = sqDist;
                    closestValidIndex = i;
                }
            }

            if (closestValidIndex != -1)
            {
                return m_spawnPoints[closestValidIndex];
            }
            else
            {
                // Caller will have to try again next frame
                return null;
            }
        }

        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_spawnPoints.Length; ++i)
            {
                m_spawnPoints[i].DebugRender(debugRenderer);
            }
        }
    }

    

}
