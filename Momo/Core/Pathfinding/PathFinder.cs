using System;

using Microsoft.Xna.Framework;

using Momo.Debug;
using Momo.Core.Spatial;



namespace Momo.Core.Pathfinding
{
    public class PathFinder
    {
        private PathNodeList m_openList = null;
        private PathNodeList m_closedList = null;


        public PathNodeList OpenList
        {
            get { return m_openList; }
        }

        public PathNodeList CloseList
        {
            get { return m_closedList; }
        }



        public struct PathFinderNode
        {
            internal PathNode m_node;
            internal int m_fromClosedIdx;

            internal float m_costFromStart;
            internal float m_costToEnd;
            internal float m_estimatedTotalCost;


            public PathNode Node
            {
                get { return m_node; }
            }


            public void Set(PathFinderNode node)
            {
                m_node = node.m_node;
                m_fromClosedIdx = node.m_fromClosedIdx;
                m_costFromStart = node.m_costFromStart;
                m_costToEnd = node.m_costToEnd;
                m_estimatedTotalCost = node.m_estimatedTotalCost;
            }

            public void Set(PathNode node, int fromClosedIdx, float costFromStart, float costToEnd)
            {
                m_node = node;
                m_fromClosedIdx = fromClosedIdx;
                m_costFromStart = costFromStart;
                m_costToEnd = costToEnd;
                m_estimatedTotalCost = costFromStart + costToEnd;
            }
        }


        public class PathNodeList
        {
            internal PathFinderNode[] m_pathFinderNodes = null;
            internal int m_pathFinderNodeCnt = 0;


            public PathFinderNode[] PathFinderNodes
            {
                get { return m_pathFinderNodes; }
            }

            public int PathFinderNodeCnt
            {
                get { return m_pathFinderNodeCnt; }
            }


            public PathNodeList(int maxCapacity)
            {
                m_pathFinderNodes = new PathFinderNode[maxCapacity];
                m_pathFinderNodeCnt = 0;
            }


            public void Clear()
            {
                for (int i = 0; i < m_pathFinderNodeCnt; ++i)
                {
                    m_pathFinderNodes[i].m_node = null;
                }

                m_pathFinderNodeCnt = 0;
            }


            public int AddNode(PathNode node, int fromClosedIdx, float costToNode, float estimateCostToGoal)
            {
                m_pathFinderNodes[m_pathFinderNodeCnt++].Set(node, fromClosedIdx, costToNode, estimateCostToGoal);

                return m_pathFinderNodeCnt-1;
            }

            public int AddNode(PathFinderNode node)
            {
                m_pathFinderNodes[m_pathFinderNodeCnt++].Set(node);

                return m_pathFinderNodeCnt-1;
            }


            public PathFinderNode RemoveNode(PathNode node)
            {
                for (int i = 0; i < m_pathFinderNodeCnt; ++i)
                {
                    if (m_pathFinderNodes[i].m_node == node)
                    {
                        PathFinderNode pathFinderNode = m_pathFinderNodes[i];

                        // Fill in hole left by this node with the last on hte list.
                        if (i < m_pathFinderNodeCnt - 1)
                        {
                            m_pathFinderNodes[i].Set(m_pathFinderNodes[m_pathFinderNodeCnt - 1]);
                        }

                        --m_pathFinderNodeCnt;

                        return pathFinderNode;
                    }
                }

                return m_pathFinderNodes[0];
            }


            public int GetNodeIdx(PathNode node)
            {
                for (int i = 0; i < m_pathFinderNodeCnt; ++i)
                {
                    if (m_pathFinderNodes[i].m_node == node)
                        return i;
                }

                return -1;
            }


            public PathNode GetBestCostPathNode()
            {
                float bestCost = float.MaxValue;
                PathNode bestPathNode = null;

                for (int i = 0; i < m_pathFinderNodeCnt; ++i)
                {
                    if (m_pathFinderNodes[i].m_estimatedTotalCost < bestCost)
                    {
                        bestCost = m_pathFinderNodes[i].m_estimatedTotalCost;
                        bestPathNode = m_pathFinderNodes[i].m_node;
                    }
                }

                return bestPathNode;
            }
        }



