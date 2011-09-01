using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public struct PathConnection
    {
        internal PathNode m_toNode;
        internal PathNode m_fromNode;

        //internal float m_distanceEstimate;



        public PathConnection(PathNode toNode, PathNode fromNode/*, float distanceEstimate*/)
        {
            m_toNode = toNode;
            m_fromNode = fromNode;

            //m_distanceEstimate = distanceEstimate;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(m_fromNode.m_position, m_toNode.m_position, new Color(0.0f, 1.0f, 1.0f, 0.3f), 1.0f);
        }
    }
}
