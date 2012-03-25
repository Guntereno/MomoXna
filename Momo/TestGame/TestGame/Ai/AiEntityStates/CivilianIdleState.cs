using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;
using Momo.Core.Spatial;

using Momo.Maths;

using Game.Entities;



namespace Game.Ai.AiEntityStates
{
    public class CivilianIdleState : TimedState
    {
        const float kStateBaseSpeedMod = 1.0f;


        public State FleeState { get; set; }


        public CivilianIdleState(AiEntity entity) :
            base(entity)
        {
            DebugColor = new Color(0.0f, 0.3f, 0.3f, 0.7f);
        }


        public override string ToString()
        {
            return "Idle";
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


            float kEntitySightRadius = 800.0f;
            float kEntityHearRadius = 250.0f;
            float kEntityViewDot = 0.5f;



            if (updateToken % 30 == 0)
            {
                GameEntity closestEntity = null;
                Vector2 closetDPosition = Vector2.Zero;
                if (AiEntityStateHelpers.GetClosestEntityInRange(kEntitySightRadius, kEntityHearRadius, kEntityViewDot, AiEntity, BinLayers.kEnemyList, BinLayers.kBoundaryOcclusionSmall, ref closestEntity, ref closetDPosition))
                {
                    AiEntity.StateMachine.CurrentState = FleeState;
                }
            }
        }
    }
}
