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
        public static void GenerateContacts(List<AiEntity> entities, Bin bin, ContactList contactList)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();

            float contactDimensionPadding = DynamicGameEntity.GetContactDimensionPadding();
            float doubleContactDimensionPadding = contactDimensionPadding * 2.0f;


            for (int i = 0; i < entities.Count; ++i)
            {
                AiEntity entity = entities[i];

                entity.GetBinRegion(ref entityRegion);

                bin.StartQuery();
                bin.Query(entityRegion, 0);
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


        // Temporary until we sort out inheritance on AIEntity and PlayerEntity
        public static void GenerateContacts(PlayerEntity entity, Bin bin, ContactList contactList)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();

            float contactDimensionPadding = DynamicGameEntity.GetContactDimensionPadding();
            float doubleContactDimensionPadding = contactDimensionPadding * 2.0f;



            entity.GetBinRegion(ref entityRegion);

            bin.StartQuery();
            bin.Query(entityRegion, 0);
            BinQueryResults queryResults = bin.EndQuery();


            for (int j = 0; j < queryResults.BinItemCount; ++j)
            {
                AiEntity checkEntity = (AiEntity)queryResults.BinItemQueryResults[j];

                bool contact = Math2D.DoesIntersect(entity.GetPosition(),
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


        public static void GenerateContacts(List<AiEntity> entities, List<BoundaryEntity> boundaries, Bin bin, ContactList contactList)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            BinRegionUniform boundaryRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();

            float contactDimensionPadding = DynamicGameEntity.GetContactDimensionPadding();


            for (int i = 0; i < entities.Count; ++i)
            {
                AiEntity entity = entities[i];

                entity.GetBinRegion(ref entityRegion);

                bin.StartQuery();
                bin.Query(entityRegion, 1);
                BinQueryResults queryResults = bin.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[j];
                    checkBoundary.GetBinRegion(ref boundaryRegion);


                    float paddedRadius = entity.GetContactRadiusInfo().Radius + contactDimensionPadding;
                    float paddedRadiusSq = paddedRadius * paddedRadius;

                    LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
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


        // Temporary until we sort out inheritance on AIEntity and PlayerEntity
        public static void GenerateContacts(PlayerEntity entity, List<BoundaryEntity> boundaries, Bin bin, ContactList contactList)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            BinRegionUniform boundaryRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();

            float contactDimensionPadding = DynamicGameEntity.GetContactDimensionPadding();


            entity.GetBinRegion(ref entityRegion);

            bin.StartQuery();
            bin.Query(entityRegion, 1);
            BinQueryResults queryResults = bin.EndQuery();


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


        public static void UpdateBulletContacts(List<AiEntity> entities, List<BulletEntity> bullets, Bin bin)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            BinRegionUniform bulletRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();


            for (int i = 0; i < entities.Count; ++i)
            {
                AiEntity entity = entities[i];

                entity.GetBinRegion(ref entityRegion);

                bin.StartQuery();
                bin.Query(entityRegion, 2);
                BinQueryResults queryResults = bin.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    BulletEntity checkBullet = (BulletEntity)queryResults.BinItemQueryResults[j];
                    checkBullet.GetBinRegion(ref bulletRegion);

                    Vector2 dPos = checkBullet.GetPosition() - checkBullet.GetLastFramePosition();
                    float dPosLengthSq = dPos.LengthSquared();

                    bool contact = Math2D.DoesIntersect(entity.GetPosition(),
                                                            entity.GetContactRadiusInfo().Radius,
                                                            entity.GetContactRadiusInfo().RadiusSq,
                                                            checkBullet.GetLastFramePosition(),
                                                            checkBullet.GetPosition(),
                                                            dPos,
                                                            dPosLengthSq,
                                                            ref intersectInfo);


                    if (contact)
                    {
                        entity.OnCollisionEvent(ref checkBullet);

                        checkBullet.OnCollisionEvent(ref entity);
                    }
                }
            }
        }


        public static void UpdateBulletContacts(List<BulletEntity> bullets, List<BoundaryEntity> boundaries, Bin bin)
        {
            BinRegionUniform bulletRegion = new BinRegionUniform();
            BinRegionUniform boundaryRegion = new BinRegionUniform();
            Vector2 intersectPoint = Vector2.Zero;


            float contactDimensionPadding = DynamicGameEntity.GetContactDimensionPadding();


            for (int i = 0; i < bullets.Count; ++i)
            {
                BulletEntity bullet = bullets[i];

                bullet.GetBinRegion(ref bulletRegion);

                bin.StartQuery();
                bin.Query(bulletRegion, 1);
                BinQueryResults queryResults = bin.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[j];
                    checkBoundary.GetBinRegion(ref boundaryRegion);

                    // TODO: Calculate per frame and store in bullet?
                    Vector2 dPos = bullet.GetPosition() - bullet.GetLastFramePosition();

                    LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
                    bool contact = Math2D.DoesIntersect(    bullet.GetPosition(),
                                                            dPos,
                                                            linePrimitive2D.Point,
                                                            linePrimitive2D.Difference,
                                                            ref intersectPoint );


                    if (contact)
                    {
                        bullet.OnCollisionEvent(ref checkBoundary);
                    }
                }
            }
        }


        public static void UpdateExplosions(List<AiEntity> entities, List<Explosion> explosions, Bin bin)
        {
            BinRegionUniform explosionRegion = new BinRegionUniform();
            BinRegionUniform entityRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();


            for (int i = 0; i < explosions.Count; ++i)
            {
                Explosion explosion = explosions[i];

                bin.GetBinRegionFromCentre(explosion.GetPosition(), explosion.GetRange(), ref explosionRegion);

                bin.StartQuery();
                bin.Query(explosionRegion, 0);
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
