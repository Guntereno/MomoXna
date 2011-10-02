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

        public EnemyData(Species type, Weapon.Design weapon)
        {
            m_type = type;
            m_weapon = weapon;
        }

        private Species m_type;
        private Weapon.Design m_weapon;

        public Species GetSpecies() { return m_type; }
        public Weapon.Design GetWeapon() { return m_weapon; }
    }
}
