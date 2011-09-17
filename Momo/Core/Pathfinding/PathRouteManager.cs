using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Pathfinding
{
    public class PathRouteManager
    {
        internal class CacheRouteInfo
        {
            internal int m_score;
            internal int m_toPathNodeId;
            internal PathRoute m_pathRoute;

            internal CacheRouteInfo m_nextRouteInfo;
        }


        private PathRoute[] m_pathRoutePool = null;
        private int m_pathRoutesCapacity = 0;
        private int m_pathRoutesInUse = 0;

        private CacheRouteInfo[] m_cacheInfoPool = null;
        private int m_cacheInfosCapacity = 0;
        private int m_cacheInfosInUse = 0;

        // Cache
        private CacheRouteInfo[] m_cachedRouteInfo = null;
        private int m_cachedRouteInfoCapacity = 0;


        private PathFinder m_pathFinder = null;



        public void Init(int cacheSize, int maxPathRoutes, int maxPathRouteLength, int highestPathNodeId)
        {
            // Path route pool
            m_pathRoutePool = new PathRoute[maxPathRoutes];
            m_pathRoutesCapacity = maxPathRoutes;
            m_pathRoutesInUse = 0;

            for (int i = 0; i < maxPathRoutes; ++i)
                m_pathRoutePool[i] = new PathRoute(maxPathRouteLength);


            // Cache info pool
            m_cacheInfoPool = new CacheRouteInfo[cacheSize];
            m_cacheInfosCapacity = cacheSize;
            m_cacheInfosInUse = 0;

            for (int i = 0; i < cacheSize; ++i)
                m_cacheInfoPool[i] = new CacheRouteInfo();


            // Cache
            m_cachedRouteInfo = new CacheRouteInfo[highestPathNodeId];
            m_cachedRouteInfoCapacity = highestPathNodeId;

            // Add sentinels
            for (int i = 0; i < highestPathNodeId; ++i)
                m_cachedRouteInfo[i] = new CacheRouteInfo();


            m_pathFinder = new PathFinder();
            m_pathFinder.Init(maxPathRouteLength * 5);
        }


        // Do not hang on to the route, the manager owns these. Request per frame.
        public bool GetPathRoute(PathNode node1, PathNode node2, ref PathRoute route)
        {
            bool foundRoute = false;
            route = RequestCachedPathRoute(node1.GetUniqueId(), node2.GetUniqueId());

            if (route == null)
            {
                foundRoute = m_pathFinder.FindPath(node1, node2, ref route);

                if (foundRoute)
                    CachePathRoute(node1.GetUniqueId(), node2.GetUniqueId(), route);
            }

            return foundRoute;
        }


        private void UdpateCache()
        {
            for (int i = 0; i < m_cachedRouteInfoCapacity; ++i)
            {
                // Skip sentinel
                CacheRouteInfo prevRouteInfo = m_cachedRouteInfo[i];
                CacheRouteInfo routeInfo = prevRouteInfo.m_nextRouteInfo;

                while (routeInfo != null)
                {
                    --routeInfo.m_score;
                    if (routeInfo.m_score == 0)
                    {
                        // Return the route to the pool now that we are done with it.
                        PathRoute route = routeInfo.m_pathRoute;
                        m_pathRoutePool[m_pathRoutesInUse--] = route;

                        prevRouteInfo.m_nextRouteInfo = routeInfo.m_nextRouteInfo;
                        routeInfo = prevRouteInfo;
                    }

                    prevRouteInfo = routeInfo;
                    routeInfo = prevRouteInfo.m_nextRouteInfo;
                }
            }
        }


        private PathRoute RequestCachedPathRoute(int pathNodeId1, int pathNodeId2)
        {
            // Skip sentinel
            CacheRouteInfo routeInfo = m_cachedRouteInfo[pathNodeId1].m_nextRouteInfo;

            while (routeInfo != null)
            {
                if (routeInfo.m_toPathNodeId == pathNodeId2)
                {
                    ++routeInfo.m_score;
                    return routeInfo.m_pathRoute;
                }

                routeInfo = routeInfo.m_nextRouteInfo;
            }

            return null;
        }


        private bool CachePathRoute(int pathNodeId1, int pathNodeId2, PathRoute route)
        {
            // Check if the cache is full.
            if (m_cacheInfosInUse == m_cacheInfosCapacity)
                return false;

            CacheRouteInfo cachePathRoute = m_cacheInfoPool[m_cacheInfosInUse++];

            cachePathRoute.m_pathRoute = route;
            cachePathRoute.m_score = 2;
            cachePathRoute.m_toPathNodeId = pathNodeId2;

            // Add new cached info just after the sentinel
            CacheRouteInfo oldNextRouteInfo = m_cachedRouteInfo[pathNodeId1].m_nextRouteInfo;
            cachePathRoute.m_nextRouteInfo = oldNextRouteInfo;
            m_cachedRouteInfo[pathNodeId1].m_nextRouteInfo = cachePathRoute;

            return true;
        }

    }
}
