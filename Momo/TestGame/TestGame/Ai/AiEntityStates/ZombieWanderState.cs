﻿using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;

using TestGame.Entities;



namespace TestGame.Ai.AiEntityStates
{
    public class ZombieWanderState : TimedState
    {
        float mWanderTurnVelocity = 0.0f;


        public State HerdState { get; set; }


        public ZombieWanderState(AiEntity entity) :
            base(entity)
        {
        }


        public override string ToString()
        {
            return "Wander";
        }


        public override void OnEnter()
        {
            base.OnEnter();
            mWanderTurnVelocity = 0.0f;

            AiEntity.SecondaryDebugColor = Color.Aquamarine;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            GameWorld world = AiEntity.World;
            Random random = world.Random;





            const float kNeighbourCheckRadius = 750.0f;
            const float kNeighbourDesireRadius = 200.0f;
            const float kNeighbourDesireRadiusSqrd = kNeighbourDesireRadius * kNeighbourDesireRadius;


            Vector2 boundaryRepelForce = Vector2.Zero;
            Vector2 entityRepelForce = Vector2.Zero;


            float kEntityRepelRadius = AiEntity.ContactRadiusInfo.Radius * 3.0f;
            float kEntityRepelStr = 1.5f;
            float kBoundaryRepelRadius = AiEntity.ContactRadiusInfo.Radius * 2.0f;
            float kBoundaryRepelStr = 1.5f;
            // Boundary repel
            if (updateToken % 20 == 0)
            {
                boundaryRepelForce = AiEntityStateHelpers.GetForceFromSurroundingBoundaries(kBoundaryRepelRadius, kBoundaryRepelRadius * kBoundaryRepelRadius, AiEntity, BinLayers.kBoundary);
                boundaryRepelForce *= kBoundaryRepelStr;
            }
            // Entity repel
            if ((updateToken + 1) % 5 == 0)
            {
                entityRepelForce = -AiEntityStateHelpers.GetForceFromSurroundingEntities(kEntityRepelRadius, kEntityRepelRadius * kEntityRepelRadius, AiEntity, BinLayers.kEnemyEntities);
                entityRepelForce *= kEntityRepelStr;
            }

            if ((updateToken + 2) % 120 == 0)
            {
                Vector2 averageNeighbourPosition = AiEntityStateHelpers.GetAveragePositionFromSurroundingEntities(kNeighbourCheckRadius, kNeighbourCheckRadius * kNeighbourCheckRadius, AiEntity, BinLayers.kEnemyEntities);
                if ((averageNeighbourPosition - AiEntity.GetPosition()).LengthSquared() > kNeighbourDesireRadiusSqrd)
                {
                    AiEntity.StateMachine.CurrentState = HerdState;
                }
            }

            if ((updateToken + 3) % 2 == 0)
            {
                float facingAngle = AiEntity.FacingAngle;
                mWanderTurnVelocity += ((float)random.NextDouble() - 0.5f) * 4.0f * frameTime.Dt;
                mWanderTurnVelocity = MathHelper.Clamp(mWanderTurnVelocity, -0.18f, 0.18f);
                facingAngle += mWanderTurnVelocity * frameTime.Dt;
                AiEntity.FacingAngle = facingAngle;
            }


            Vector2 walkDirection = entityRepelForce;
            walkDirection += boundaryRepelForce;
            walkDirection += AiEntity.FacingDirection;

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
