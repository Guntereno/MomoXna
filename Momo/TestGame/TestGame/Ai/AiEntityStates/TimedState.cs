using Momo.Core;
using Momo.Core.StateMachine;
using TestGame.Entities;
using Microsoft.Xna.Framework;

namespace TestGame.Ai.AiEntityStates
{
    public class TimedState : AiEntityState
    {
        protected float mTimer;

        protected float mTimeInState;
        protected float mMinTimeInState;
        protected float mRandomTimeInState;



        public State NextState { get; set; }

        public float TimeInState
        {
            set
            {
                mMinTimeInState = value;
                mRandomTimeInState = 0.0f;
            }
        }
        public float MinimumTimeInState
        {
            get { return mMinTimeInState; }
            set { mMinTimeInState = value; }
        }
        public float MaximumTimeInState
        {
            get { return mMinTimeInState + mRandomTimeInState; }
            set { mRandomTimeInState = value - mMinTimeInState; }
        }


        public TimedState(AiEntity entity) :
            base(entity)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            mTimeInState = mMinTimeInState + (float)AiEntity.World.Random.NextDouble() * mRandomTimeInState;
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            mTimer += frameTime.Dt;
            if (mTimer >= mTimeInState)
            {
                AiEntity.StateMachine.CurrentState = NextState;
            }
        }
    }
}
