using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public struct PathRegionConnection
    {
        internal PathRegion m_region1;
        internal PathRegion m_region2;

        internal PathNode[] m_regionNodes1;
        internal PathNode[] m_regionNodes2;

        internal float m_distanceEstimate;



        public PathRegionConnection(PathRegion region1, PathNode[] regionNodes1, PathRegion region2, PathNode[] regionNodes2, float distanceEstimate)
        {
            m_region1 = region1;
            m_region2 = region2;

            m_regionNodes1 = regionNodes1;
            m_regionNodes2 = regionNodes2;

            m_distanceEstimate = distanceEstimate;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_regionNodes1.Length; ++i)
            {
                PathNode node1 = m_regionNodes1[i];
                PathNode node2 = m_regionNodes2[i];

                debugRenderer.DrawFilledLine(node1.m_position, node2.m_position, new Color(0.0f, 1.0f, 0.0f, 0.5f), 3.0f);
            }
        }
    }
}
