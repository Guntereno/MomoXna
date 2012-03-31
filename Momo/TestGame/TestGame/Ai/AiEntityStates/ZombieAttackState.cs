using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;

using Game.Entities;



namespace Game.Ai.AiEntityStates
{
    public class ZombieAttackState : TimedState
    {
        public ZombieChaseState ChaseState { get; set; }

        public Vector2 ChasePosition { get; set; }


        Vector2 mLastPosition = Vector2.Zero;
        float mSpeedMod = 0.0f;
        float mSpeedModAverage = 1.0f;


        const float kStateBaseSpeedMod = 10.0f;


        public ZombieAttackState(AiEntity entity) :
            base(entity)
        {
            DebugColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);
        }


        public override string ToString()
        {
            return "Attack";
        }


        public override void OnEnter()
        {
            base.OnEnter();

            ChasePosition = Vector2.Zero;
            mSpeedMod = 1.0f;
            mSpeedModAverage = 1.0f;


            AiEntity.Speed = AiEntity.BaseSpeed * kStateBaseSpeedMod;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            Zone zone = AiEntity.Zone;
            Random random = zone.Random;


            Vector2 chaseEntityForce = Vector2.Zero;

            float kEntitySightRadius = 800.0f;
            float kEntityHearRadius = 300.0f;
            float kEntityViewDot = -0.15f;






            Vector2 dPosition = AiEntity.GetPosition() - mLastPosition;
            float dPositionLen = dPosition.Length();
            mLastPosition = AiEntity.GetPosition();


            if (dPositionLen <= (AiEntity.Gait.LastMoveIntent * 0.95f))
            {
                mSpeedMod -= 5.0f * frameTime.Dt;
                if (mSpeedMod < 0.005f)
                    mSpeedMod = 0.005f;
            }
            else
            {
                mSpeedMod += 5.0f * frameTime.Dt;
                if (mSpeedMod > 1.0f)
                    mSpeedMod = 1.0f;
            }


            float averageMod = 0.95f;
            float invAverageMod = 1.0f - averageMod;
            mSpeedModAverage = (mSpeedModAverage * averageMod) + (mSpeedMod * invAverageMod);



            // ---- Search for nearest entity to chase ----
            if ((updateToken + 2) % 5 == 0)
            {
                GameEntity closestEntity = null;
                Vector2 closetDPosition = Vector2.Zero;
                if (AiEntityStateHelpers.GetClosestEntityInRange(kEntitySightRadius, kEntityHearRadius, kEntityViewDot, AiEntity, BinLayers.kFriendyList, BinLayers.kBoundaryOcclusionSmall, ref closestEntity, ref closetDPosition))
                {
                    ChasePosition = closestEntity.GetPosition();
                    AiEntity.Speed = AiEntity.BaseSpeed * kStateBaseSpeedMod;
                }
            }




            if (ChasePosition != Vector2.Zero)
            {
                Vector2 dChasePosition = ChasePosition - AiEntity.GetPosition();
                float dChasePositionLen = dChasePosition.Length();


                if (dChasePositionLen > 20.0f)
                {
                    ZombieStateHelper.TurnTowardsAndWalk(AiEntity, dChasePosition, 0.10f, frameTime.Dt);
                }
                else
                {
                    AiEntity.TurnTowards(dChasePosition, 0.10f);
                }

                if (dChasePositionLen > 80.0f)
                {
                    AiEntity.StateMachine.CurrentState = ChaseState;
                    ChaseState.ChasePosition = ChasePosition;
                }
            }
        }
    }
}
