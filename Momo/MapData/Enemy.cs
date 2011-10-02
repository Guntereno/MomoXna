using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapData
{
    public class EnemyData
    {
        public enum Species
        {
            Melee = 0,
            Missile = 1
        }

        public EnemyData(Species type)
        {
            m_type = type;
        }

        private Species m_type;

        public Species GetSpecies() { return m_type; }
    }
}
