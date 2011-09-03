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
        internal PathConnection[] m_connections = null;
        internal PathRegionConnection[] m_regionConnections = null;

        internal int m_nodeCnt = 0;
        internal int m_connectionCnt = 0;

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
                        m_nodes[m_nodeCnt++] = new PathNode(gridPos, margin);
                        gridPos.X += gridDimension.X;
                    }

                    gridPos.Y += gridDimension.Y;
                    gridPos.X = gridAreaMin.X;
                }
            }
        }


        public void GenerateNodesFromBoundaries(float radius, bool closedLoop, Vector2[][] boundardPositions)
        {
            int nodesRequired = 0;
            for (int i = 0; i < boundardPositions.Length; ++i)
                nodesRequired += boundardPositions[i].Length;

            m_nodes = new PathNode[nodesRequired];
            m_nodeCnt = 0;

            float radiusSq = radius * radius;
            float offset = (float)Math.Sqrt(radiusSq + radiusSq);


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

                    m_nodes[m_nodeCnt++] = new PathNode(ExtendedMaths2D.ExtrudePoint(lastBoundaryPos, lastBoundaryNormal, boundaryNormal, radius), radius);
   
                    lastBoundaryPos = boundaryPos;
                    lastBoundaryNormal = boundaryNormal;
                }

                if (closedLoop && innerBoundaryPositionCount > 3)
                {
                    m_nodes[m_nodeCnt++] = new PathNode(ExtendedMaths2D.ExtrudePoint(lastBoundaryPos, lastBoundaryNormal, firstBoundaryNormal, radius), radius);
                }
            }
        }



        internal struct GeneratorNodeInfo
        {
            internal GeneratorLinkInfo[] m_links;
            internal int m_linkCnt;

            public GeneratorNodeInfo(int linkCapacity)
            {
                m_linkCnt = 0;
                m_links = new GeneratorLinkInfo[linkCapacity];
            }

            public void AddLink(int nodeIdx, float distance)
            {
                m_links[m_linkCnt++] = new GeneratorLinkInfo(nodeIdx, distance);
            }
        };

        internal struct GeneratorLinkInfo
        {
            internal int m_nodeIdx;
            internal float m_distance;

            public GeneratorLinkInfo(int nodeIdx, float distance)
            {
                m_nodeIdx = nodeIdx;
                m_distance = distance;
            }
        };


        // Generates garbage
        public void GenerateNodePaths(float radius, Bin bin, int boundaryLayer)
        {
            const int kBinSelectionCapacity = 500;
            GeneratorNodeInfo[] nodeInfo = new GeneratorNodeInfo[m_nodeCnt];
            BinRegionSelection tempBinRegionSelection = new BinRegionSelection(kBinSelectionCapacity);


            BinRegionUniform boundaryRegion = new BinRegionUniform();


            int pathsRequired = 0;
            pathsRequired = (m_nodeCnt * (m_nodeCnt - 1)) / 2;

            m_connections = new PathConnection[pathsRequired];
            m_connectionCnt = 0;


            // Check for clear paths 
            for (int i = 0; i < m_nodeCnt; ++i)
            {
                PathNode node1 = m_nodes[i];

                nodeInfo[i] = new GeneratorNodeInfo(20);

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
                        checkBoundary.GetBinRegion(ref boundaryRegion);

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
                        //nodeInfo[i].AddLink(j, dNodePosLen);
                    }
                }
            }

            // Go through each connection and check if there is route that is almost the same distance via other nodes.
            // If there is remove the direct connection
            //RemoveUnnecessaryLinks(nodeInfo);

            for (int i = 0; i < m_nodeCnt; ++i)
            {
                for (int j = 0; j < nodeInfo[i].m_linkCnt; ++j)
                {
                    m_connections[m_connectionCnt++] = new PathConnection(m_nodes[i], m_nodes[nodeInfo[i].m_links[j].m_nodeIdx]);
                }
            }
        }


        //private void RemoveUnnecessaryLinks(GeneratorNodeInfo[] nodeInfo)
        //{
        //    List<int> searchedNodes = new List<int>(100);

        //    for (int i = 0; i < nodeInfo.Length; ++i)
        //    {
        //        for (int j = 0; j < nodeInfo[i].m_linkCnt; ++j)
        //        {
        //            int searchForNodeIdx = nodeInfo[i].m_links[j].m_nodeIdx;
        //            float searchForNodeShortestDist = nodeInfo[i].m_links[j].m_distance;


        //            for (int k = 0; k < nodeInfo[searchForNodeIdx].m_linkCnt; ++k)
        //            {
        //                if (j != k)
        //                {
        //                    int fromNodeIdx = nodeInfo[searchForNodeIdx].m_links[k].m_nodeIdx;
        //                    float shortestDistance = 0.0f;

        //                    searchedNodes.Clear();
        //                    bool foundRoute = CalculateShortestDistance(fromNodeIdx, searchForNodeIdx, nodeInfo, searchedNodes, 3, ref shortestDistance);
        //                }
        //            }
        //        }
        //    }
        //}


        private bool CalculateShortestDistance(int fromNodeIdx, int toNodeIdx, GeneratorNodeInfo[] nodeInfo, List<int> searchedNodes, int searchDepth, ref float outDistance)
        {
            for (int i = 0; i < nodeInfo[fromNodeIdx].m_linkCnt; ++i)
            {
                int linkToIdx = nodeInfo[fromNodeIdx].m_links[i].m_nodeIdx;
                bool alreadySearched = false;

                for (int j = 0; j < searchedNodes.Count; ++j)
                {
                    if (linkToIdx == searchedNodes[j])
                    {
                        alreadySearched = true;
                        break;
                    }
                }

                if (alreadySearched == false)
                {
                    searchedNodes.Add(linkToIdx);

                    bool found = false;

                    if (linkToIdx == toNodeIdx)
                    {
                        outDistance += nodeInfo[fromNodeIdx].m_links[i].m_distance;
                        return true;
                    }
                    else
                    {
                        found = CalculateShortestDistance(linkToIdx, toNodeIdx, nodeInfo, searchedNodes, searchDepth - 1, ref outDistance); 
                    }

                    if (found)
                    {
                        return true;
                    }
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


            if (m_connections != null)
            {
                for (int i = 0; i < m_connectionCnt; ++i)
                {
                    m_connections[i].DebugRender(debugRenderer);
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
