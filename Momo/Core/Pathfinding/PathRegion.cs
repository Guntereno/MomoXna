using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Momo.Debug;
using Momo.Maths;
using Momo.Core.Spatial;
using Momo.Core.GameEntities;

using Momo.Core.Primitive2D;
using Momo.Core.Collision2D;



namespace Momo.Core.Pathfinding
{
    public class PathRegion
    {
        internal PathNode[] m_nodes = null;
        internal PathRegionConnection[] m_regionConnections = null;

        internal int m_nodeCnt = 0;

        internal Vector2 m_minCorner = Vector2.Zero;
        internal Vector2 m_maxCorner = Vector2.Zero;



        public PathRegion(Vector2 minCorner, Vector2 maxCorner)
        {
            m_minCorner = minCorner;
            m_maxCorner = maxCorner;
        }


        public PathNode[] GetNodes()
        {
            return m_nodes;
        }


        public int GetNodeCount()
        {
            return m_nodeCnt;
        }


        public void GenerateNodesFromGrid(float margin, float spacing)
        {
            Vector2 inset = new Vector2(margin, margin);
            Vector2 gridAreaMin = m_minCorner + inset;
            Vector2 gridAreaMax = m_maxCorner - inset;

            Vector2 dMinMax = gridAreaMax - gridAreaMin;

            Vector2 gridDimension = dMinMax / new Vector2(spacing, spacing);



            if (gridDimension.X > 1.0f && gridDimension.Y > 0.5f)
            {
                int gridWidth = (int)gridDimension.X;
                int gridHeight = (int)gridDimension.Y;

                float decimalX = gridDimension.X - (float)gridWidth;
                float decimalY = gridDimension.Y - (float)gridHeight;

                if (decimalX > 0.5f)
                    ++gridWidth;

                if (decimalY > 0.5f)
                    ++gridHeight;


                gridDimension = dMinMax / new Vector2((float)gridWidth, (float)gridHeight);
                

                ++gridWidth;
                ++gridHeight;

                int nodesRequired = gridWidth * gridHeight;
                m_nodes = new PathNode[nodesRequired];
                m_nodeCnt = 0;



                Vector2 gridPos = gridAreaMin;

                for (int y = 0; y < gridHeight; ++y)
                {
                    for (int x = 0; x < gridWidth; ++x)
                    {
                        m_nodes[m_nodeCnt++] = new PathNode(0, gridPos, margin, 8);
                        gridPos.X += gridDimension.X;
                    }

                    gridPos.Y += gridDimension.Y;
                    gridPos.X = gridAreaMin.X;
                }
            }
        }


        public void GenerateNodesFromBoundaries(float radius, int maxConnections, bool closedLoop, Vector2[][] boundardPositions)
        {
            int nodesRequired = 0;
            for (int i = 0; i < boundardPositions.Length; ++i)
                nodesRequired += boundardPositions[i].Length;

            m_nodes = new PathNode[nodesRequired];
            m_nodeCnt = 0;

            float radiusSq = radius * radius;
            float offset = (float)Math.Sqrt(radiusSq + radiusSq);


            // Test
            //m_nodes[m_nodeCnt++] = new PathNode(new Vector2(750.0f, 250.0f), radius, maxConnections);
            //m_nodes[m_nodeCnt++] = new PathNode(new Vector2(550.0f, 250.0f), radius, maxConnections);
            //m_nodes[m_nodeCnt++] = new PathNode(new Vector2(650.0f, 250.0f), radius, maxConnections);
            //m_nodes[m_nodeCnt++] = new PathNode(new Vector2(450.0f, 250.0f), radius, maxConnections);
            //return;


            int nodeCnt = 0;

            for( int i = 0; i < boundardPositions.Length; ++i)
            {
                Vector2[] innerBoundaryPositions = boundardPositions[i];
                int innerBoundaryPositionCount = innerBoundaryPositions.Length;

                if (innerBoundaryPositionCount < 2)
                    continue;


                Vector2 boundaryPos = innerBoundaryPositions[1];
                Vector2 lastBoundaryPos = innerBoundaryPositions[0];

                Vector2 lastBoundaryNormal = (boundaryPos - lastBoundaryPos);
                lastBoundaryNormal.Normalize();
                lastBoundaryPos = boundaryPos;

                Vector2 firstBoundaryNormal = lastBoundaryNormal;


                for (int j = 2; j < innerBoundaryPositionCount; ++j)
                {
                    boundaryPos = innerBoundaryPositions[j];
                    Vector2 boundaryNormal = boundaryPos - lastBoundaryPos;
                    boundaryNormal.Normalize();

                    m_nodes[m_nodeCnt++] = new PathNode(nodeCnt++, ExtendedMaths2D.ExtrudePoint(lastBoundaryPos, lastBoundaryNormal, boundaryNormal, radius), radius, maxConnections);
   
                    lastBoundaryPos = boundaryPos;
                    lastBoundaryNormal = boundaryNormal;
                }

                if (closedLoop && innerBoundaryPositionCount > 3)
                {
                    m_nodes[m_nodeCnt++] = new PathNode(nodeCnt++, ExtendedMaths2D.ExtrudePoint(lastBoundaryPos, lastBoundaryNormal, firstBoundaryNormal, radius), radius, maxConnections);
                }
            }
        }



