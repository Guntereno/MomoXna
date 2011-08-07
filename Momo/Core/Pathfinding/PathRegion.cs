using System;

using Microsoft.Xna.Framework;

using Momo.Debug;



namespace Momo.Core.Pathfinding
{
    public class PathRegion
    {
        internal PathRegionConnection[] m_connections = null;

        internal PathNode[] m_nodes = null;

        internal Vector2 m_minCorner = Vector2.Zero;
        internal Vector2 m_maxCorner = Vector2.Zero;



        public PathRegion(Vector2 minCorner, Vector2 maxCorner)
        {
            m_minCorner = minCorner;
            m_maxCorner = maxCorner;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawQuad(m_minCorner, m_maxCorner, new Color(0.0f, 1.0f, 0.0f, 0.10f), Color.Black, true, 0.0f);

            if (m_nodes != null)
            {
                for (int i = 0; i < m_nodes.Length; ++i)
                {
                    m_nodes[i].DebugRender(debugRenderer);
                }
            }

            if (m_connections != null)
            {
                for (int i = 0; i < m_connections.Length; ++i)
                {
                    m_connections[i].DebugRender(debugRenderer);
                }
            }
        }


        public void GenerateUniformGridOfNodes(float margin, float spacing)
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
                int nodeIdx = 0;



                Vector2 gridPos = gridAreaMin;

                for (int y = 0; y < gridHeight; ++y)
                {
                    for (int x = 0; x < gridWidth; ++x)
                    {
                        m_nodes[nodeIdx++] = new PathNode(gridPos, margin);
                        gridPos.X += gridDimension.X;
                    }

                    gridPos.Y += gridDimension.Y;
                    gridPos.X = gridAreaMin.X;
                }
            }

        }
    }
}
