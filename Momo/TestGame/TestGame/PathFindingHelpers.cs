using System;

using Microsoft.Xna.Framework;

using Momo.Maths;
using Momo.Debug;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.Spatial;
using Momo.Core.Pathfinding;

using Momo.Core.Primitive2D;
using Momo.Core.Collision2D;


using TestGame.Entities;
using TestGame.Objects;



namespace TestGame
{
    public class PathFindingHelpers
    {
        // TODO: Need to get this using one of the several pre computed extruded collision layers.
        // Considerably cheaper.


        private static float ms_maxCircularSearchRange = 0.0f;
        private static int m_maxCircularSearchRegions = 0;
        private static BinRegionSelection [] ms_circularSearchRegions = null;

        private static PathFinder ms_pathFinder = null;



        public static void Init(float maxCircularSearchRange, int maxCircularSearchRegions, Bin bin)
        {
            ms_maxCircularSearchRange = maxCircularSearchRange;
            m_maxCircularSearchRegions = maxCircularSearchRegions;
            ms_circularSearchRegions = new BinRegionSelection[maxCircularSearchRegions];

            float circularSelectionRadiusBands = ms_maxCircularSearchRange / (float)m_maxCircularSearchRegions;
            BinRegionSelection[] emptyMinusSelection = new BinRegionSelection[1];
            BinRegionSelection circularSelection = new BinRegionSelection(1000);
            BinLocation centre = new BinLocation(0, 0);

            for (int i = 0; i < maxCircularSearchRegions; ++i)
            {
                float radius = circularSelectionRadiusBands * (float)(i + 1);
                float resolution = (float)Math.PI / (radius * 0.15f);

                bin.GetBinRegionFromCircle(centre,
                                            radius,
                                            resolution,
                                            ref ms_circularSearchRegions,
                                            i,
                                            ref circularSelection);

 
                ms_circularSearchRegions[i] = new BinRegionSelection(ref circularSelection);
                circularSelection.Clear();
            }

            ms_pathFinder = new PathFinder();
            ms_pathFinder.Init(200);
        }


        // Most expensive version. Line of sight checks plus re-searches for the start and end node
        // based on world positions.
        public static bool CreatePath(Vector2 startPos, Vector2 endPos, Bin bin, ref PathRoute outRoute)
        {
            // Check for direct line of sight
            if(CollisionHelpers.IsClearLineOfSight(startPos, endPos - startPos, bin))
            {
                outRoute.SetPathInfo(startPos, endPos, null, null);
                return true;
            }


            PathNode endNode = null;
            GetClosestPathNode(endPos, bin, BinLayers.kPathNodes, ref endNode);

            if (endNode != null)
                return CreatePathNoLineOfSight(startPos, endPos, endNode, bin, ref outRoute);

            return false;
        }


        // Cheaper...
        public static bool CreatePathNoLineOfSight(Vector2 startPos, Vector2 endPos, PathNode endNode, Bin bin, ref PathRoute outRoute)
        {
            PathNode startNode = null;
            GetClosestPathNode(startPos, bin, BinLayers.kPathNodes, ref startNode);

            if (startNode != null)
                return CreatePathNoLineOfSight(startPos, startNode, endPos, endNode, ref outRoute);

            return false;
        }


        // Cheaper still...
        public static bool CreatePathNoLineOfSight(Vector2 startPos, PathNode startNode, Vector2 endPos, PathNode endNode, ref PathRoute outRoute)
        {

            // We have two nodes to form a path between.
            outRoute.SetPathInfo(startPos, endPos, startNode, endNode);

            bool foundPath = ms_pathFinder.FindPath(startNode, endNode, ref outRoute);

            return true;
        }


        // Use this wisely. Try to cache results for a frame etc. Its ok... but not amazingly quick.
        public static bool GetClosestPathNode(Vector2 pos, Bin bin, int layer, ref PathNode outNode)
        {
            // Use bins to track down limited selection of nodes.
            BinLocation binLocation = new BinLocation();
            bin.GetBinLocation(pos, ref binLocation);

            int posBinIndex = bin.GetBinIndex(pos);

            for (int i = 0; i < m_maxCircularSearchRegions; ++i)
            {
                BinQueryLocalityResults queryResults = bin.GetShaderQueryLocalityResults();
                queryResults.SetLocalityInfo(pos);
                queryResults.StartQuery();
                bin.Query(ref ms_circularSearchRegions[i], posBinIndex, layer, queryResults);
                queryResults.EndQuery();


                while (queryResults.BinItemCount > 0)
                {
                    int closestBinItemIdx = queryResults.GetClosestBinItemIndex();

                    PathNode pathNode = (PathNode)queryResults.BinItemQueryResults[closestBinItemIdx];

                    if (CollisionHelpers.IsClearLineOfSight(pos, pathNode.GetPosition() - pos, bin))
                    {
                        outNode = pathNode;
                        return true;
                    }
                    else
                    {
                        queryResults.RemoveBinItem(closestBinItemIdx);
                    }
                }
            }

            return false;
        }


        public static void DebugRender(DebugRenderer debugRenderer)
        {
            ms_pathFinder.DebugRender(debugRenderer);
        }
    }
}