        internal struct GeneratorNodeInfo
        {
            internal int[] m_links;
            internal int m_linkCnt;

            public GeneratorNodeInfo(int linkCapacity)
            {
                m_linkCnt = 0;
                m_links = new int[linkCapacity];
            }

            public void AddLink(int linkIdx)
            {
                m_links[m_linkCnt++] = linkIdx;
            }
        };

        internal struct GeneratorLinkInfo
        {
            internal int m_nodeIdx1;
            internal int m_nodeIdx2;
            internal float m_distance;
            internal bool m_valid;


            public GeneratorLinkInfo(int nodeIdx1, int nodeIdx2, float distance)
            {
                m_nodeIdx1 = nodeIdx1;
                m_nodeIdx2 = nodeIdx2;
                m_distance = distance;
                m_valid = true;
            }

            public int GetOtherNodeIdx(int idx)
            {
                if (m_nodeIdx1 != idx)
                    return m_nodeIdx1;

                return m_nodeIdx2;
            }
        };


        // Generates garbage
        public void GenerateNodePaths(float radius, Bin bin, int boundaryLayer)
        {
            const int kBinSelectionCapacity = 500;
            BinRegionSelection tempBinRegionSelection = new BinRegionSelection(kBinSelectionCapacity);


            int pathsRequired = 0;
            pathsRequired = (m_nodeCnt * (m_nodeCnt - 1)) / 2;


            GeneratorNodeInfo[] nodeInfo = new GeneratorNodeInfo[m_nodeCnt];
            GeneratorLinkInfo[] linkInfo = new GeneratorLinkInfo[pathsRequired];
            int linkCnt = 0;

            for (int i = 0; i < m_nodeCnt; ++i)
            {
                nodeInfo[i] = new GeneratorNodeInfo(40);
            }

            // Check for clear paths 
            for (int i = 0; i < m_nodeCnt; ++i)
            {
                PathNode node1 = m_nodes[i];


                for (int j = i + 1; j < m_nodeCnt; ++j)
                {
                    PathNode node2 = m_nodes[j];

                    Vector2 dNodePos = node2.m_position - node1.m_position;
                    float dNodePosLen = dNodePos.Length();

                    bin.GetBinRegionFromLine(node1.m_position, dNodePos, ref tempBinRegionSelection);

                    // Boundaries
                    BinQueryResults queryResults = bin.GetShaderQueryResults();
                    queryResults.StartQuery();
                    bin.Query(ref tempBinRegionSelection, boundaryLayer, queryResults);
                    queryResults.EndQuery();

                    bool contact = false;
                    for (int k = 0; k < queryResults.BinItemCount; ++k)
                    {
                        BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[k];

                        LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
                        contact |= Maths2D.DoesIntersect(       node1.m_position,
                                                                dNodePos,
                                                                radius,
                                                                linePrimitive2D.Point,
                                                                linePrimitive2D.Difference);

                        if (contact)
                            break;
                    }

                    if (contact == false)
                    {
                        //m_connections[m_connectionCnt++] = new PathConnection(node1, node2);
                        linkInfo[linkCnt] = new GeneratorLinkInfo(i, j, dNodePosLen);
                        nodeInfo[i].AddLink(linkCnt);
                        nodeInfo[j].AddLink(linkCnt);
                        ++linkCnt;
                    }
                }
            }

            // Go through each connection and check if there is route that is almost the same distance via other nodes.
            // If there is remove the direct connection
            RemoveUnnecessaryLinks(nodeInfo, linkInfo);

            int invalidLinkCnt = 0;
            for (int i = 0; i < linkCnt; ++i)
            {
                if (linkInfo[i].m_valid)
                {
                    PathNode node1 = m_nodes[linkInfo[i].m_nodeIdx1];
                    PathNode node2 = m_nodes[linkInfo[i].m_nodeIdx2];
                    float distance = linkInfo[i].m_distance;

                    PathConnection connection1 = new PathConnection(node2, distance);
                    PathConnection connection2 = new PathConnection(node1, distance);

                    node1.AddConnection(connection1);
                    node2.AddConnection(connection2);
                }
                else
                {
                    ++invalidLinkCnt;
                }
            }

            //invalidLinkCnt = invalidLinkCnt;
        }


