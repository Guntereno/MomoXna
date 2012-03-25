using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;

using Game.Entities;



namespace Game.Ai.AiEntityStates
{
    public class ZombieChaseState : TimedState
    {
        Vector2 mChasePosition;
        float mSearchTime;

        const float kStateBaseSpeedMod = 17.0f;

        const float kSearchStopTime = 3.0f;
        const float kSearchCoolDownTime = 2.0f;

        const float kSearchStopCoolDownDiff = kSearchStopTime - kSearchCoolDownTime;



        public ZombieChaseState(AiEntity entity) :
            base(entity)
        {
            DebugColor = new Color(1.0f, 0.0f, 0.0f, 0.7f);
        }


        public override string ToString()
        {
            return "Chase";
        }


        public override void OnEnter()
        {
            base.OnEnter();

            mChasePosition = Vector2.Zero;
            mSearchTime = 0.0f;

            AiEntity.Speed = AiEntity.BaseSpeed * kStateBaseSpeedMod;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            Zone zone = AiEntity.Zone;
            Random random = zone.Random;


            mSearchTime += frameTime.Dt;

            Vector2 chaseEntityForce = Vector2.Zero;

            float kEntityRepelRadius = AiEntity.ContactRadiusInfo.Radius * 3.0f;
            float kEntityRepelStr = 5.0f;
            float kBoundaryRepelRadius = AiEntity.ContactRadiusInfo.Radius * 2.0f;
            float kBoundaryRepelStr = 3.5f;
            float kEntitySightRadius = 800.0f;
            float kEntityHearRadius = 300.0f;
            float kEntityViewDot = -0.15f;
            float kChaseStr = 0.15f;


            // Boundary repel
            Vector2 boundaryRepelForce = ZombieStateHelper.CalculateBoundaryRepelForce(AiEntity, kBoundaryRepelStr, kBoundaryRepelRadius, updateToken, 0, 20);
            Vector2 entityRepelForce = ZombieStateHelper.CalculateEnemyRepelForce(AiEntity, kEntityRepelStr, kEntityRepelRadius, updateToken, 1, 3);


            // ---- Search for nearest entity to chase ----
            if ((updateToken + 2) % 5 == 0)
            {
                GameEntity closestEntity = null;
                Vector2 closetDPosition = Vector2.Zero;
                if (AiEntityStateHelpers.GetClosestEntityInRange(kEntitySightRadius, kEntityHearRadius, kEntityViewDot, AiEntity, BinLayers.kFriendyList, BinLayers.kBoundaryOcclusionSmall, ref closestEntity, ref closetDPosition))
                {
                    mChasePosition = closestEntity.GetPosition();
                    AiEntity.Speed = AiEntity.BaseSpeed * kStateBaseSpeedMod;
                    mSearchTime = 0.0f;
                }
            }

            if (mChasePosition != Vector2.Zero)
            {
                Vector2 dChasePosition = mChasePosition - AiEntity.GetPosition();
                chaseEntityForce = dChasePosition;
                chaseEntityForce.Normalize();
                chaseEntityForce *= kChaseStr;
            }



            // -------------------------------------------
            Vector2 walkDirection = entityRepelForce;
            walkDirection += boundaryRepelForce;
            walkDirection += chaseEntityForce;
            walkDirection += AiEntity.FacingDirection * 0.1f;

            ZombieStateHelper.TurnTowardsAndWalk(AiEntity, walkDirection, 0.10f, frameTime.Dt);


            // Abandon search after a period of time.
            if (mSearchTime > kSearchCoolDownTime)
            {
                // TODO: This has been done a bit tispy. There must be a more efficient way??!
                float speedMod = ((kSearchStopTime - mSearchTime) / kSearchStopCoolDownDiff);
                AiEntity.Speed = AiEntity.BaseSpeed + ((AiEntity.BaseSpeed * kStateBaseSpeedMod * speedMod) - AiEntity.BaseSpeed);

                if (mSearchTime > kSearchStopTime)
                {
                    AiEntity.StateMachine.CurrentState = NextState;
                }
            }
        }
    }
}
