using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public class PathRoute
    {
        private bool m_valid = false;

        private Vector2 m_startPosition = Vector2.Zero;
        private Vector2 m_endPosition = Vector2.Zero;

        private PathNode m_startNode = null;
        private PathNode m_endNode = null;


        public void SetPathInfo(Vector2 startPos, Vector2 endPos, PathNode startNode, PathNode endNode)
        {
            m_startPosition = startPos;
            m_endPosition = endPos;

            m_startNode = startNode;
            m_endNode = endNode;

            m_valid = true;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            Color nodeFillColor = new Color(1.0f, 0.2f, 1.0f, 0.5f);
            Color nodeOutlineColor = new Color(0.0f, 0.25f, 0.5f, 0.5f);
            Color nodeConnectionLine = new Color(1.0f, 0.25f, 1.0f, 0.5f);

            if (m_valid)
            {
                debugRenderer.DrawCircle(m_startPosition, 15.0f, nodeFillColor, nodeOutlineColor, true, 4.0f, 16);
                debugRenderer.DrawCircle(m_endPosition, 15.0f, nodeFillColor, nodeOutlineColor, true, 4.0f, 16);


                if (m_startNode != null && m_endNode != null)
                {
                    debugRenderer.DrawFilledLine(m_startPosition, m_startNode.GetPosition(), nodeConnectionLine, 5.0f);
                    debugRenderer.DrawCircle(m_startNode.GetPosition(), 10.0f, nodeFillColor, nodeOutlineColor, true, 4.0f, 16);

                    debugRenderer.DrawFilledLine(m_endPosition, m_endNode.GetPosition(), nodeConnectionLine, 5.0f);
                    debugRenderer.DrawCircle(m_endNode.GetPosition(), 10.0f, nodeFillColor, nodeOutlineColor, true, 4.0f, 16);
                }
                else
                {
                    debugRenderer.DrawFilledLine(m_startPosition, m_endPosition, nodeConnectionLine, 5.0f);
                }


                //if (m_nextRouteNode != null)
                //    m_nextRouteNode.DebugRender(m_fromNode, debugRenderer);
            }
        }
    }
}
