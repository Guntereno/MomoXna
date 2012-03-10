using System;

using Microsoft.Xna.Framework;

using Momo.Core.Spatial;
using Momo.Core.GameEntities;

using Momo.Maths;

using TestGame.Entities;



namespace TestGame.Ai.AiEntityStates
{
    public class AiEntityStateHelpers
    {
        public static Vector2 GetForceFromSurroundingEntities(float radius, float radiusSqrd, AiEntity entity, int entityLayer)
        {
            Vector2 force = Vector2.Zero;

            Bin bin = entity.World.Bin;
            BinRegionUniform entityRegion = new BinRegionUniform();
            bin.GetBinRegionFromCentre(entity.GetPosition(), radius, ref entityRegion);

            // Entities
            BinQueryResults queryResults = bin.GetShaderQueryResults();
            queryResults.StartQuery();
            bin.Query(ref entityRegion, entityLayer, queryResults);
            queryResults.EndQuery();


            for (int j = 0; j < queryResults.BinItemCount; ++j)
            {
                AiEntity checkEntity = (AiEntity)queryResults.BinItemQueryResults[j];

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

            Bin bin = entity.World.Bin;
            BinRegionUniform entityRegion = new BinRegionUniform();
            bin.GetBinRegionFromCentre(entity.GetPosition(), radius, ref entityRegion);

            // Entities
            BinQueryResults queryResults = bin.GetShaderQueryResults();
            queryResults.StartQuery();
            bin.Query(ref entityRegion, entityLayer, queryResults);
            queryResults.EndQuery();


            for (int j = 0; j < queryResults.BinItemCount; ++j)
            {
                AiEntity checkEntity = (AiEntity)queryResults.BinItemQueryResults[j];

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


        public static Vector2 GetForceFromSurroundingBoundaries(float radius, float radiusSqrd, AiEntity entity, int entityLayer)
        {
            Vector2 force = Vector2.Zero;

            Bin bin = entity.World.Bin;
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


        //public static bool CanSeeEntity(GameEntity entity1, GameEntity entity2, Bin bin, int boundaryLayer)
        //{
        //    BinRegionUniform boundaryRegion = new BinRegionUniform();

        //    bin.GetBinRegionFromLine(position, dPos, ref ms_tempBinRegionSelection);

        //     Boundaries
        //    BinQueryResults queryResults = bin.GetShaderQueryResults();
        //    queryResults.StartQuery();
        //    bin.Query(ref ms_tempBinRegionSelection, boundaryLayer, queryResults);
        //    queryResults.EndQuery();

        //    for (int i = 0; i < queryResults.BinItemCount; ++i)
        //    {
        //        BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[i];
        //        checkBoundary.GetBinRegion(ref boundaryRegion);

        //        LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
        //        bool contact = Maths2D.DoesIntersect(position,
        //                                                dPos,
        //                                                linePrimitive2D.Point,
        //                                                linePrimitive2D.Difference);


        //        if (contact)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

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
    }
}
