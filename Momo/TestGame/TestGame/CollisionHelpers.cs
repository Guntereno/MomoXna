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
using TestGame.Objects;



namespace TestGame
{
    public class CollisionHelpers
    {
        private static readonly int kBinSelectionCapacity = 1000;
        private static BinRegionSelection ms_binRegion = new BinRegionSelection(kBinSelectionCapacity);


        public static void GenerateContacts(DynamicGameEntity[] entities, int entityCount, Bin bin, ContactList contactList)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            BinRegionUniform boundaryRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();

            float contactDimensionPadding = DynamicEntity.GetContactDimensionPadding();
            float doubleContactDimensionPadding = contactDimensionPadding * 2.0f;


            for (int i = 0; i < entityCount; ++i)
            {
                DynamicGameEntity entity = entities[i];

                entity.GetBinRegion(ref entityRegion);

                // Entities
                bin.StartQuery();
                bin.Query(entityRegion, BinLayers.kAiEntity);
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


                // Boundaries
                bin.StartQuery();
                bin.Query(entityRegion, BinLayers.kBoundary);
                queryResults = bin.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[j];
                    checkBoundary.GetBinRegion(ref boundaryRegion);


                    float paddedRadius = entity.GetContactRadiusInfo().Radius + contactDimensionPadding;
                    float paddedRadiusSq = paddedRadius * paddedRadius;

                    LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
                    bool contact = Math2D.DoesIntersect(entity.GetPosition(),
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


        public static bool IsClearLineOfSight(Vector2 position, Vector2 dPos, Bin bin)
        {
            BinRegionUniform boundaryRegion = new BinRegionUniform();
            Vector2 intersectPoint = Vector2.Zero;


            bin.GetBinRegionFromLine(position, dPos, ref ms_binRegion);

            // Boundaries
            bin.StartQuery();
            bin.Query(ms_binRegion, BinLayers.kBoundary);
            BinQueryResults queryResults = bin.EndQuery();

            for (int j = 0; j < queryResults.BinItemCount; ++j)
            {
                BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[j];
                checkBoundary.GetBinRegion(ref boundaryRegion);

                LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
                bool contact = Math2D.DoesIntersect(    position,
                                                        dPos,
                                                        linePrimitive2D.Point,
                                                        linePrimitive2D.Difference,
                                                        ref intersectPoint );


                if (contact)
                {
                    return false;
                }
            }
                
            return true;
        }


        public static void UpdateBulletContacts(BulletEntity[] bullets, int bulletCount, Bin bin)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            BinRegionUniform bulletRegion = new BinRegionUniform();
            BinRegionUniform boundaryRegion = new BinRegionUniform();

            IntersectInfo2D intersectInfo = new IntersectInfo2D();
            Vector2 intersectPoint = Vector2.Zero;


            float contactDimensionPadding = DynamicEntity.GetContactDimensionPadding();


            for (int i = 0; i < bulletCount; ++i)
            {
                BulletEntity bullet = bullets[i];

                bool bulletContact = false;
                float dPosLengthSq = bullet.GetPositionDifferenceFromLastFrame().LengthSquared();

                bullet.GetBinRegion(ref bulletRegion);


                // Entities
                bin.StartQuery();
                bin.Query(bulletRegion, BinLayers.kAiEntity);
                BinQueryResults queryResults = bin.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    AiEntity checkEntity = (AiEntity)queryResults.BinItemQueryResults[j];
                    checkEntity.GetBinRegion(ref entityRegion);



                    bool contact = Math2D.DoesIntersect(    checkEntity.GetPosition(),
                                                            checkEntity.GetContactRadiusInfo().Radius,
                                                            checkEntity.GetContactRadiusInfo().RadiusSq,
                                                            bullet.GetLastFramePosition(),
                                                            bullet.GetPosition(),
                                                            bullet.GetPositionDifferenceFromLastFrame(),
                                                            dPosLengthSq,
                                                            ref intersectInfo);


                    if (contact)
                    {
                        checkEntity.OnCollisionEvent(ref bullet);

                        bullet.OnCollisionEvent(ref checkEntity);
                        bulletContact = true;
                        break;
                    }
                }


                if (bulletContact)
                    continue;


                // Boundaries
                bin.StartQuery();
                bin.Query(bulletRegion, BinLayers.kBoundary);
                queryResults = bin.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[j];
                    checkBoundary.GetBinRegion(ref boundaryRegion);

                    LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
                    bool contact = Math2D.DoesIntersect(    bullet.GetLastFramePosition(),
                                                            bullet.GetPositionDifferenceFromLastFrame(),
                                                            linePrimitive2D.Point,
                                                            linePrimitive2D.Difference,
                                                            ref intersectPoint );


                    if (contact)
                    {
                        bullet.OnCollisionEvent(ref checkBoundary);
                        break;
                    }
                }
            }
        }


        public static void UpdateExplosions(List<Explosion> explosions, Bin bin)
        {
            BinRegionUniform explosionRegion = new BinRegionUniform();
            BinRegionUniform entityRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();


            for (int i = 0; i < explosions.Count; ++i)
            {
                Explosion explosion = explosions[i];

                bin.GetBinRegionFromCentre(explosion.GetPosition(), explosion.GetRange(), ref explosionRegion);

                bin.StartQuery();
                bin.Query(explosionRegion, BinLayers.kAiEntity);
                BinQueryResults queryResults = bin.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    AiEntity checkEntity = (AiEntity)queryResults.BinItemQueryResults[j];
                    checkEntity.GetBinRegion(ref entityRegion);

                    bool contact = Math2D.DoesIntersect(    explosion.GetPosition(),
                                                            explosion.GetRange(),
                                                            checkEntity.GetPosition(),
                                                            checkEntity.GetContactRadiusInfo().Radius,
                                                            ref intersectInfo);


                    if (contact)
                    {
                        Vector2 contactNormal = -(intersectInfo.PositionDifference / intersectInfo.PositionDistance);

                        float force = (1.0f - (intersectInfo.PositionDistance / explosion.GetRange())) * explosion.GetForce();

                        checkEntity.OnExplosionEvent(ref explosion, contactNormal * force);
                    }
                }
            }
        }
    }
}
