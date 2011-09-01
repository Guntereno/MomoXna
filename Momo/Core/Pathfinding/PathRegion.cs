using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Momo.Debug;
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


        public PathNode[] Nodes
        {
            get { return m_nodes; }
            set { m_nodes = value; }
        }

        public PathRegionConnection[] RegionConnections
        {
            get { return m_regionConnections; }
            set { m_regionConnections = value; }
        }


        public PathRegion(Vector2 minCorner, Vector2 maxCorner)
        {
            m_minCorner = minCorner;
            m_maxCorner = maxCorner;
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


        public void GenerateNodesFromBoundaries(Vector2[][] boundardPositions)
        {
            int nodesRequired = 0;
            for (int i = 0; i < boundardPositions.Length; ++i)
                nodesRequired += boundardPositions[i].Length;

            m_nodes = new PathNode[nodesRequired];
            m_nodeCnt = 0;



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
                lastBoundaryNormal = Math2D.Perpendicular(lastBoundaryNormal);

                lastBoundaryPos = boundaryPos;


                for (int j = 2; j < innerBoundaryPositionCount; ++j)
                {
                    boundaryPos = innerBoundaryPositions[j];
                    Vector2 boundaryNormal = boundaryPos - lastBoundaryPos;
                    boundaryNormal.Normalize();
                    boundaryNormal = Math2D.Perpendicular(boundaryNormal);


                    Vector2 cornerNormal = (boundaryNormal + lastBoundaryNormal) * -0.5f;
                    cornerNormal.Normalize();

                    m_nodes[m_nodeCnt++] = new PathNode(lastBoundaryPos + (cornerNormal * 15.0f), 15.0f);

                    lastBoundaryPos = boundaryPos;
                    lastBoundaryNormal = boundaryNormal;
                }
            }
        }


        struct ConnectedTo
        {
            PathNode m_node;
            Vector2 m_dPos;
            float m_dPosLenSq;

            public ConnectedTo(PathNode node, Vector2 dPos, float dPosLenSq)
            {
                m_node = node;
                m_dPos = dPos;
                m_dPosLenSq = dPosLenSq;
            }
        };

        public void GenerateNodePaths(Bin bin, int boundaryLayer)
        {
            const int kConnectedToCapacity = 12;
            const int kBinSelectionCapacity = 500;
            BinRegionSelection tempBinRegionSelection = new BinRegionSelection(kBinSelectionCapacity);
            ConnectedTo [] connectedTo = new ConnectedTo[kConnectedToCapacity];

            BinRegionUniform boundaryRegion = new BinRegionUniform();
            Vector2 intersectPoint = Vector2.Zero;

            int pathsRequired = 0;
            pathsRequired = (m_nodeCnt * (m_nodeCnt - 1)) / 2;

            m_connections = new PathConnection[pathsRequired];
            m_connectionCnt = 0;




            // Check for clear paths 
            for (int i = 0; i < m_nodeCnt; ++i)
            {
                PathNode node1 = m_nodes[i];
                int connectedToCnt = 0;

                for (int j = i + 1; j < m_nodeCnt; ++j)
                {
                    PathNode node2 = m_nodes[j];

                    Vector2 dNodePos = node2.m_position - node1.m_position;

                    bin.GetBinRegionFromLine(node1.m_position, dNodePos, ref tempBinRegionSelection);

                    // Boundaries
                    bin.StartQuery();
                    bin.Query(tempBinRegionSelection, boundaryLayer);
                    BinQueryResults queryResults = bin.EndQuery();

                    bool contact = false;
                    for (int k = 0; k < queryResults.BinItemCount; ++k)
                    {
                        BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[k];
                        checkBoundary.GetBinRegion(ref boundaryRegion);

                        LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
                        contact |= Math2D.DoesIntersect(node1.m_position,
                                                                dNodePos,
                                                                linePrimitive2D.Point,
                                                                linePrimitive2D.Difference,
                                                                ref intersectPoint);


                        if (contact)
                            break;
                    }

                    if (contact == false)
                    {
                        //connectedTo[connectedToCnt++] = new ConnectedTo();
                        m_connections[m_connectionCnt++] = new PathConnection(node1, node2);
                    }
                }
            }
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
