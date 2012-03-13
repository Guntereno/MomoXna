using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;

using TestGame.Entities;



namespace TestGame.Ai.AiEntityStates
{
    public class ZombieChaseState : TimedState
    {
        Vector2 mChasePosition;
        float mSearchTime;

        const float kStateBaseSpeedMod = 6.0f;

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

            Vector2 boundaryRepelForce = Vector2.Zero;
            Vector2 entityRepelForce = Vector2.Zero;
            Vector2 chaseEntityForce = Vector2.Zero;

            float kEntityRepelRadius = AiEntity.ContactRadiusInfo.Radius * 3.0f;
            float kEntityRepelStr = 5.0f;
            float kBoundaryRepelRadius = AiEntity.ContactRadiusInfo.Radius * 2.0f;
            float kBoundaryRepelStr = 3.5f;
            float kEntitySightRadius = 800.0f;
            float kEntityHearRadius = 250.0f;
            float kEntityViewDot = 0.5f;
            float kChaseStr = 0.25f;


            // Boundary repel
            if (updateToken % 20 == 0)
            {
                boundaryRepelForce = AiEntityStateHelpers.GetForceFromSurroundingBoundaries(kBoundaryRepelRadius, kBoundaryRepelRadius * kBoundaryRepelRadius, AiEntity, BinLayers.kBoundary);
                boundaryRepelForce *= kBoundaryRepelStr;
            }
            // Entity repel
            if ((updateToken + 1) % 3 == 0)
            {
                entityRepelForce = -AiEntityStateHelpers.GetForceFromSurroundingEntities(kEntityRepelRadius, kEntityRepelRadius * kEntityRepelRadius, AiEntity, BinLayers.kEnemyEntities);
                entityRepelForce *= kEntityRepelStr;
            }

            // ---- Search for nearest entity to chase ----
            if ((updateToken + 2) % 5 == 0)
            {
                GameEntity closestEntity = null;
                Vector2 closetDPosition = Vector2.Zero;
                if (AiEntityStateHelpers.GetEntities(kEntitySightRadius, kEntityHearRadius, kEntityViewDot, AiEntity, BinLayers.kFriendyList, BinLayers.kBoundaryOcclusionSmall, ref closestEntity, ref closetDPosition))
                {
                    mChasePosition = closestEntity.GetPosition();
                    AiEntity.Speed = AiEntity.BaseSpeed * kStateBaseSpeedMod;
                    mSearchTime = 0.0f;
                }

                Vector2 dChasePosition = mChasePosition - AiEntity.GetPosition();
                float chaseDPositionLenSqrd = dChasePosition.LengthSquared();
                float chasePositionLen = (float)Math.Sqrt(chaseDPositionLenSqrd);
                chaseEntityForce = dChasePosition / chasePositionLen;
                chaseEntityForce *= kChaseStr;
            }



            // -------------------------------------------

            Vector2 walkDirection = entityRepelForce;
            walkDirection += boundaryRepelForce;
            walkDirection += chaseEntityForce;
            walkDirection += AiEntity.FacingDirection;

            float walkDirectionLenSqrd = walkDirection.LengthSquared();
            if (walkDirectionLenSqrd > 0.0f)
            {
                float walkDirectionLen = (float)System.Math.Sqrt(walkDirectionLenSqrd);
                Vector2 walkDirectionNorm = walkDirection / walkDirectionLen;
                AiEntity.TurnTowardsAndWalk(walkDirectionNorm, 0.10f, AiEntity.Speed * frameTime.Dt);
            }


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
