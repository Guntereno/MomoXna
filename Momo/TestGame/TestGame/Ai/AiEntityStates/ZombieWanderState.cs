using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;

using Game.Entities;



namespace Game.Ai.AiEntityStates
{
    public class ZombieWanderState : TimedState
    {
        float mWanderTurnVelocity = 0.0f;


        public State HerdState { get; set; }
        public State ChaseState { get; set; }


        public ZombieWanderState(AiEntity entity) :
            base(entity)
        {
            DebugColor = new Color(0.3f, 0.0f, 0.0f, 0.7f);
        }


        public override string ToString()
        {
            return "Wander";
        }


        public override void OnEnter()
        {
            base.OnEnter();

            mWanderTurnVelocity = 0.0f;

            AiEntity.Speed = AiEntity.BaseSpeed;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            Zone zone = AiEntity.Zone;
            Random random = zone.Random;



            float kEntityRepelRadius = AiEntity.ContactRadiusInfo.Radius * 3.5f;
            float kEntityRepelStr = 2.0f;
            float kBoundaryRepelRadius = AiEntity.ContactRadiusInfo.Radius * 2.5f;
            float kBoundaryRepelStr = 4.0f;


            Vector2 boundaryRepelForce = ZombieStateHelper.CalculateBoundaryRepelForce(AiEntity, kBoundaryRepelStr, kBoundaryRepelRadius, updateToken, 0, 5);
            Vector2 entityRepelForce = ZombieStateHelper.CalculateEnemyRepelForce(AiEntity, kEntityRepelStr, kEntityRepelRadius, updateToken, 1, 7);



            // Wander
            if ((updateToken + 3) % 2 == 0)
            {
                float facingAngle = AiEntity.FacingAngle;
                mWanderTurnVelocity += ((float)random.NextDouble() - 0.5f) * 4.0f * frameTime.Dt;
                mWanderTurnVelocity = MathHelper.Clamp(mWanderTurnVelocity, -0.18f, 0.18f);
                facingAngle += mWanderTurnVelocity * frameTime.Dt;
                AiEntity.FacingAngle = facingAngle;
            }

            // ---- Search for nearest entity to chase ----
            if (ZombieStateHelper.CheckForFriendly(AiEntity, updateToken) != null)
            {
                AiEntity.StateMachine.CurrentState = ChaseState;
            }


            Vector2 walkDirection = entityRepelForce;
            walkDirection += boundaryRepelForce;
            walkDirection += AiEntity.FacingDirection;

            ZombieStateHelper.TurnTowardsAndWalk(AiEntity, walkDirection, 0.05f, frameTime.Dt);
        }
    }
}
