using System;
using Microsoft.Xna.Framework;
using Momo.Core;
using Momo.Core.StateMachine;
using TestGame.Entities;

namespace TestGame.Ai.AiEntityStates
{
    public class IdleState : TimedState
    {
        float mChangeTimer = 0.0f;
        float mLookSpeed = 1.0f;
 

        public IdleState(AiEntity entity) :
            base(entity)
        {
        }


        public override string ToString()
        {
            return "Idle";
        }


        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            base.Update(ref frameTime, updateToken);

            GameWorld world = AiEntity.World;
            Random random = world.Random;



            if (mLookSpeed != 0.0f)
            {
                AiEntity.FacingAngle += mLookSpeed * frameTime.Dt;
            }


            if (mChangeTimer <= 0.0f)
            {
                int choice = random.Next(1000);

                if (choice < 50)
                {
                    mLookSpeed = 5.0f;
                    mChangeTimer = 0.20f;
                }
                else if (choice < 100)
                {
                    mLookSpeed = -5.0f;
                    mChangeTimer = 0.20f;
                }
                else
                {
                    mLookSpeed = 0.0f;
                    mChangeTimer = 5.0f;
                }
            }

            mChangeTimer -= frameTime.Dt;
        }
    }
}
