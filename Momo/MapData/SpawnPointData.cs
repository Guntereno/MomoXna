using Microsoft.Xna.Framework;

namespace MapData
{
    public class SpawnPointData
    {
        private Vector2 m_position;

        public SpawnPointData(Vector2 position)
        {
            m_position = position;
        }

        public Vector2 GetPosition() { return m_position; }
    }
}
