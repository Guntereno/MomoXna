using System;
using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Core.StateMachine;
using TestGame.Entities;

namespace TestGame.Ai.AiEntityStates
{
    public class WanderState : TimedState
    {
        float m_wanderTurnVelocity = 0.0f;


        public State IdleState { get; set; }


        public WanderState(AiEntity entity) :
            base(entity)
        {
        }


        public override string ToString()
        {
            return "Wander";
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            GameWorld world = AiEntity.World;
            Random random = world.Random;


            float entityRepelRadius = 100.0f;
            float boundaryRepelRadius = 25.0f;
            float entityRepelStr = 2.5f;
            float boundaryRepelStr = 1.5f;

            Vector2 entityRepelForce = -AiEntityStateHelpers.GetForceFromSurroundingEntities(entityRepelRadius, entityRepelRadius * entityRepelRadius, AiEntity, BinLayers.kEnemyEntities);
            Vector2 boundaryRepelForce = AiEntityStateHelpers.GetForceFromSurroundingBoundaries(boundaryRepelRadius, boundaryRepelRadius * boundaryRepelRadius, AiEntity, BinLayers.kBoundary);


            m_wanderTurnVelocity += ((float)random.NextDouble() - 0.5f) * 50.0f * frameTime.Dt;
            m_wanderTurnVelocity = MathHelper.Clamp(m_wanderTurnVelocity, -1.0f, 1.0f);

            float facingAngle = AiEntity.FacingAngle;
            facingAngle += m_wanderTurnVelocity * frameTime.Dt;
            AiEntity.FacingAngle = facingAngle;


            Vector2 walkDirection = entityRepelForce * entityRepelStr;
            walkDirection += boundaryRepelForce * boundaryRepelStr;
            walkDirection += AiEntity.FacingDirection;

            float walkDirectionLenSqrd = walkDirection.LengthSquared();
            if (walkDirectionLenSqrd > 0.0f)
            {
                float walkDirectionLen = (float)System.Math.Sqrt(walkDirectionLenSqrd);
                Vector2 walkDirectionNorm = walkDirection / walkDirectionLen;
                AiEntity.TurnTowardsAndWalk(walkDirectionNorm, 0.1f, 1.0f);
            }
        }
    }
}
