using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public class PathRoute
    {
        private PathNode[] m_pathNodes = null;
        private int m_pathNodeCnt = 0;


        public PathRoute(int maxNodes)
        {
            m_pathNodes = new PathNode[maxNodes];
        }


        public void AddNodeToPath(PathNode node)
        {
            m_pathNodes[m_pathNodeCnt++] = node;
        }


        public void Clear()
        {
            for (int i = 0; i < m_pathNodeCnt; ++i)
            {
                m_pathNodes[i] = null;
            }

            m_pathNodeCnt = 0;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            const float kNodeConnectionLineWidth = 3.0f;
            const float kNodeRadius = 10.0f;
            Color nodeFillColor = new Color(1.0f, 0.2f, 1.0f, 0.15f);
            Color nodeConnectionLine = new Color(1.0f, 0.25f, 1.0f, 0.15f);



            Vector2 lastPosition = Vector2.Zero;

            for (int i = 0; i < m_pathNodeCnt; ++i)
            {
                if(i > 0)
                    debugRenderer.DrawFilledLine(lastPosition, m_pathNodes[i].GetPosition(), nodeConnectionLine, kNodeConnectionLineWidth);

                debugRenderer.DrawFilledCircle(m_pathNodes[i].GetPosition(), kNodeRadius, nodeFillColor);

                lastPosition = m_pathNodes[i].GetPosition();
            }

            //debugRenderer.DrawFilledLine(lastPosition, m_endPosition, nodeConnectionLine, kNodeConnectionLineWidth);
        }
    }
}
