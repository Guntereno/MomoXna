using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.StateMachine
{
    public class StateMachine
    {
        State m_currentState = null;
        Object m_owner = null;


        public StateMachine(Object owner)
        {
            m_owner = owner;
        }


        public Object GetOwner() { return m_owner; }


        public void Update(ref FrameTime frameTime)
        {
            if (m_currentState != null)
            {
                m_currentState.Update(ref frameTime);
            }
        }

        public void SetCurrentState(State state)
        {
            if (m_currentState != null)
            {
                m_currentState.OnExit();
            }

            m_currentState = state;

            if (m_currentState != null)
            {
                m_currentState.OnEnter();
            }
        }

        public State GetCurrentState()
        {
            return m_currentState;
        }

        public String GetCurrentStateName()
        {
            if (m_currentState == null)
            {
                return "";
            }

            return m_currentState.ToString();
        }


    }
}
