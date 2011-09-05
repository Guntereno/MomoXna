using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapData
{
    public struct Wave
    {
        private Enemy[] m_enemies;

        public Wave(Enemy[] enemies)
        {
            m_enemies = enemies;
        }

        public Enemy[] GetEnemies() { return m_enemies; }
    }
}
