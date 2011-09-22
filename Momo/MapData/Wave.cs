using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapData
{
    public class SpawnGroup
    {
        private SpawnPoint[] m_enemies;

        public SpawnGroup(SpawnPoint[] enemies)
        {
            m_enemies = enemies;
        }

        public SpawnPoint[] GetEnemies() { return m_enemies; }
    }
}
