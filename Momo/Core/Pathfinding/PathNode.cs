using System;

using Microsoft.Xna.Framework;

using Momo.Debug;
using Momo.Core.Spatial;



namespace Momo.Core.Pathfinding
{
    public class PathNode : BinItem
    {
        internal short m_uniqueId = 0;
        internal Vector2 m_position = Vector2.Zero;
        internal float m_radius = 0.0f;

        internal PathConnection[] m_connections = null;
        internal int m_connectionCnt = 0;


        public PathNode(int uniqueId, Vector2 position, float radius, int maxConnections)
        {
            m_uniqueId = (short)uniqueId;
            m_position = position;
            m_radius = radius;

            m_connections = new PathConnection[maxConnections];
            m_connectionCnt = 0;
        }


        public void AddConnection(PathConnection connection)
        {
            m_connections[m_connectionCnt++] = connection;
        }


        public short GetUniqueId()
        {
            return m_uniqueId;
        }

        public override Vector2 GetPosition()
        {
            return m_position;
        }


        public float GetRadius()
        {
            return m_radius;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledCircle(m_position, m_radius, new Color(0.0f, 1.0f, 1.0f, 0.15f));

            for (int i = 0; i < m_connectionCnt; ++i)
            {
                m_connections[i].DebugRender(this, debugRenderer);
            }
        }
    }
}
