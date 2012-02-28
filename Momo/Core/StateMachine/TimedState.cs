using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.StateMachine
{
    public class TimedState : State
    {
        #region Fields

        private State m_exitState = null;
        private float m_timer = 0.0f;
        private float m_length = 0.0f;

        #endregion

        #region Constructor

        public TimedState(IStateMachineOwner owner) :
            base(owner)
        {
        }

        #endregion


        #region Public Interface

        public float Timer
        {
            get { return m_timer; }
            set { m_timer = value; }
        }

        public float Length
        {
            get { return m_length; }
            set { m_length = value; }
        }

        public State ExitState
        {
            get { return m_exitState; }
            set { m_exitState = value; }
        }

        public override void OnEnter()
        {
            m_timer = m_length;
        }

        public override void Update(ref FrameTime frameTime)
        {
            m_timer -= frameTime.Dt;
            if (m_timer <= 0.0f)
            {
                Owner.StateMachine.CurrentState = m_exitState;
            }
        }

        #endregion
    }
}
