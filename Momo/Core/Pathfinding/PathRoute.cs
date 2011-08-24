using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public class PathRoute
    {
        private PathRegion m_fromRegion = null;
        private PathNode m_fromNode = null;
        private PathRouteNode m_nextRouteNode = null;


        private void CalculatePath(PathRegion fromRegion, PathNode fromNode, PathRegion toRegion, PathNode toNode)
        {
            m_fromRegion = fromRegion;
            m_fromNode = fromNode;


            //
            // Calculate the region route seperately in its own function.
            //

            //for (int i = 0; i < m_fromRegion.m_connections.Length; ++i)
            //{
            //    PathRegionConnection connection = m_fromRegion.m_connections[i];

            //    PathRegion connectedRegion = connection.m_region1;

            //    if (connectedRegion == m_fromRegion)
            //        connectedRegion = connection.m_region2;




            //}
        }


        public void DebugRender(PathNode fromNode, DebugRenderer debugRenderer)
        {
            debugRenderer.DrawOutlineCircle(m_fromNode.m_position, 15.0f, new Color(0.0f, 1.0f, 1.0f, 0.5f), 5.0f);

            if (m_nextRouteNode != null)
                m_nextRouteNode.DebugRender(m_fromNode, debugRenderer);
        }
    }
}
