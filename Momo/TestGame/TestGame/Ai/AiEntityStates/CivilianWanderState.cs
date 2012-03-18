﻿using System;
using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.StateMachine;
using Momo.Core.Spatial;

using Momo.Maths;

using Game.Entities;



namespace Game.Ai.AiEntityStates
{
    public class CivilianWanderState : TimedState
    {
        const float kStateBaseSpeedMod = 1.0f;


        public State FleeState { get; set; }


        public CivilianWanderState(AiEntity entity) :
            base(entity)
        {
            DebugColor = new Color(0.0f, 0.6f, 0.6f, 0.7f);
        }


        public override string ToString()
        {
            return "Wander";
        }


        public override void OnEnter()
        {
            base.OnEnter();

            AiEntity.Speed = AiEntity.BaseSpeed * kStateBaseSpeedMod;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            Zone zone = AiEntity.Zone;
            Random random = zone.Random;


            float kEntitySightRadius = 800.0f;
            float kEntityHearRadius = 250.0f;
            float kEntityViewDot = 0.5f;


            // Flee enemies
            if (updateToken % 30 == 0)
            {
                GameEntity closestEntity = null;
                Vector2 closetDPosition = Vector2.Zero;
                if (AiEntityStateHelpers.GetEntities(kEntitySightRadius, kEntityHearRadius, kEntityViewDot, AiEntity, BinLayers.kEnemyList, BinLayers.kBoundaryOcclusionSmall, ref closestEntity, ref closetDPosition))
                {
                    AiEntity.StateMachine.CurrentState = FleeState;
                }
            }
        }
    }
}
