using System;

using Microsoft.Xna.Framework;

using Momo.Core.Spatial;
using Momo.Core.GameEntities;
using Momo.Core.Primitive2D;
using Momo.Core.Collision2D;

using Momo.Maths;

using Game;
using Game.Entities;



namespace Game.Ai.AiEntityStates
{
    public class AiEntityStateHelpers
    {
        static BinQueryResults msQueryResults1 = new BinQueryResults(500);
        static BinQueryResults msQueryResults2 = new BinQueryResults(500);
        static BinRegionSelection msBinRegionSelection = new BinRegionSelection(500);


        public static Vector2 GetForceFromSurroundingEntities(float radius, float radiusSqrd, AiEntity entity, int entityLayer)
        {
            Vector2 force = Vector2.Zero;

            Bin bin = entity.Zone.Bin;
            BinRegionUniform entityRegion = new BinRegionUniform();
            bin.GetBinRegionFromCentre(entity.GetPosition(), radius, ref entityRegion);

            // Entities
            msQueryResults1.StartQuery();
            bin.Query(ref entityRegion, entityLayer, msQueryResults1);
            msQueryResults1.EndQuery();


            for (int j = 0; j < msQueryResults1.BinItemCount; ++j)
            {
                AiEntity checkEntity = (AiEntity)msQueryResults1.BinItemQueryResults[j];

                if (checkEntity != entity)
                {
                    Vector2 dPositions = checkEntity.GetPosition() - entity.GetPosition();
                    float distSqrd = dPositions.LengthSquared();

                    if (distSqrd < radiusSqrd)
                    {
                        float dist = (float)System.Math.Sqrt(distSqrd);
                        Vector2 direction = dPositions / dist;

                        float forceMod = 1.0f - (dist / radius);
                        force += (direction * forceMod);

                        //float directionMod = -Vector2.Dot(direction, entity.FacingDirection);
                        //if (directionMod < 0.0f)
                        //    directionMod = 0.0f;

                        //force += (direction * forceMod * directionMod);
                    }
                }
            }


            return force;
        }


        public static Vector2 GetAveragePositionFromSurroundingEntities(float radius, float radiusSqrd, AiEntity entity, int entityLayer)
        {
            Vector2 averagePosition = entity.GetPosition();
            int averagePositionCnt = 1;

            Bin bin = entity.Zone.Bin;
            BinRegionUniform entityRegion = new BinRegionUniform();
            bin.GetBinRegionFromCentre(entity.GetPosition(), radius, ref entityRegion);

            // Entities
            msQueryResults1.StartQuery();
            bin.Query(ref entityRegion, entityLayer, msQueryResults1);
            msQueryResults1.EndQuery();


            for (int j = 0; j < msQueryResults1.BinItemCount; ++j)
            {
                AiEntity checkEntity = (AiEntity)msQueryResults1.BinItemQueryResults[j];

                if (checkEntity != entity)
                {
                    Vector2 dPositions = checkEntity.GetPosition() - entity.GetPosition();
                    float distSqrd = dPositions.LengthSquared();

                    if (distSqrd < radiusSqrd)
                    {
                        averagePosition += checkEntity.GetPosition();
                        ++averagePositionCnt;
                    }
                }
            }


            return averagePosition / (float)averagePositionCnt;
        }


        public static bool GetClosestEntityInRange( float sighthRadius, float hearRadius, float viewDot, AiEntity entity, int [] entityLayer, int viewLayer,
                                                    ref GameEntity outClosestEntity, ref Vector2 outDPostion)
        {
            //
            // Make list of entities in range, and within view dot. Then work closest backwards to do a line of sight check.
            //
            float sightRadiusSqrd = sighthRadius * sighthRadius;
            float hearRadiusSqrd = hearRadius * hearRadius;

            float closestRadiusSqrd = sightRadiusSqrd;
            GameEntity closestEntity = null;


            Bin bin = entity.Zone.Bin;
            BinRegionUniform entityRegion = new BinRegionUniform();
            bin.GetBinRegionFromCentre(entity.GetPosition(), sighthRadius, ref entityRegion);

            // Entities
            msQueryResults1.StartQuery();
            for (int i = 0; i < entityLayer.Length; ++i)
            {
                bin.Query(ref entityRegion, entityLayer[i], msQueryResults1);
            }
            msQueryResults1.EndQuery();


            for (int i = 0; i < msQueryResults1.BinItemCount; ++i)
            {
                GameEntity checkEntity = (GameEntity)msQueryResults1.BinItemQueryResults[i];

                if (checkEntity != entity)
                {
                    Vector2 dPosition = checkEntity.GetPosition() - entity.GetPosition();
                    float distSqrd = dPosition.LengthSquared();

                    if (distSqrd < closestRadiusSqrd)
                    {
                        float dist = (float)Math.Sqrt(distSqrd);
                        Vector2 dPositionNorm = dPosition / dist;

                        if (distSqrd < hearRadiusSqrd || Vector2.Dot(dPositionNorm, entity.FacingDirection) > viewDot)
                        {
                            if (IsClearLineOfSightBoundary(entity.GetPosition(), dPosition, bin, viewLayer, msQueryResults2, msBinRegionSelection))
                            {
                                closestRadiusSqrd = distSqrd;
                                closestEntity = checkEntity;
                            }
                        }
                    }
                }
            }


            if (closestEntity != null)
            {
                outClosestEntity = closestEntity;
                outDPostion = closestEntity.GetPosition() - entity.GetPosition();
                return true;
            }

            return false;
        }


