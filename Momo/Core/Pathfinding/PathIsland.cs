using System;

using Microsoft.Xna.Framework;

using Momo.Debug;
using Momo.Core.Spatial;



namespace Momo.Core.Pathfinding
{
    public class PathIsland
    {
        // Finding a path and path tracking
        // 1) Region search (broad phase - must be cheap)
        // 2) A* within each region (this can be delayed until enters region - better if frequently recalculating route)
        // 3) Within A* path use line of sight to future nodes on calculated path to see if its unobstructed

        private PathRegion[] m_regions = null;


        public bool CreatePath(Vector2 startPos, Vector2 endPos, PathNode startNode, PathNode endNode, ref PathRoute outRoute)
        {
            if(startNode != null)
            {
                if(endNode != null)
                {
                    // We have two nodes to form a path between.
                    outRoute.SetPathInfo(startPos, endPos, startNode, endNode);


                    return true;
                }
            }

            return false;
        }



        public void SetRegions(PathRegion[] regions)
        {
            m_regions = regions;
        }

        public PathRegion[] GetRegions()
        {
            return m_regions;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            if (m_regions != null)
            {
                for (int i = 0; i < m_regions.Length; ++i)
                {
                    PathRegion region = m_regions[i];

                    region.DebugRender(debugRenderer);
                }
            }
        }
    }
}
