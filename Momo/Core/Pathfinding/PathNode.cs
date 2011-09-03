using System;

using Microsoft.Xna.Framework;

using Momo.Debug;
using Momo.Core.Spatial;



namespace Momo.Core.Pathfinding
{
    public class PathNode : BinItem
    {
        internal Vector2 m_position;
        internal float m_radius;


        public PathNode(Vector2 position, float radius)
        {
            m_position = position;
            m_radius = radius;
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
        }
    }
}