        public static void GetEntitiesInRange(  float sighthRadius, float viewDot, AiEntity entity, int[] entityLayer, int viewLayer,
                                                ref GameEntity[] outViewList, ref int outViewListCount, int viewListCapacity )
        {
            float sightRadiusSqrd = sighthRadius * sighthRadius;

            int viewListCnt = 0;

            Bin bin = entity.Zone.Bin;
            BinRegionUniform entityRegion = new BinRegionUniform();
            bin.GetBinRegionFromCentre(entity.GetPosition(), sighthRadius, ref entityRegion);

            // Entities
            msQueryResults1.StartQuery();
            for (int i = 0; i < entityLayer.Length; ++i)
            {
                bin.Query(ref entityRegion, entityLayer[i], msQueryResults1);
            }
            msQueryResults1.EndQuery();


            for (int i = 0; i < msQueryResults1.BinItemCount; ++i)
            {
                if(viewListCnt >= viewListCapacity)
                {
                    break;
                }

                GameEntity checkEntity = (GameEntity)msQueryResults1.BinItemQueryResults[i];

                if (checkEntity != entity)
                {
                    Vector2 dPosition = checkEntity.GetPosition() - entity.GetPosition();
                    float distSqrd = dPosition.LengthSquared();
                    float dist = (float)Math.Sqrt(distSqrd);
                    Vector2 dPositionNorm = dPosition / dist;

                    if(Vector2.Dot(dPositionNorm, entity.FacingDirection) > viewDot)
                    {
                        if (IsClearLineOfSightBoundary(entity.GetPosition(), dPosition, bin, viewLayer, msQueryResults2, msBinRegionSelection))
                        {
                            outViewList[viewListCnt] = checkEntity;
                            ++viewListCnt;
                        }
                    }
                }
            }

            outViewListCount = viewListCnt;
        }


        public static Vector2 GetForceFromSurroundingBoundaries(float radius, float radiusSqrd, AiEntity entity, int entityLayer)
        {
            Vector2 force = Vector2.Zero;

            Bin bin = entity.Zone.Bin;
            BinRegionUniform entityRegion = new BinRegionUniform();
            bin.GetBinRegionFromCentre(entity.GetPosition(), radius, ref entityRegion);

            // Entities
            BinQueryResults queryResults = bin.GetShaderQueryResults();
            queryResults.StartQuery();
            bin.Query(ref entityRegion, entityLayer, queryResults);
            queryResults.EndQuery();


            for (int j = 0; j < queryResults.BinItemCount; ++j)
            {
                BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[j];

                Vector2 closestPointOnLine = Maths2D.NearestPointOntoLine(entity.GetPosition(), checkBoundary.CollisionPrimitive.Point, checkBoundary.CollisionPrimitive.Difference);

                Vector2 dPositions = entity.GetPosition() - closestPointOnLine;
                float distSqrd = dPositions.LengthSquared();

                if (distSqrd < radiusSqrd)
                {
                    float dist = (float)System.Math.Sqrt(distSqrd);
                    Vector2 direction = dPositions / dist;



                    float forceMod = 1.0f - (dist / radius);
                    force += (direction * forceMod);

                    //float directionMod = -Vector2.Dot(direction, entity.FacingDirection);
                    //if (directionMod < 0.0f)
                    //    directionMod = 0.0f;

                    //force += (direction * forceMod * directionMod);
                }
            }


            return force;
        }


        public static BinQueryResults GetBinItems(float radius, Vector2 position, Bin bin, int binLayer)
        {
            BinRegionUniform region = new BinRegionUniform();
            bin.GetBinRegionFromCentre(position, radius, ref region);

            // Entities
            BinQueryResults queryResults = bin.GetShaderQueryResults();
            queryResults.StartQuery();
            bin.Query(ref region, binLayer, queryResults);
            queryResults.EndQuery();

            return queryResults;
        }


        public static bool IsClearLineOfSightBoundary(Vector2 position, Vector2 dPos, Bin bin, int boundaryLayer)
        {
            return IsClearLineOfSightBoundary(position, dPos, bin, boundaryLayer, msQueryResults1, msBinRegionSelection);
        }


        public static bool IsClearLineOfSightBoundary(Vector2 position, Vector2 dPos, Bin bin, int boundaryLayer, BinQueryResults queryResults, BinRegionSelection binRegionSelection)
        {
            BinRegionUniform boundaryRegion = new BinRegionUniform();

            bin.GetBinRegionFromLine(position, dPos, ref binRegionSelection);

            // Boundaries
            queryResults.StartQuery();
            bin.Query(ref binRegionSelection, boundaryLayer, queryResults);
            queryResults.EndQuery();

            for (int i = 0; i < queryResults.BinItemCount; ++i)
            {
                BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[i];
                checkBoundary.GetBinRegion(ref boundaryRegion);

                LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
                bool contact = Maths2D.DoesIntersect(position,
                                                        dPos,
                                                        linePrimitive2D.Point,
                                                        linePrimitive2D.Difference);


                if (contact)
                {
                    return false;
                }
            }

            return true;
        }


        public static int CountBinItems(Vector2 position, Vector2 dPos, Bin bin, int binLayer)
        {
            return CountBinItems(position, dPos, bin, binLayer, msQueryResults1, msBinRegionSelection);
        }


        public static int CountBinItems(Vector2 position, Vector2 dPos, Bin bin, int boundaryLayer, BinQueryResults queryResults, BinRegionSelection binRegionSelection)
        {
            bin.GetBinRegionFromLine(position, dPos, ref binRegionSelection);

            // Boundaries
            queryResults.StartQuery();
            bin.Query(ref binRegionSelection, boundaryLayer, queryResults);
            queryResults.EndQuery();

            return queryResults.BinItemCount;
        }
    }
}