        public void Init(int maxOpenNodes)
        {
            m_openList = new PathNodeList(maxOpenNodes);
            m_closedList = new PathNodeList(maxOpenNodes);
        }


        public bool FindPath(PathNode startNode, PathNode endNode, ref PathRoute path)
        {
            m_openList.Clear();
            m_closedList.Clear();


            // We search from the goal to the start, that way the formed route is the correct order and requires no
            // flipping.

            PathNode activeOpenNode = endNode;
            int lastClosedIdx = -1;
            float bestOpenNodeCost = float.MaxValue;


            m_openList.AddNode(endNode, -1, 0.0f, GetEstimatedCostToTravel(endNode, startNode));


            while (activeOpenNode != null)
            {
                // Remove openNode from the linked list. Add it to the closed list.
                PathFinderNode activePathFinderNode = m_openList.RemoveNode(activeOpenNode);
                lastClosedIdx = m_closedList.AddNode(activePathFinderNode);


                // Check if we have found the goal.
                if (activeOpenNode == startNode)
                {
                    while (lastClosedIdx >= 0)
                    {
                        path.AddNodeToPath(m_closedList.m_pathFinderNodes[lastClosedIdx].m_node);
                        lastClosedIdx = m_closedList.m_pathFinderNodes[lastClosedIdx].m_fromClosedIdx;
                    }
                    return true;
                }


                PathNode bestOpenNode = null;

                for (int i = 0; i < activeOpenNode.m_connectionCnt; ++i)
                {
                    PathConnection connection = activeOpenNode.m_connections[i];
                    PathNode connectedToPathNode = connection.m_toNode;

                    // If the connection is part of the closed list, then move on to the next
                    // connection.
                    if (m_closedList.GetNodeIdx(connectedToPathNode) >= 0)
                        continue;


                    int connectedOpenNodeIdx = m_openList.GetNodeIdx(connectedToPathNode);
                    if (connectedOpenNodeIdx < 0)
                    {
                        float connectNodeCostToNode = activePathFinderNode.m_costFromStart + connection.m_distance;
                        float costToEnd = GetEstimatedCostToTravel(connectedToPathNode, startNode);

                        connectedOpenNodeIdx = m_openList.AddNode(connectedToPathNode, lastClosedIdx, connectNodeCostToNode, costToEnd);
                    }

                    float openNodeEstimatedTotalCost = m_openList.m_pathFinderNodes[connectedOpenNodeIdx].m_estimatedTotalCost;
                    if (openNodeEstimatedTotalCost < bestOpenNodeCost)
                    {
                        bestOpenNodeCost = openNodeEstimatedTotalCost;
                        bestOpenNode = connectedToPathNode;
                    }
                }


                if (bestOpenNode == null)
                {
                    bestOpenNode = m_openList.GetBestCostPathNode();
                    bestOpenNodeCost = float.MaxValue;
                }

                activeOpenNode = bestOpenNode;
            }


            return false;
        }



        public float GetEstimatedCostToTravel(PathNode node1, PathNode node2)
        {
            //return Math.Abs(node1.m_position.X - node2.m_position.X) + Math.Abs(node1.m_position.Y - node2.m_position.Y);
            return (node1.m_position - node2.m_position).Length();
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_openList.m_pathFinderNodeCnt; ++i)
            {
                PathNode node = m_openList.m_pathFinderNodes[i].m_node;
                debugRenderer.DrawOutlineCircle(node.GetPosition(), node.GetRadius() + 5.0f, new Color(0.0f, 1.0f, 0.5f, 0.55f), 5.0f);
            }

            for (int i = 0; i < m_closedList.m_pathFinderNodeCnt; ++i)
            {
                PathNode node = m_closedList.m_pathFinderNodes[i].m_node;
                debugRenderer.DrawOutlineCircle(node.GetPosition(), node.GetRadius() + 5.0f, new Color(1.0f, 0.0f, 0.5f, 0.55f), 5.0f);
            }
        }

    }
}
