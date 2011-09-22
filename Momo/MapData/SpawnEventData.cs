using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapData
{
    public class SpawnEventData : EventData
    {
        private int m_spawnCount;
        private float m_spawnDelay;

        public SpawnEventData(string name, string startTrigger, string endTrigger, int spawnCount, float spawnDelay)
            : base(name, startTrigger, endTrigger)
        {
            m_spawnCount = spawnCount;
            m_spawnDelay = spawnDelay;
        }

        public int GetSpawnCount() { return m_spawnCount; }
        public float GetSpawnDelay() { return m_spawnDelay; }
    }
}
