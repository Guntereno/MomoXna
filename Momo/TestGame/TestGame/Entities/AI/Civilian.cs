using TestGame.Ai.AiEntityStates;
using TestGame.Weapons;
using Momo.Core.StateMachine;
using Microsoft.Xna.Framework;



namespace TestGame.Entities.AI
{
    public class Civilian : AiEntity
    {
        #region State Machine
        private CivilianFleeState mStateFlee = null;
        private CivilianIdleState mStateIdle = null;
        #endregion


        public Civilian(GameWorld world)
            : base(world)
        {
            mBinLayer = BinLayers.kCivilianEntities;

            mStateFlee = new CivilianFleeState(this);
            mStateFlee.MinimumTimeInState = 1000.0f;
            mStateFlee.MaximumTimeInState = 100000.0f;

            mStateIdle = new CivilianIdleState(this);
            mStateIdle.MinimumTimeInState = 1000.0f;
            mStateIdle.MaximumTimeInState = 100000.0f;


            mStateFlee.IdleState = mStateIdle;
            mStateIdle.FleeState = mStateFlee;


            BaseSpeed = 10.0f + ((float)World.Random.NextDouble() * 5.0f);

            PrimaryDebugColor = new Color(0.0f, 1.0f, 1.0f, 0.3f);
            SecondaryDebugColor = PrimaryDebugColor;
        }


        public override void ResetItem()
        {
            base.ResetItem();

            StateMachine.CurrentState = mStateFlee;
        }

    }
}
