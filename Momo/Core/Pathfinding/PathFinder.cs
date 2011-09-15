using System;

using Microsoft.Xna.Framework;

using Momo.Debug;
using Momo.Core.Spatial;



namespace Momo.Core.Pathfinding
{
    public class PathFinder
    {
        private PathNode[] m_nodes = null;

        private OpenList m_openList = null;
        private ClosedList m_closedList = null;



        internal class OpenNode
        {
            internal PathNode m_node;
            internal float m_costFromStart;
            internal float m_costToEnd;
            internal float m_estimatedTotalCost;


            public void Set(OpenNode node)
            {
                m_node = node.m_node;
                m_costFromStart = node.m_costFromStart;
                m_costToEnd = node.m_costToEnd;
                m_estimatedTotalCost = node.m_estimatedTotalCost;
            }
        }


        internal class ClosedNode
        {
            internal PathNode m_node;
            internal ClosedNode m_fromNode;
        }

        internal class OpenList
        {
            internal OpenNode[] m_openNodes = null;
            internal int m_openListCnt = 0;

            public OpenList(int maxCapacity)
            {
                m_openNodes = new OpenNode[maxCapacity];

                for (int i = 0; i < maxCapacity; ++i)
                    m_openNodes[i] = new OpenNode();

                m_openListCnt = 0;
            }


            public void Clear()
            {
                for (int i = 0; i < m_openListCnt; ++i)
                {
                    m_openNodes[i].m_node = null;
                }

                m_openListCnt = 0;
            }

            public OpenNode AddNode(PathNode node, float costToNode, float estimateCostToGoal)
            {
                OpenNode newOpenNode = m_openNodes[m_openListCnt++];
                newOpenNode.m_node = node;
                newOpenNode.m_costFromStart = costToNode;
                newOpenNode.m_costToEnd = estimateCostToGoal;
                newOpenNode.m_estimatedTotalCost = costToNode + estimateCostToGoal;

                return newOpenNode;
            }

            public float RemoveNode(PathNode node)
            {
                float costToNode = float.MaxValue;
                for (int i = 0; i < m_openListCnt; ++i)
                {
                    if (m_openNodes[i].m_node == node)
                    {
                        costToNode = m_openNodes[i].m_costFromStart;

                        // Fill in hole left by this node with the last on hte list.
                        if (i < m_openListCnt - 1)
                            m_openNodes[i].Set(m_openNodes[m_openListCnt-1]);

                        --m_openListCnt;
                        break;
                    }
                }

                return costToNode;
            }


            public OpenNode GetNodeInList(PathNode node)
            {
                for (int i = 0; i < m_openListCnt; ++i)
                {
                    if (m_openNodes[i].m_node == node)
                        return m_openNodes[i];
                }

                return null;
            }


            public PathNode GetBestCostPathNode()
            {
                float bestCost = float.MaxValue;
                PathNode bestPathNode = null;

                for (int i = 0; i < m_openListCnt; ++i)
                {
                    if (m_openNodes[i].m_estimatedTotalCost < bestCost)
                    {
                        bestCost = m_openNodes[i].m_estimatedTotalCost;
                        bestPathNode = m_openNodes[i].m_node;
                    }
                }

                return bestPathNode;
            }
        }


        internal class ClosedList
        {
            internal ClosedNode[] m_closedNodes = null;
            internal int m_closedNodeCnt = 0;

            public ClosedList(int maxCapacity)
            {
                m_closedNodes = new ClosedNode[maxCapacity];

                for (int i = 0; i < maxCapacity; ++i)
                    m_closedNodes[i] = new ClosedNode();

                m_closedNodeCnt = 0;
            }


            public void Clear()
            {
                for (int i = 0; i < m_closedNodeCnt; ++i)
                {
                    m_closedNodes[i].m_node = null;
                }

                m_closedNodeCnt = 0;
            }

            public ClosedNode AddNode(PathNode node, ClosedNode fromNode)
            {
                ClosedNode newClosedNode = m_closedNodes[m_closedNodeCnt++];
                newClosedNode.m_node = node;
                newClosedNode.m_fromNode = fromNode;

                return newClosedNode;
            }

            public bool IsNodeInList(PathNode node)
            {
                for (int i = 0; i < m_closedNodeCnt; ++i)
                {
                    if (m_closedNodes[i].m_node == node)
                        return true;
                }

                return false;
            }
        }


        public void Init(int maxOpenNodes)
        {
            m_openList = new OpenList(maxOpenNodes);
            m_closedList = new ClosedList(maxOpenNodes);
        }


        public bool FindPath(PathNode startNode, PathNode endNode, ref PathRoute path)
        {
            m_openList.Clear();
            m_closedList.Clear();




            PathNode activeOpenNode = startNode;
            ClosedNode lastClosedNode = null;
            float costToNode = 0.0f;
            float bestOpenNodeCost = float.MaxValue;


            m_openList.AddNode(activeOpenNode, 0.0f, GetEstimatedCostToTravel(activeOpenNode, endNode));


            while (activeOpenNode != null)
            {
                // Check if we have found the goal.
                if (activeOpenNode == endNode)
                {
                    path.Clear();
                    //while (lastClosedNode != null)
                    //{
                    //    path.AddNodeToPath(lastClosedNode.m_node);
                    //    lastClosedNode = lastClosedNode.m_fromNode;
                    //}
                    return true;
                }

                // Remove openNode from the linked list. Add it to the closed list.
                costToNode = m_openList.RemoveNode(activeOpenNode);
                lastClosedNode = m_closedList.AddNode(activeOpenNode, lastClosedNode);

                PathNode bestOpenNode = null;

                for (int i = 0; i < activeOpenNode.m_connectionCnt; ++i)
                {
                    PathConnection connection = activeOpenNode.m_connections[i];
                    PathNode connectedToPathNode = connection.m_toNode;

                    // If the connection is part of the closed list, then move on to the next
                    // connection.
                    if (m_closedList.IsNodeInList(connectedToPathNode))
                        continue;


                    OpenNode connectedOpenNode = m_openList.GetNodeInList(connectedToPathNode);
                    if (connectedOpenNode == null)
                    {
                        float connectNodeCostToNode = costToNode + connection.m_distance;
                        float costToEnd = GetEstimatedCostToTravel(connectedToPathNode, endNode);

                        connectedOpenNode = m_openList.AddNode(connectedToPathNode, connectNodeCostToNode, costToEnd);
                    }

                    if (connectedOpenNode.m_estimatedTotalCost < bestOpenNodeCost)
                    {
                        bestOpenNodeCost = connectedOpenNode.m_estimatedTotalCost;
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
            return (node1.m_position - node2.m_position).Length();
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            for (int i = 0; i < m_openList.m_openListCnt; ++i)
            {
                PathNode node = m_openList.m_openNodes[i].m_node;
                debugRenderer.DrawOutlineCircle(node.GetPosition(), node.GetRadius() + 5.0f, new Color(0.0f, 1.0f, 0.5f, 0.55f), 5.0f);
            }

            for (int i = 0; i < m_closedList.m_closedNodeCnt; ++i)
            {
                PathNode node = m_closedList.m_closedNodes[i].m_node;
                debugRenderer.DrawOutlineCircle(node.GetPosition(), node.GetRadius() + 5.0f, new Color(1.0f, 0.0f, 0.5f, 0.55f), 5.0f);
            }
        }

    }
}