        private void RemoveUnnecessaryLinks(GeneratorNodeInfo[] nodeInfo, GeneratorLinkInfo[] linkInfo)
        {
            List<int> searchedNodes = new List<int>(100);

            for (int i = 0; i < nodeInfo.Length; ++i)
            {
                GeneratorNodeInfo node1 = nodeInfo[i];

                // Go through the links of each node
                for (int j = 0; j < node1.m_linkCnt; ++j)
                {
                    int linkIdx = node1.m_links[j];
                    GeneratorLinkInfo link1 = linkInfo[linkIdx];

                    if (link1.m_valid)
                    {
                        int searchForNodeIdx = link1.GetOtherNodeIdx(i);


                        float searchForNodeShortestDist = link1.m_distance;

                        searchedNodes.Clear();
                        float shortestDistance = 0.0f;
                        bool foundRoute = CalculateShortestDistance(i, searchForNodeIdx, linkIdx, nodeInfo, linkInfo, searchedNodes, 3, ref shortestDistance);


                        if (foundRoute)
                        {
                            float distanceRatio = shortestDistance / searchForNodeShortestDist;

                            if (distanceRatio < 1.1f)
                            {
                                linkInfo[linkIdx].m_valid = false;
                            }
                        }
                    }
                }
            }
        }


        public bool FindNodeFromList(int idx, List<int> nodes)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (idx == nodes[i])
                    return true;
            }

            return false;
        }


        private bool CalculateShortestDistance(int fromNodeIdx, int toNodeIdx, int notUsingLinkIdx, GeneratorNodeInfo[] nodeInfo, GeneratorLinkInfo[] linkInfo, List<int> searchedNodes, int searchDepth, ref float outDistance)
        {
            searchedNodes.Add(fromNodeIdx);

            GeneratorNodeInfo fromNode = nodeInfo[fromNodeIdx];
            GeneratorNodeInfo toNode = nodeInfo[toNodeIdx];

            for (int i = 0; i < fromNode.m_linkCnt; ++i)
            {
                int linkIdx = fromNode.m_links[i];
                GeneratorLinkInfo link = linkInfo[linkIdx];

                if (linkIdx == notUsingLinkIdx)
                    continue;

                if (link.m_valid == false)
                    continue;

                int linkToNodeIdx = link.GetOtherNodeIdx(fromNodeIdx);

                if (FindNodeFromList(linkToNodeIdx, searchedNodes))
                    continue;


                if (linkToNodeIdx == toNodeIdx)
                {
                    outDistance = outDistance + link.m_distance;
                    return true;
                }
                else
                {
                    float newTotalDistance = outDistance + link.m_distance;
                    bool found = CalculateShortestDistance(linkToNodeIdx, toNodeIdx, notUsingLinkIdx, nodeInfo, linkInfo, searchedNodes, searchDepth - 1, ref newTotalDistance);
                    outDistance = newTotalDistance;

                    if (found)
                        return true;
                }
            }

            return false;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            //debugRenderer.DrawQuad(m_minCorner, m_maxCorner, new Color(0.0f, 1.0f, 0.0f, 0.10f), Color.Black, true, 0.0f);

            if (m_nodes != null)
            {
                for (int i = 0; i < m_nodeCnt; ++i)
                {
                    m_nodes[i].DebugRender(debugRenderer);
                }
            }


            if (m_regionConnections != null)
            {
                for (int i = 0; i < m_regionConnections.Length; ++i)
                {
                    m_regionConnections[i].DebugRender(debugRenderer);
                }
            }
        }
    }
}
