using Game.Ai.AiEntityStates;
using Game.Weapons;
using Momo.Core.StateMachine;
using Microsoft.Xna.Framework;



namespace Game.Entities.AI
{
    public class Civilian : AiEntity
    {
        #region State Machine
        private CivilianFleeState mStateFlee = null;
        private CivilianIdleState mStateIdle = null;
        #endregion


        public Civilian(Zone zone)
            : base(zone)
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


            BaseSpeed = 10.0f + ((float)Zone.Random.NextDouble() * 5.0f);

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
