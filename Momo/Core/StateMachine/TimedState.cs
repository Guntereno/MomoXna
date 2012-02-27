using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.StateMachine
{
    public class TimedState : State
    {
        private string m_exitState = null;
        private float m_timer = 0.0f;
        private float m_length = 0.0f;

        public TimedState(StateMachine machine) :
            base(machine)
        {
        }

        public float GetTimer() { return m_timer; }

        public void SetLength(float length)
        {
            m_length = length;
        }

        public void SetExitState(string exitState)
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
                GetMachine().SetCurrentState(m_exitState);
            }
        }
    }
}
