using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public class PathConnection
    {
        internal PathNode m_toNode = null;
        internal float m_distance = 0.0f;



        public PathConnection(PathNode toNode, float distance)
        {
            m_toNode = toNode;
            m_distance = distance;
        }


        public void DebugRender(PathNode fromNode, DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledLine(fromNode.m_position, m_toNode.m_position, new Color(0.0f, 1.0f, 1.0f, 0.05f), 5.0f);
        }
    }
}
