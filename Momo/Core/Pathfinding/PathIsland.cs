using System;

using Microsoft.Xna.Framework;

using Momo.Debug;


namespace Momo.Core.Pathfinding
{
    public class PathIsland
    {
        // Finding a path and path tracking
        // 1) Region search (broad phase - must be cheap)
        // 2) A* within each region (this can be delayed until enters region - bet if frequently recalculating route)
        // 3) Within A* path use line of sight to future nodes on calculated path to see if its unobstructed

        private PathRegion[] m_regions = null;


        //public bool void CreatePath(Vector2 startPos, Vector2 endPos, float radius, out PathRoute)
        //{

        //    // Check for direct line of sight
        //    //if(CollisionHelper.isLineOfSight(startPos, endPos, radius)
        //    //{

        //    //    return true;
        //    //}


        //    //PathNode startNode = GetClosestNode(startPos);
        //    //PathNode endNode = GetClosestNode(endPos);

        //    //if(startNode != null && endNode != null)
        //    //{

        //    //  return true;
        //    //}

        //    //return false;
        //}


        //public bool GetClosestNode(Vector2 pos, out PathNode)
        //{
        //    // Use bins to track down limited selection of nodes.
        //}


        public void SetRegions(PathRegion[] regions)
        {
            m_regions = regions;
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
