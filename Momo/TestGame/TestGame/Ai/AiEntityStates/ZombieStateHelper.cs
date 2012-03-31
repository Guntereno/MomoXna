using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;
using Momo.Core.Spatial;

using Game.Entities;



namespace Game.Ai.AiEntityStates
{
    public class ZombieStateHelper
    {

        public static void UpdateViewList(AiEntity entity, float seeRadius, float viewDot, ref GameEntity[] entityList, ref int entityListCnt, int entityListCapacity, uint updateToken, uint updateOffset, uint updateFreq)
        {
            // Clear up the list first.
            for (int i = 0; i < entityListCnt; ++i)
            {
                if (entityList[i].IsDestroyed())
                {
                    entityList[i] = null;
                    --entityListCnt;

                    if (entityListCnt > 0)
                    {
                        entityList[i] = entityList[entityListCnt];
                        --i;
                    }
                }
            }


            if ((updateToken + updateOffset) % updateFreq == 0)
            {
                int listCnt = 0;

                AiEntityStateHelpers.GetEntitiesInRange(seeRadius, viewDot, entity, BinLayers.kEnemyList, BinLayers.kBoundaryOcclusionSmall,
                                                        ref entityList, ref listCnt, entityListCapacity);


                for (int i = listCnt; i < entityListCnt; ++i)
                {
                    entityList[i] = null;
                }

                entityListCnt = listCnt;
            }
        }


        public static void TurnTowardsAndWalk(AiEntity entity, Vector2 walkForce, float turnSpeed, float dt)
        {
            float walkForceLenSqrd = walkForce.LengthSquared();
            if (walkForceLenSqrd > 0.0f)
            {
                float walkDirectionLen = (float)System.Math.Sqrt(walkForceLenSqrd);
                Vector2 walkDirection = walkForce / walkDirectionLen;
                entity.TurnTowardsAndWalk(walkDirection, turnSpeed, entity.Speed * dt);
            }
        }



        public static Vector2 CalculateBoundaryRepelForce(AiEntity entity, float forceStr, float radius, uint updateToken, uint updateOffset, uint updateFreq)
        {
            Vector2 force = Vector2.Zero;

            // Boundary repel
            if ((updateToken + updateOffset) % updateFreq == 0)
            {
                force = AiEntityStateHelpers.GetForceFromSurroundingBoundaries(radius, radius * radius, entity, BinLayers.kBoundary);
                force *= forceStr;
            }

            return force;
        }


        public static Vector2 CalculateEnemyRepelForce(AiEntity entity, float forceStr, float radius, uint updateToken, uint updateOffset, uint updateFreq)
        {
            Vector2 force = Vector2.Zero;

            // Entity repel
            if ((updateToken + updateOffset) % updateFreq == 0)
            {
                force = -AiEntityStateHelpers.GetForceFromSurroundingEntities(radius, radius * radius, entity, BinLayers.kEnemyEntities);
                force *= forceStr;
            }

            return force;
        }


        public static GameEntity CheckForFriendly(AiEntity entity, uint updateToken)
        {
            Zone zone = entity.Zone;

            float kEntitySightRadius = 800.0f;
            float kEntityHearRadius = 300.0f;
            float kEntityViewDot = -0.15f;


            // ---- Search for nearest entity to chase ----
            if ((updateToken + 2) % 5 == 0)
            {
                GameEntity closestEntity = null;
                Vector2 closetDPosition = Vector2.Zero;
                if (AiEntityStateHelpers.GetClosestEntityInRange(kEntitySightRadius, kEntityHearRadius, kEntityViewDot, entity, BinLayers.kFriendyList, BinLayers.kBoundaryOcclusionSmall, ref closestEntity, ref closetDPosition))
                {
                    return closestEntity;
                }
            }

            return null;
        }
    }
}
