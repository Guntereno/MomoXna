using Momo.Core;
using Momo.Core.StateMachine;
using TestGame.Entities;
using Microsoft.Xna.Framework;

namespace TestGame.Ai.AiEntityStates
{
    public class TimedState : AiEntityState
    {
        #region Fields

        protected float m_timer;

        #endregion


        #region Constructor

        public TimedState(AiEntity entity) :
            base(entity)
        {
        }

        #endregion

        #region Public Interface

        public State NextState { get; set; }

        public float Length { get; set; }

        public override void OnEnter()
        {
            base.OnEnter();

            m_timer = 0.0f;
        }

        public override void Update(ref FrameTime frameTime, int updateToken)
        {
            m_timer += frameTime.Dt;
            if (m_timer >= Length)
            {
                AiEntity.StateMachine.CurrentState = NextState;
            }
        }

        #endregion
    }
}
