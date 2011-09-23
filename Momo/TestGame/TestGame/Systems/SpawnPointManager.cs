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
        GameWorld m_world = null;
        SpawnGroup[] m_spawnGroups = null;

        internal SpawnPointManager(GameWorld world)
        {
            m_world = world;
        }

        internal void LoadSpawnGroups(MapData.Map map)
        {
            // Add the objects
            m_spawnGroups = new SpawnGroup[map.SpawnGroups.Length];
            for (int i = 0; i < map.SpawnGroups.Length; ++i)
            {
                m_spawnGroups[i] = new SpawnGroup(m_world, map.SpawnGroups[i]);
            }
        }

        internal SpawnGroup GetNextSpawnGroup()
        {
            const float kMaxDistance = 500.0f;
            const float kMaxDistanceSq = kMaxDistance * kMaxDistance;

            float closestValidDistance = float.MaxValue;
            int closestValidIndex = -1;
            for (int i = 0; i < m_spawnGroups.Length; ++i)
            {
                if (m_spawnGroups[i].IsOwned())
                    continue;

                float sqDist = m_spawnGroups[i].GetSquaredDistanceToPlayers();

                if ((sqDist < m_spawnGroups[i].GetData().GetRadius().RadiusSq) || (sqDist > kMaxDistanceSq))
                    continue;

                if (sqDist < closestValidDistance)
                {
                    closestValidDistance = sqDist;
                    closestValidIndex = i;
                }
            }

            if (closestValidIndex != -1)
            {
                return m_spawnGroups[closestValidIndex];
            }
            else
            {
                // Caller will have to try again next frame
                return null;
            }
        }

        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_spawnGroups.Length; ++i)
            {
                m_spawnGroups[i].DebugRender(debugRenderer);
            }
        }
    }

    

}
