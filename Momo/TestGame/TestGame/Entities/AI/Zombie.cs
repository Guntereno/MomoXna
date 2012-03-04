using TestGame.Ai.AiEntityStates;
using TestGame.Weapons;
using Momo.Core.StateMachine;
using Microsoft.Xna.Framework;



namespace TestGame.Entities.AI
{
    public class Zombie : AiEntity
    {
        #region State Machine
        private RandomWanderState m_stateRandomWander = null;
        #endregion

        public Zombie(GameWorld world)
            : base(world)
        {
            m_stateRandomWander = new RandomWanderState(this);
            mStateStunned.NextState = m_stateRandomWander;

            SecondaryDebugColor = new Color(1.0f, 0.0f, 0.0f);
        }


        public override void ResetItem()
        {
            base.ResetItem();

            StateMachine.CurrentState = m_stateRandomWander;
        }

    }
}
