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
        private Vector2 m_lastTargetPos = Vector2.Zero;

        private float m_searchLineDist = 0.0f;
        private Vector2 m_lastSearchFrom = Vector2.Zero;
        private Vector2 m_lastSearchPos = Vector2.Zero;


        public void Reset()
        {
            m_targetNode = null;
            m_targetNodeIdx = -1;
            m_searchLineDist = 0.0f;
        }



        // Returns true if reach goal.
        // Assumes you can see the first node on the route.
        public bool Track(Vector2 myPosition, Vector2 goal, Bin bin, int occludingBoundaryLayer, ref PathRoute route, out Vector2 outDirection)
        {
            int routeNodeCnt = route.GetPathNodeCount();

            bool foundOldRoute = GetNodeToTrack(ref route, out m_targetNodeIdx, out m_targetNode);


            // Could not find a node to target. None in the list most likely.
            if (m_targetNode == null)
            {
                outDirection = Vector2.Zero;
                return false;
            }


            Vector2 nextTargetPos = GetNextTargetPos(ref route, m_targetNodeIdx);

            Vector2 dTargetNext = nextTargetPos - m_targetNode.GetPosition();
            float dTargetNextLen = dTargetNext.Length();
            Vector2 dTargetNextDirection = dTargetNext / dTargetNextLen;

            Vector2 checkPos = m_targetNode.GetPosition() + (dTargetNextDirection * m_searchLineDist);
            Vector2 checkLine = checkPos - myPosition;

            m_lastSearchPos = checkPos;
            m_lastSearchFrom = myPosition;

            // Check if we have a clean line of sight to our next target position.
            bool canSeeNextTargetPos = CollisionHelpers.IsClearLineOfSight(myPosition, checkLine, bin, occludingBoundaryLayer);
            bool moveToNextTargetNode = false;

            // If we can see the next target position, increment our search line position (for next frame).
            const float kSearchLineStep = 9.0f;
            if (canSeeNextTargetPos)
            {
                m_lastTargetPos = checkPos;
                m_searchLineDist += kSearchLineStep;

                if (m_searchLineDist >= dTargetNextLen)
                {
                    m_searchLineDist = dTargetNextLen;
                    moveToNextTargetNode = true;
                }
            }
            else if(m_searchLineDist > 0.0f)
            {
                m_searchLineDist -= kSearchLineStep;
            }




            Vector2 dPos = (m_targetNode.GetPosition() - myPosition);
            if(canSeeNextTargetPos)
                dPos = (m_lastTargetPos - myPosition);

            float dPosLen = dPos.Length();
            Vector2 direction = dPos / dPosLen;


            outDirection = direction;



            if (dPosLen < 5.0f || moveToNextTargetNode)
            {
                ++m_targetNodeIdx;
                m_searchLineDist = 0.0f;

                if (m_targetNodeIdx < routeNodeCnt)
                {
                    m_targetNode = route.GetPathNodes()[m_targetNodeIdx];
                    m_lastTargetPos = m_targetNode.GetPosition();
                }
                else
                {
                    m_lastTargetPos = m_goalPosition;
                    return true;
                }
            }


            return true;
        }


        private Vector2 GetNextTargetPos(ref PathRoute route, int targetNodeIdx)
        {
            // Check if we can see the next target.
            Vector2 nextTargetPos = m_goalPosition;
            if (m_targetNodeIdx + 1 < route.GetPathNodeCount())
            {
                nextTargetPos = route.GetPathNodes()[m_targetNodeIdx + 1].GetPosition();
            }

            return nextTargetPos;
        }


        // Returns true if the route needs resetting, no previous route, or lost track of old node.
        private bool GetNodeToTrack(ref PathRoute route, out int pathNodeIdx, out PathNode pathNode)
        {
            int routeNodeCnt = route.GetPathNodeCount();


            // If the route has no nodes, then return true, we have 'tracked' to the goal. This should never
            // really happen but safer this way.
            if (routeNodeCnt <= 0)
            {
                pathNode = null;
                pathNodeIdx = -1;
                return true;
            }



            int nodeIdx = 0;
            bool resetRoute = true;


            if (m_targetNode != null)
            {
                // Check we are still on the same route as last time.
                if (m_targetNodeIdx < routeNodeCnt)
                {
                    if (route.GetPathNodes()[m_targetNodeIdx] != m_targetNode)
                    {
                        for (int i = 0; i < routeNodeCnt; ++i)
                        {
                            if (route.GetPathNodes()[i] == m_targetNode)
                            {
                                nodeIdx = i;
                                resetRoute = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        nodeIdx = m_targetNodeIdx;
                    }
                }
                else
                {
                    // Since the old next node idx has over shot the new route, lets search from the end, to the start.
                    for (int i = routeNodeCnt - 1; i >= 0; --i)
                    {
                        if (route.GetPathNodes()[i] == m_targetNode)
                        {
                            nodeIdx = i;
                            resetRoute = false;
                            break;
                        }
                    }
                }
            }


            pathNode = route.GetPathNodes()[nodeIdx];
            pathNodeIdx = nodeIdx;

            return resetRoute;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            const float kNodeConnectionLineWidth = 3.0f;
            const float kNodeRadius = 6.0f;
            Color nodeFillColor = new Color(1.0f, 0.55f, 1.0f, 0.50f);
            Color nodeConnectionLine = new Color(1.0f, 0.55f, 1.0f, 0.50f);


            debugRenderer.DrawFilledLine(m_lastSearchFrom, m_lastTargetPos, nodeConnectionLine, kNodeConnectionLineWidth);
            debugRenderer.DrawFilledCircle(m_lastTargetPos, kNodeRadius, nodeFillColor);
            debugRenderer.DrawFilledCircle(m_lastSearchPos, kNodeRadius, nodeFillColor);
        }
    }
}
