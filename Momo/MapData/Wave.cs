using Microsoft.Xna.Framework;
using Momo.Core;

namespace MapData
{
    public class SpawnGroupData
    {
        private SpawnPoint[] m_enemies;

        Vector2 m_center;
        RadiusInfo m_radius;

        public SpawnGroupData(SpawnPoint[] enemies, Vector2 center, RadiusInfo radius)
        {
            m_enemies = enemies;
            m_center = center;
            m_radius = radius;
        }

        public SpawnPoint[] GetSpawnPoints() { return m_enemies; }
        public Vector2 GetCenter() { return m_center; }
        public RadiusInfo GetRadius() { return m_radius; }
    }
}
