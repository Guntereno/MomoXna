using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public class PathRouteNode
    {
        internal PathNode m_toNode;
        internal PathRouteNode m_nextRouteNode = null;


        public PathRouteNode(PathNode toNode)
        {
            m_toNode = toNode;
        }


        public void DebugRender(PathNode fromNode, DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(fromNode.m_position, m_toNode.m_position, new Color(0.0f, 1.0f, 1.0f, 0.5f), 1.0f);

            if (m_nextRouteNode != null)
                m_nextRouteNode.DebugRender(m_toNode, debugRenderer);
        }
    }
}
