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
        public State WanderState { get; set; }
        public ZombieChaseState ChaseState { get; set; }

        private const int kHerdListCapacity = 5;
        private GameEntity[] mHerdList = new GameEntity[kHerdListCapacity];
        private int mHerdListCount = 0;


        const float kStateBaseSpeedMod = 5.0f;


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

            AiEntity.Speed = AiEntity.BaseSpeed * kStateBaseSpeedMod;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            Zone world = AiEntity.Zone;
            Random random = world.Random;


            float velocityAlignmentStr = 0.1f;
            float averagePositionStr = 0.5f;

            float kEntityRepelRadius = AiEntity.ContactRadiusInfo.Radius * 2.5f;
            float kEntityRepelStr = 2.0f;
            float kBoundaryRepelRadius = AiEntity.ContactRadiusInfo.Radius * 2.5f;
            float kBoundaryRepelStr = 4.0f;


            Vector2 forceAveragePosition = Vector2.Zero;
            Vector2 forceAverageVelocity = Vector2.Zero;


            ZombieStateHelper.UpdateViewList(AiEntity, 200.0f, 0.65f, ref mHerdList, ref mHerdListCount, kHerdListCapacity, updateToken, 0, 20);


            if (updateToken % 10 == 0)
            {
                Vector2 averageVelocity = Vector2.Zero;
                Vector2 averagePosition = Vector2.Zero;
                int averageVelocityCount = 0;
                int averagePositionCount = 0;

                for (int i = 0; i < mHerdListCount; ++i)
                {
                    GameEntity entity = mHerdList[i];

                    //float dPositionLen = (float)System.Math.Sqrt(dPositionLenSqrd);
                    //float w = 1.0f - (dPositionLen / velocityAlignmentRadius.Radius);

                    //averageVelocity += (entity.FacingDirection * w);
                    averageVelocity += entity.FacingDirection;
                    ++averageVelocityCount;

                    averagePosition += entity.GetPosition();
                    ++averagePositionCount;
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

                if (mHerdListCount == 0)
                {
                    AiEntity.StateMachine.CurrentState = WanderState;
                }
            }



            Vector2 boundaryRepelForce = ZombieStateHelper.CalculateBoundaryRepelForce(AiEntity, kBoundaryRepelStr, kBoundaryRepelRadius, updateToken, 1, 5);
            Vector2 entityRepelForce = ZombieStateHelper.CalculateEnemyRepelForce(AiEntity, kEntityRepelStr, kEntityRepelRadius, updateToken, 2, 7);


            // ---- Search for nearest entity to chase ----
            GameEntity closestFriendly = ZombieStateHelper.CheckForFriendly(AiEntity, updateToken);
            if (closestFriendly != null)
            {
                AiEntity.StateMachine.CurrentState = ChaseState;
                ChaseState.ChasePosition = closestFriendly.GetPosition();
            }



            Vector2 walkDirection = Vector2.Zero;
            walkDirection += forceAverageVelocity * velocityAlignmentStr;
            walkDirection += forceAveragePosition * averagePositionStr;
            walkDirection += entityRepelForce;
            walkDirection += boundaryRepelForce;
            walkDirection += AiEntity.FacingDirection * 0.25f;

            ZombieStateHelper.TurnTowardsAndWalk(AiEntity, walkDirection, 0.05f, frameTime.Dt);
        }
    }
}
