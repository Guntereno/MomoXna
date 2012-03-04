using TestGame.Ai.AiEntityStates;
using TestGame.Weapons;
using Momo.Core.StateMachine;
using Microsoft.Xna.Framework;



namespace TestGame.Entities.AI
{
    public class Civilian : AiEntity
    {
        #region State Machine
        private IdleState mStateIdle = null;
        private WanderState mStateWander = null;
        #endregion


        public Civilian(GameWorld world)
            : base(world)
        {
            mBinLayer = BinLayers.kCivilianEntities;

            mStateIdle = new IdleState(this);
            mStateWander = new WanderState(this);

            mStateIdle.MinimumTimeInState = 1.0f;
            mStateIdle.MaximumTimeInState = 10.0f;
            mStateIdle.NextState = mStateWander;


            //m_stateWander.MinimumTimeInState = 1.0f;
            //m_stateWander.MaximumTimeInState = 10.0f;
            mStateWander.TimeInState = 10000.0f;
            mStateWander.NextState = mStateIdle;
            //m_stateWander.IdleState = m_stateIdle;

            BaseSpeed = 1.0f + ((float)World.Random.NextDouble() * 0.35f);

            SecondaryDebugColor = new Color(0.0f, 0.0f, 1.0f);
        }


        public override void ResetItem()
        {
            base.ResetItem();


            StateMachine.CurrentState = mStateWander;
        }

    }
}
