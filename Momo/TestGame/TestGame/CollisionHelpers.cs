using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Maths;
using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.Spatial;

using Momo.Core.Primitive2D;
using Momo.Core.Collision2D;

using Game.Entities;



namespace Game
{
    public class CollisionHelpers
    {
        private const int kBinSelectionCapacity = 1000;
        private static BinRegionSelection ms_tempBinRegionSelection = new BinRegionSelection(kBinSelectionCapacity);


        public static void Init()
        {

        }


        public static void GenerateEntityContacts(GameEntity[] entities, int entityCount, float sizeMod, Bin bin, int entityLayer, ContactList contactList)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();

            float contactDimensionPadding = DynamicEntity.ContactDimensionPadding;
            float doubleContactDimensionPadding = contactDimensionPadding * 2.0f;


            for (int i = 0; i < entityCount; ++i)
            {
                GameEntity entity = entities[i];

                entity.GetBinRegion(ref entityRegion);

                // Entities
                BinQueryResults queryResults = bin.GetShaderQueryResults();
                queryResults.StartQuery();
                bin.Query(ref entityRegion, entityLayer, queryResults);
                queryResults.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    GameEntity checkEntity = (GameEntity)queryResults.BinItemQueryResults[j];

                    if (entity != checkEntity)
                    {
                        bool contact = Maths2D.DoesIntersect(   entity.GetPosition(),
                                                                ( entity.ContactRadiusInfo.Radius * sizeMod ) + contactDimensionPadding,
                                                                checkEntity.GetPosition(),
                                                                checkEntity.ContactRadiusInfo.Radius + contactDimensionPadding,
                                                                ref intersectInfo);

                        if (contact)
                        {
                            Contact existingContact = contactList.GetContact(checkEntity, entity);

                            if (existingContact == null)
                            {
                                Vector2 contactNormal = intersectInfo.PositionDifference / intersectInfo.PositionDistance;
                                float contactOverlap = (intersectInfo.ResolveDistance - intersectInfo.PositionDistance) - doubleContactDimensionPadding;
                                contactList.AddContact(entity, checkEntity, contactNormal, contactOverlap, 1.0f, 0.9f);
                            }
                        }
                    }
                }
            }
        }


        public static void GenerateBoundaryContacts(GameEntity[] entities, int entityCount, Bin bin, int boundaryLayer, ContactList contactList)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            BinRegionUniform boundaryRegion = new BinRegionUniform();
            IntersectInfo2D intersectInfo = new IntersectInfo2D();

            float contactDimensionPadding = DynamicEntity.ContactDimensionPadding;


            for (int i = 0; i < entityCount; ++i)
            {
                GameEntity entity = entities[i];

                entity.GetBinRegion(ref entityRegion);

                // Boundaries
                BinQueryResults queryResults = bin.GetShaderQueryResults();
                queryResults.StartQuery();
                bin.Query(ref entityRegion, boundaryLayer, queryResults);
                queryResults.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[j];
                    checkBoundary.GetBinRegion(ref boundaryRegion);


                    float paddedRadius = entity.ContactRadiusInfo.Radius + contactDimensionPadding;
                    float paddedRadiusSq = paddedRadius * paddedRadius;

                    LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;
                    bool contact = Maths2D.DoesIntersect(   entity.GetPosition(),
                                                            paddedRadius,
                                                            paddedRadiusSq,
                                                            linePrimitive2D.Point,
                                                            linePrimitive2D.Point + linePrimitive2D.Difference,
                                                            linePrimitive2D.Difference,
                                                            linePrimitive2D.LengthSq,
                                                            ref intersectInfo);


                    if (contact)
                    {
                        Vector2 contactNormal = -(intersectInfo.PositionDifference / intersectInfo.PositionDistance);
                        float contactOverlap = (intersectInfo.ResolveDistance - intersectInfo.PositionDistance) - contactDimensionPadding;
                        contactList.AddContact(entity, null, contactNormal, contactOverlap, 1.0f, 0.4f);
                    }
                }
            }
        }


        // This method needs to check all the contacts in one method so it can check which came first, so only that reacts.
        public static void GenerateProjectileContacts(BulletEntity[] bullets, int bulletCount, Bin bin, int[] entityLayers, int boundaryLayer)
        {
            BinRegionUniform entityRegion = new BinRegionUniform();
            BinRegionUniform bulletRegion = new BinRegionUniform();
            BinRegionUniform boundaryRegion = new BinRegionUniform();


            float contactDimensionPadding = DynamicEntity.ContactDimensionPadding;


            for (int i = 0; i < bulletCount; ++i)
            {
                BulletEntity bullet = bullets[i];

                float dPosLengthSq = bullet.PositionDifferenceFromLastFrame.LengthSquared();

                bullet.GetBinRegion(ref bulletRegion);


                float boundaryContactDelta = float.MaxValue;
                float entityContactDelta = float.MaxValue;

                BoundaryEntity boundaryContact = null;
                GameEntity entityContact = null;


                // Boundaries
                BinQueryResults queryResults = bin.GetShaderQueryResults();
                queryResults.StartQuery();
                bin.Query(ref bulletRegion, boundaryLayer, queryResults);
                queryResults.EndQuery();


                for (int j = 0; j < queryResults.BinItemCount; ++j)
                {
                    BoundaryEntity checkBoundary = (BoundaryEntity)queryResults.BinItemQueryResults[j];
                    checkBoundary.GetBinRegion(ref boundaryRegion);

                    LinePrimitive2D linePrimitive2D = checkBoundary.CollisionPrimitive;


                    float contactDelta = float.MaxValue;

                    bool contact = Maths2D.DoesIntersect(   bullet.LastFramePosition,
                                                            bullet.PositionDifferenceFromLastFrame,
                                                            linePrimitive2D.Point,
                                                            linePrimitive2D.Difference,
                                                            ref contactDelta);


                    // Is there a contact and is it closer than last contact.
                    if (contact && (contactDelta < boundaryContactDelta))
                    {
                        boundaryContactDelta = contactDelta;
                        boundaryContact = checkBoundary;
                    }
                }


                // Check all entity layers
                for (int j = 0; j < entityLayers.Length; ++j)
                {
                    int entityLayer = entityLayers[j];

                    queryResults = bin.GetShaderQueryResults();
                    queryResults.StartQuery();
                    bin.Query(ref bulletRegion, entityLayer, queryResults);
                    queryResults.EndQuery();


                    for (int k = 0; k < queryResults.BinItemCount; ++k)
                    {
                        GameEntity checkEntity = (GameEntity)queryResults.BinItemQueryResults[k];
                        checkEntity.GetBinRegion(ref entityRegion);

                        float contactDelta = float.MaxValue;

                        bool contact = Maths2D.DoesIntersect(   checkEntity.GetPosition(),
                                                                checkEntity.ContactRadiusInfo.Radius,
                                                                checkEntity.ContactRadiusInfo.RadiusSq,
                                                                bullet.LastFramePosition,
                                                                bullet.GetPosition(),
                                                                bullet.PositionDifferenceFromLastFrame,
                                                                dPosLengthSq,
                                                                ref contactDelta);


                        // Is there a contact and is it closer than last contact.
                        if (contact && (contactDelta < entityContactDelta))
                        {
                            entityContactDelta = contactDelta;
                            entityContact = checkEntity;
                        }
                    }
                }


                if (entityContact != null && (entityContactDelta <= boundaryContactDelta))
                {
                    entityContact.OnCollisionEvent(ref bullet);
                    bullet.OnCollisionEvent(ref entityContact);
                }
                else if (boundaryContact != null && (boundaryContactDelta < entityContactDelta))
                {
                    bullet.OnCollisionEvent(ref boundaryContact);
                }
            }
        }


        public static bool IsClearLineOfSightBoundary(Vector2 position, Vector2 dPos, Bin bin, int boundaryLayer)
        {
            BinRegionUniform boundaryRegion = new BinRegionUniform();

            bin.GetBinRegionFromLine(position, dPos, ref ms_tempBinRegionSelection);

            // Boundaries
            BinQueryResults queryResults = bin.GetShaderQueryResults();
            queryResults.StartQuery();
            bin.Query(ref ms_tempBinRegionSelection, boundaryLayer, queryResults);
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


        //public static void UpdateExplosions(List<Explosion> explosions, Bin bin, int entityLayer)
        //{
        //    BinRegionUniform explosionRegion = new BinRegionUniform();
        //    BinRegionUniform entityRegion = new BinRegionUniform();
        //    IntersectInfo2D intersectInfo = new IntersectInfo2D();


        //    for (int i = 0; i < explosions.Count; ++i)
        //    {
        //        Explosion explosion = explosions[i];

        //        bin.GetBinRegionFromCentre(explosion.GetPosition(), explosion.GetRange(), ref explosionRegion);

        //        BinQueryResults queryResults = bin.GetShaderQueryResults();
        //        queryResults.StartQuery();
        //        bin.Query(ref explosionRegion, entityLayer, queryResults);
        //        queryResults.EndQuery();


        //        for (int j = 0; j < queryResults.BinItemCount; ++j)
        //        {
        //            AiEntity checkEntity = (AiEntity)queryResults.BinItemQueryResults[j];
        //            checkEntity.GetBinRegion(ref entityRegion);

        //            bool contact = Maths2D.DoesIntersect(    explosion.GetPosition(),
        //                                                    explosion.GetRange(),
        //                                                    checkEntity.GetPosition(),
        //                                                    checkEntity.ContactRadiusInfo.Radius,
        //                                                    ref intersectInfo);


        //            if (contact)
        //            {
        //                Vector2 contactNormal = -(intersectInfo.PositionDifference / intersectInfo.PositionDistance);

        //                float force = (1.0f - (intersectInfo.PositionDistance / explosion.GetRange())) * explosion.GetForce();

        //                checkEntity.OnExplosionEvent(ref explosion, contactNormal * force);
        //            }
        //        }
        //    }
        //}
    }
}
