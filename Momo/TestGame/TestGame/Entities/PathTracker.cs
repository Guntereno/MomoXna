using System;

using Microsoft.Xna.Framework;

using Momo.Core.Pathfinding;
using Momo.Core.Spatial;
using Momo.Debug;



namespace TestGame.Entities
{
    public class PathTracker
    {
        private Vector2 m_goalPosition = Vector2.Zero;
        private PathNode m_targetNode = null;
        private int m_targetNodeIdx = -1;

        private float m_searchLinePos = 0.0f;
        private Vector2 m_lastSearchFrom = Vector2.Zero;
        private Vector2 m_lastSearchPos = Vector2.Zero;


        public void Reset()
        {
            m_targetNode = null;
            m_targetNodeIdx = -1;
            m_searchLinePos = 0.0f;
        }


        // Returns true if reach goal.
        // Assumes you can see the first node on the route.
        public bool Track(Vector2 myPosition, Vector2 goal, Bin bin, int occludingBoundaryLayer, ref PathRoute route, out Vector2 outDirection, out float outEstimateDistanceSq)
        {
            int routeNodeCnt = route.GetPathNodeCount();
            
            // If the route has no nodes, then return true, we have 'tracked' to the goal. This should never
            // really happen but safer this way.
            if(routeNodeCnt <= 0)
            {
                outDirection = Vector2.Zero;
                outEstimateDistanceSq = 0.0f;
                return true;
            }


            float estDistSq = (myPosition - goal).LengthSquared();


            // If no target node, then start at the beginning of the node list.
            if (m_targetNode == null)
            {
                m_targetNode = route.GetPathNodes()[0];
                m_targetNodeIdx = 0;
                m_searchLinePos = 0.0f;
            }
            else
            {
                bool valid = true;

                // Check we are still on the same route as last time.
                if(m_targetNodeIdx < routeNodeCnt)
                {
                    if (route.GetPathNodes()[m_targetNodeIdx] != m_targetNode)
                    {
                        for (int i = 0; i < routeNodeCnt; ++i)
                        {
                            if (route.GetPathNodes()[i] == m_targetNode)
                            {
                                m_targetNodeIdx = i;
                                break;
                            }
                        }

                        valid = false;
                    }
                }
                else
                {
                    // Since the old next node idx has over shot the new route, lets search from the end, to the start.
                    for(int i = routeNodeCnt - 1; i >= 0; --i)
                    {
                        if(route.GetPathNodes()[i] == m_targetNode)
                        {
                            m_targetNodeIdx = i;
                            break;
                        }
                    }

                    valid = false;
                }



                if (!valid)
                {
                    m_targetNode = route.GetPathNodes()[0];
                    m_targetNodeIdx = 0;
                }
            }



            // Check if we can see the next target.
            Vector2 nextTargetPos = m_goalPosition;
            if (m_targetNodeIdx + 1 < routeNodeCnt)
            {
                nextTargetPos = route.GetPathNodes()[m_targetNodeIdx + 1].GetPosition();
            }

            Vector2 checkPos = m_targetNode.GetPosition() + ((nextTargetPos - m_targetNode.GetPosition()) * m_searchLinePos);
            Vector2 checkLine = checkPos - myPosition;

            m_lastSearchPos = checkPos;
            m_lastSearchFrom = myPosition;

            bool canSeeNextTargetPos = CollisionHelpers.IsClearLineOfSight(myPosition, checkLine, bin, occludingBoundaryLayer);
            bool canSeeNextTargetNode = false;

            if (canSeeNextTargetPos)
            {
                m_searchLinePos += 0.05f;

                if (m_searchLinePos >= 1.0f)
                    canSeeNextTargetNode = true;
            }
            else
            {
                m_searchLinePos -= 0.05f;

                if (m_searchLinePos < 0.0f)
                    m_searchLinePos = 0.0f;
            }



            Vector2 dPos = (m_targetNode.GetPosition() - myPosition);
            if(canSeeNextTargetPos)
                dPos = (checkPos - myPosition);

            float dPosLen = dPos.Length();
            Vector2 direction = dPos / dPosLen;


            outDirection = direction;
            outEstimateDistanceSq = estDistSq;



            if (dPosLen < 5.0f || canSeeNextTargetNode)
            {
                ++m_targetNodeIdx;
                m_searchLinePos = 0.0f;

                if (m_targetNodeIdx < routeNodeCnt)
                {
                    m_targetNode = route.GetPathNodes()[m_targetNodeIdx];
                }
                else
                {
                    return true;
                }
            }


            return false;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            const float kNodeConnectionLineWidth = 3.0f;
            const float kNodeRadius = 6.0f;
            Color nodeFillColor = new Color(1.0f, 0.55f, 1.0f, 0.50f);
            Color nodeConnectionLine = new Color(1.0f, 0.55f, 1.0f, 0.50f);


            debugRenderer.DrawFilledLine(m_lastSearchFrom, m_lastSearchPos, nodeConnectionLine, kNodeConnectionLineWidth);
            debugRenderer.DrawFilledCircle(m_lastSearchPos, kNodeRadius, nodeFillColor);
        }
    }
}
