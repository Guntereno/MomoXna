using System;

using Microsoft.Xna.Framework;

using Momo.Maths;
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
        private static float ms_maxCircularSearchRange = 1000.0f;
        private static int m_maxCircularSearchRegions = 5;
        public static BinRegionSelection [] ms_circularSearchRegions = null;


        public static void Init(float maxCircularSearchRange, int maxCircularSearchRegions, Bin bin)
        {
            ms_maxCircularSearchRange = maxCircularSearchRange;
            m_maxCircularSearchRegions = maxCircularSearchRegions;
            ms_circularSearchRegions = new BinRegionSelection[maxCircularSearchRegions];

            float circularSelectionRadiusBands = 50.0f;// ms_maxCircularSearchRange / (float)m_maxCircularSearchRegions;
            BinRegionSelection emptyMinusSelection = new BinRegionSelection(0);
            BinRegionSelection circularSelection = new BinRegionSelection(1000);
            BinLocation centre = new BinLocation(12, 20);

            for (int i = 0; i < maxCircularSearchRegions; ++i)
            {
                //if (i != 0)
                //{
                //    bin.GetBinRegionFromCircle(centre,
                //                                circularSelectionRadiusBands * (float)(i + 1), 0.1f,
                //                                ref ms_circularSearchRegions[i - 1],
                //                                ref circularSelection);
                //}
                //else
                //{
                //    bin.GetBinRegionFromCircle(centre,
                //                                circularSelectionRadiusBands * (float)(i + 1), 0.1f,
                //                                ref emptyMinusSelection,
                //                                ref circularSelection);
                //}
 
                ms_circularSearchRegions[i] = new BinRegionSelection(ref circularSelection);
                circularSelection.Clear();
            }
        }


        public static bool CreatePath(Vector2 startPos, Vector2 endPos, Bin bin, ref PathRoute outRoute)
        {
            // Check for direct line of sight
            if(CollisionHelpers.IsClearLineOfSight(startPos, endPos - startPos, bin))
            {
                outRoute.SetPathInfo(startPos, endPos, null, null);
                return true;
            }


            PathNode startNode = null;
            PathNode endNode = null;

            GetClosestPathNode(startPos, bin, BinLayers.kPathNodes, ref startNode);

            if (startNode != null)
            {
                GetClosestPathNode(endPos, bin, BinLayers.kPathNodes, ref endNode);

                if (endNode != null)
                {
                    // We have two nodes to form a path between.
                    outRoute.SetPathInfo(startPos, endPos, startNode, endNode);


                    return true;
                }
            }


            return false;
        }


        public static bool GetClosestPathNode(Vector2 pos, Bin bin, int layer, ref PathNode outNode)
        {
            // Use bins to track down limited selection of nodes.
            BinLocation binLocation = new BinLocation();
            bin.GetBinLocation(pos, ref binLocation);


            bin.StartQuery();
            //bin.QuerySearch(1, ref binLocation, layer);
            BinQueryResults queryResults = bin.EndQuery();


            for (int i = 0; i < queryResults.BinItemCount; ++i)
            {
                PathNode pathNode = (PathNode)queryResults.BinItemQueryResults[i];

                outNode = pathNode;
                return true;
            }

            //AddMethod to bin that spiral searches. It searches out until it gets a hit. It keeps the search tho
            // until it has finished with the whole layer. Then if the results are no good, you can tell it to start searching
            // again from the next layer.


            return false;
        }
    }
}
