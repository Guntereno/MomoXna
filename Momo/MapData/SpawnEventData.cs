using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapData
{
    public class SpawnEventData : EventData
    {
        private float m_spawnDelay;
        private EnemyData[] m_enemies = null;

        public SpawnEventData(string name, string startTrigger, string endTrigger, float spawnDelay, EnemyData[] enemies)
            : base(name, startTrigger, endTrigger)
        {
            m_spawnDelay = spawnDelay;
            m_enemies = enemies;
        }

        public float GetSpawnDelay() { return m_spawnDelay; }
        public EnemyData[] GetEnemies() { return m_enemies; }
    }
}
