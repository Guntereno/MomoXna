using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;
using Momo.Core.Spatial;

using Game.Entities;




namespace Game.Ai.AiEntityStates
{
    public class ZombieHerdState : TimedState
    {
        public State IdleState { get; set; }
        public State ChaseState { get; set; }


        public ZombieHerdState(AiEntity entity) :
            base(entity)
        {
            DebugColor = new Color(0.6f, 0.0f, 0.0f, 0.7f);
        }


        public override string ToString()
        {
            return "Zombie herd";
        }

        
        public override void OnEnter()
        {
            base.OnEnter();

            AiEntity.Speed = AiEntity.BaseSpeed;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            Zone world = AiEntity.Zone;
            Random random = world.Random;


            RadiusInfo velocityAlignmentRadius = new RadiusInfo(200.0f);
            float velocityAlignmentStr = 1.0f;
            float averagePositionStr = 0.1f;

            float kEntityRepelRadius = AiEntity.ContactRadiusInfo.Radius * 3.0f;
            float kEntityRepelStr = 5.0f;
            float kBoundaryRepelRadius = AiEntity.ContactRadiusInfo.Radius * 2.0f;
            float kBoundaryRepelStr = 1.0f;
            float kEntitySightRadius = 800.0f;
            float kEntityHearRadius = 250.0f;


            Vector2 forceAveragePosition = Vector2.Zero;
            Vector2 forceAverageVelocity = Vector2.Zero;
            Vector2 boundaryRepelForce = Vector2.Zero;
            Vector2 entityRepelForce = Vector2.Zero;



            if (updateToken % 10 == 0)
            {
                BinQueryResults binItems = AiEntityStateHelpers.GetBinItems(velocityAlignmentRadius.Radius, AiEntity.GetPosition(), AiEntity.Zone.Bin, BinLayers.kEnemyEntities);

                Vector2 averageVelocity = Vector2.Zero;
                Vector2 averagePosition = Vector2.Zero;
                int averageVelocityCount = 0;
                int averagePositionCount = 0;

                for (int i = 0; i < binItems.BinItemCount; ++i)
                {
                    AiEntity entity = (AiEntity)binItems.BinItemQueryResults[i];

                    if (entity != AiEntity)
                    {
                        Vector2 dPosition = entity.GetPosition() - AiEntity.GetPosition();
                        float dPositionLenSqrd = dPosition.LengthSquared();

                        if (dPositionLenSqrd < velocityAlignmentRadius.RadiusSq)
                        {
                            float facingAlignment = Vector2.Dot(AiEntity.FacingDirection, entity.FacingDirection);

                            if (facingAlignment > 0.4f)
                            {
                                //float dPositionLen = (float)System.Math.Sqrt(dPositionLenSqrd);
                                //float w = 1.0f - (dPositionLen / velocityAlignmentRadius.Radius);

                                //averageVelocity += (entity.FacingDirection * w);
                                averageVelocity += entity.FacingDirection;
                                ++averageVelocityCount;
                            }

                            averagePosition += entity.GetPosition();
                            ++averagePositionCount;
                        }
                    }
                }


                if (averagePositionCount > 0)
                {
                    averagePosition /= averagePositionCount;
                    forceAveragePosition = averagePosition - AiEntity.GetPosition();
                    forceAveragePosition.Normalize();
                }

                if (averageVelocityCount > 0)
                {
                    averageVelocity /= averageVelocityCount;
                    forceAverageVelocity = averageVelocity;
                    forceAverageVelocity.Normalize();
                }

                entityRepelForce = -AiEntityStateHelpers.GetForceFromSurroundingEntities(kEntityRepelRadius, kEntityRepelRadius * kEntityRepelRadius, AiEntity, BinLayers.kEnemyEntities);
                entityRepelForce *= kEntityRepelStr;
            }



            if ((updateToken + 1) % 5 == 0)
            {
                boundaryRepelForce = AiEntityStateHelpers.GetForceFromSurroundingBoundaries(kBoundaryRepelRadius, kBoundaryRepelRadius * kBoundaryRepelRadius, AiEntity, BinLayers.kBoundary);
                boundaryRepelForce *= kBoundaryRepelStr;
            }

            // ---- Search for nearest entity to chase ----
            if ((updateToken + 2) % 5 == 0)
            {
                GameEntity closestEntity = null;
                Vector2 closetDPosition = Vector2.Zero;
                if (AiEntityStateHelpers.GetEntities(kEntitySightRadius, kEntityHearRadius, 0.5f, AiEntity, BinLayers.kFriendyList, BinLayers.kBoundaryOcclusionSmall, ref closestEntity, ref closetDPosition))
                {
                    AiEntity.StateMachine.CurrentState = ChaseState;
                }
            }



            Vector2 walkDirection = Vector2.Zero;
            walkDirection += forceAverageVelocity * velocityAlignmentStr;
            walkDirection += forceAveragePosition * averagePositionStr;
            walkDirection += entityRepelForce;
            walkDirection += boundaryRepelForce;
            walkDirection += AiEntity.FacingDirection * 0.5f;

            float walkDirectionLenSqrd = walkDirection.LengthSquared();
            if (walkDirectionLenSqrd > 0.0f)
            {
                float walkDirectionLen = (float)System.Math.Sqrt(walkDirectionLenSqrd);
                Vector2 walkDirectionNorm = walkDirection / walkDirectionLen;
                AiEntity.TurnTowardsAndWalk(walkDirectionNorm, 0.05f, AiEntity.Speed * frameTime.Dt);
            }
        }
    }
}
