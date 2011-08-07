using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public struct PathConnection
    {
        internal PathNode m_toNode;
       
        internal float m_distanceEstimate;



        public PathConnection(PathNode toNode, float distanceEstimate)
        {
            m_toNode = toNode;

            m_distanceEstimate = distanceEstimate;
        }


        public void DebugRender(PathNode fromNode, DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(fromNode.m_position, m_toNode.m_position, new Color(0.0f, 1.0f, 0.0f, 0.3f), 1.0f);
        }
    }
}
