using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public class PathNode
    {
        internal Vector2 m_position;
        internal float m_radius;



        public PathNode(Vector2 position, float radius)
        {
            m_position = position;
            m_radius = radius;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledCircle(m_position, m_radius, new Color(0.0f, 1.0f, 1.0f, 0.50f));
        }
    }
}
