using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public struct PathRegionConnection
    {
        internal PathRegion m_fromRegion;
        internal PathRegion m_toRegion;

        internal PathNode[] m_fromNodes;
        internal PathNode[] m_toNodes;

        internal float m_distanceEstimate;



        public PathRegionConnection(PathRegion fromRegion, PathNode[] fromNodes, PathRegion toRegion, PathNode[] toNodes, float distanceEstimate)
        {
            m_fromRegion = fromRegion;
            m_toRegion = toRegion;

            m_fromNodes = fromNodes;
            m_toNodes = toNodes;

            m_distanceEstimate = distanceEstimate;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            if (m_fromNodes != null && m_toNodes != null)
            {
                for (int i = 0; i < m_fromNodes.Length; ++i)
                {
                    PathNode node1 = m_fromNodes[i];
                    PathNode node2 = m_toNodes[i];

                    debugRenderer.DrawFilledLine(node1.m_position, node2.m_position, new Color(0.0f, 1.0f, 0.0f, 0.15f), 8.0f, 4);
                }
            }
        }
    }
}
