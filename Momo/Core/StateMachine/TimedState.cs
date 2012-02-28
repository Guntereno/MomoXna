using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.StateMachine
{
    public class TimedState : State
    {
        private State m_exitState = null;
        private float m_timer = 0.0f;
        private float m_length = 0.0f;

        public TimedState(IStateMachineOwner owner) :
            base(owner)
        {
        }

        public float GetTimer() { return m_timer; }

        public void SetLength(float length)
        {
            m_length = length;
        }

        public void SetExitState(State exitState)
        {
            m_exitState = exitState;
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
    }
}
