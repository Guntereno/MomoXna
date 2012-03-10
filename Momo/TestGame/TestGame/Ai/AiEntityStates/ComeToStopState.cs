using System;
using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Core.StateMachine;
using TestGame.Entities;

namespace TestGame.Ai.AiEntityStates
{
    public class ComeToStopState : TimedState
    {
        public ComeToStopState(AiEntity entity) :
            base(entity)
        {
        }


        public override string ToString()
        {
            return "Come to stop";
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

            GameWorld world = AiEntity.World;
            Random random = world.Random;


            float newSpeed = AiEntity.Speed - (0.1f * frameTime.Dt);

            if (newSpeed <= 0.0f)
            {
                AiEntity.Speed = 0.0f;
                AiEntity.StateMachine.CurrentState = NextState;
            }
            else
            {
                AiEntity.Speed = newSpeed;
            }
        }
    }
}
