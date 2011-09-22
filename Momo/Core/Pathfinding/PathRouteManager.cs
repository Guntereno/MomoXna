using System;

using Microsoft.Xna.Framework;

using Momo.Debug;
using Momo.Fonts;



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
        private int m_pathRoutePoolCnt = 0;

        private CacheRouteInfo[] m_cacheRouteInfoPool = null;
        private int m_cacheRouteInfoPoolCnt = 0;

        // Cache
        private CacheRouteInfo[] m_cachedRouteInfo = null;
        private int m_cachedRouteInfoCapacity = 0;

        private int m_maxCacheScore = 220;
        private int m_initalCacheScore = 140;


        private PathFinder m_pathFinder = null;

        private MutableString m_debugString = new MutableString(50);

        private int m_debugAvgCacheMiss = 0;
        private int m_debugAvgCacheRequests = 0;
        private int m_debugAvgCacheMissAcc = 0;
        private int m_debugAvgCacheRequestsAcc = 0;
        private float m_debugAvgTimer = 0.0f;




        public void Init(int cacheSize, int maxPathRouteLength, int highestPathNodeId)
        {
            // Path route pool
            m_pathRoutePool = new PathRoute[cacheSize];
            m_pathRoutePoolCnt = cacheSize;

            for (int i = 0; i < cacheSize; ++i)
                m_pathRoutePool[i] = new PathRoute(maxPathRouteLength);


            // Cache info pool
            m_cacheRouteInfoPool = new CacheRouteInfo[cacheSize];
            m_cacheRouteInfoPoolCnt = cacheSize;

            for (int i = 0; i < cacheSize; ++i)
                m_cacheRouteInfoPool[i] = new CacheRouteInfo();


            // Cache
            m_cachedRouteInfo = new CacheRouteInfo[highestPathNodeId];
            m_cachedRouteInfoCapacity = highestPathNodeId;

            // Add sentinels
            for (int i = 0; i < highestPathNodeId; ++i)
                m_cachedRouteInfo[i] = new CacheRouteInfo();


            m_pathFinder = new PathFinder();
            m_pathFinder.Init(maxPathRouteLength * 5);
        }


        public void Update(ref FrameTime frameTime)
        {
            m_debugAvgTimer += frameTime.Dt;
            if (m_debugAvgTimer > 1.0f)
            {
                m_debugAvgCacheMiss = m_debugAvgCacheMissAcc;
                m_debugAvgCacheRequests = m_debugAvgCacheRequestsAcc;

                m_debugAvgCacheMissAcc = 0;
                m_debugAvgCacheRequestsAcc = 0;
                m_debugAvgTimer -= 1.0f;
            }

            UdpateCache();
        }


        // Do not hang on to the route, the manager owns these. Request per frame.
        public bool GetPathRoute(PathNode node1, PathNode node2, bool cacheOnly, ref PathRoute outRoute)
        {
            outRoute = RequestCachedPathRoute(node1.GetUniqueId(), node2.GetUniqueId());

            ++m_debugAvgCacheRequestsAcc;


            if (outRoute == null && !cacheOnly)
            {
                ++m_debugAvgCacheMissAcc;

                PathRoute newRoute = m_pathRoutePool[--m_pathRoutePoolCnt];
                m_pathRoutePool[m_pathRoutePoolCnt] = null;

                bool foundRoute = m_pathFinder.FindPath(node1, node2, ref newRoute);

                if (foundRoute)
                {
                    bool sucessfullyCached = CachePathRoute(node1.GetUniqueId(), node2.GetUniqueId(), newRoute);
                    System.Diagnostics.Debug.Assert(sucessfullyCached);

                    outRoute = newRoute;
                }
            }

            return (outRoute != null);
        }


        private void UdpateCache()
        {
            for (int i = 0; i < m_cachedRouteInfoCapacity; ++i)
            {
                if (m_cachedRouteInfo[i].m_nextRouteInfo != null)
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
                            routeInfo.m_pathRoute.Clear();
                            m_pathRoutePool[m_pathRoutePoolCnt++] = routeInfo.m_pathRoute;


                            m_cacheRouteInfoPool[m_cacheRouteInfoPoolCnt++] = routeInfo;

                            prevRouteInfo.m_nextRouteInfo = routeInfo.m_nextRouteInfo;
                            routeInfo = prevRouteInfo;
                        }

                        prevRouteInfo = routeInfo;
                        routeInfo = prevRouteInfo.m_nextRouteInfo;
                    }
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
                    if (routeInfo.m_score > m_maxCacheScore)
                        routeInfo.m_score = m_maxCacheScore;

                    return routeInfo.m_pathRoute;
                }

                routeInfo = routeInfo.m_nextRouteInfo;
            }

            return null;
        }


        private bool CachePathRoute(int pathNodeId1, int pathNodeId2, PathRoute route)
        {
            // Check if the cache is full.
            if (m_cacheRouteInfoPoolCnt > 0)
            {
                CacheRouteInfo cachePathRoute = m_cacheRouteInfoPool[--m_cacheRouteInfoPoolCnt];
                m_cacheRouteInfoPool[m_cacheRouteInfoPoolCnt] = null;

                cachePathRoute.m_pathRoute = route;
                cachePathRoute.m_score = m_initalCacheScore;
                cachePathRoute.m_toPathNodeId = pathNodeId2;

                // Add new cached info just after the sentinel
                CacheRouteInfo oldNextRouteInfo = m_cachedRouteInfo[pathNodeId1].m_nextRouteInfo;
                cachePathRoute.m_nextRouteInfo = oldNextRouteInfo;
                m_cachedRouteInfo[pathNodeId1].m_nextRouteInfo = cachePathRoute;

                return true;
            }

            return false;
        }


        public void DebugRender(DebugRenderer debugRenderer, TextBatchPrinter debugTextBatchPrinter, TextStyle debugTextStyle)
        {
            Vector2 kInfoPanelTopLeftCorner = new Vector2(10.0f, 600.0f);
            const float kLineHeight = 20.0f;

            m_pathFinder.DebugRender(debugRenderer);

            m_debugString.Clear();
            m_debugString.Append("Cached routes: ");
            m_debugString.Append(m_cacheRouteInfoPool.Length - m_cacheRouteInfoPoolCnt);
            m_debugString.Append("/");
            m_debugString.Append(m_cacheRouteInfoPool.Length);
            m_debugString.EndAppend();
            
            debugTextBatchPrinter.AddToDrawList(m_debugString.GetCharacterArray(), Color.White, Color.Black, kInfoPanelTopLeftCorner, debugTextStyle);

            m_debugString.Clear();
            m_debugString.Append("Cache miss: ");
            m_debugString.Append(m_debugAvgCacheMiss);
            m_debugString.Append("/sec");
            m_debugString.EndAppend();

            kInfoPanelTopLeftCorner.Y += kLineHeight;
            debugTextBatchPrinter.AddToDrawList(m_debugString.GetCharacterArray(), Color.White, Color.Black, kInfoPanelTopLeftCorner, debugTextStyle);

            m_debugString.Clear();
            m_debugString.Append("Cache requests: ");
            m_debugString.Append(m_debugAvgCacheRequests);
            m_debugString.Append("/sec");
            m_debugString.EndAppend();

            kInfoPanelTopLeftCorner.Y += kLineHeight;
            debugTextBatchPrinter.AddToDrawList(m_debugString.GetCharacterArray(), Color.White, Color.Black, kInfoPanelTopLeftCorner, debugTextStyle);
        }
    }
}
