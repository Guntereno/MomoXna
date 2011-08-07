using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.Spatial;

using Momo.Core.Primitive2D;
using Momo.Core.Collision2D;

using TestGame.Entities;



namespace TestGame
{
    public class GameHelpers
    {
        public static void GenerateContacts(List<AiEntity> entities, Bin bin, ContactList contactList)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();

            float contactDimensionPadding = DynamicGameEntity.GetContactDimensionPadding();
            float doubleContactDimensionPadding = contactDimensionPadding * 2.0f;


            for (int i = 0; i < entities.Count; ++i)
            {
                AiEntity entity = entities[i];

                // TODO: Pad the bin region slightly so that we generate near by contact pairs.
                entity.GetBinRegion(ref entityRegion);

                bin.StartQuery();
                bin.Query(entityRegion);
                BinQueryResults queryResults = bin.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    AiEntity checkEntity = (AiEntity)queryResults.BinItemQueryResults[j];

                    if (entity != checkEntity)
                    {
                        bool contact = Math2D.DoesIntersect(    entity.GetPosition(),
                                                                entity.GetContactRadiusInfo().Radius + contactDimensionPadding,
                                                                checkEntity.GetPosition(),
                                                                checkEntity.GetContactRadiusInfo().Radius + contactDimensionPadding,
                                                                ref intersectInfo);

                        if (contact)
                        {
                            Contact existingContact = contactList.GetContact(checkEntity, entity);

                            if (existingContact == null)
                            {
                                Vector2 contactNormal = intersectInfo.PositionDifference / intersectInfo.PositionDistance;
                                float contactOverlap = (intersectInfo.ResolveDistance - intersectInfo.PositionDistance) - doubleContactDimensionPadding;
                                contactList.AddContact(entity, checkEntity, contactNormal, contactOverlap, 1.0f, 0.0f);
                            }
                        }
                    }
                }
            }
        }


        public static void GenerateContacts(List<AiEntity> entities, List<BoundaryEntity> boundaries, Bin bin, ContactList contactList)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            BinRegionUniform boundaryRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();

            float contactDimensionPadding = DynamicGameEntity.GetContactDimensionPadding();


            for (int i = 0; i < entities.Count; ++i)
            {
                AiEntity entity = entities[i];

                // TODO: Pad the bin region slightly so that we generate near by contact pairs.
                entity.GetBinRegion(ref entityRegion);


                for (int j = 0; j < boundaries.Count; ++j)
                {
                    BoundaryEntity checkBoundary = boundaries[j];
                    checkBoundary.GetBinRegion(ref boundaryRegion);

                    // Is the entity within the overall boundary entity
                    if (entityRegion.IsInRegion(boundaryRegion) == true)
                    {
                        for (int k = 0; k < checkBoundary.CollisionPrimitive.LineCount; ++k)
                        {
                            if (entityRegion.IsInRegion(checkBoundary.CollisionLineBinRegions[k]) == true)
                            {
                                float paddedRadius = entity.GetContactRadiusInfo().Radius + contactDimensionPadding;
                                float paddedRadiusSq = paddedRadius * paddedRadius;

                                LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive.LineList[k];
                                bool contact = Math2D.DoesIntersect(    entity.GetPosition(),
                                                                        paddedRadius,
                                                                        paddedRadiusSq,
                                                                        linePrimitive2D.Point,
                                                                        linePrimitive2D.Point + linePrimitive2D.Difference,
                                                                        linePrimitive2D.Difference,
                                                                        linePrimitive2D.LengthSqList,
                                                                        ref intersectInfo);


                                if (contact)
                                {
                                    Vector2 contactNormal = -(intersectInfo.PositionDifference / intersectInfo.PositionDistance);
                                    float contactOverlap = (intersectInfo.ResolveDistance - intersectInfo.PositionDistance) - contactDimensionPadding;
                                    contactList.AddContact(entity, null, contactNormal, contactOverlap, 1.0f, 0.0f);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
