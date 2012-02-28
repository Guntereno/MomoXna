using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.StateMachine
{
    public class StateMachine
    {
        private State m_currentState = null;
        private IStateMachineOwner m_owner = null;


        public StateMachine(IStateMachineOwner owner)
        {
            m_owner = owner;
        }


        public Object Owner         { get { return m_owner; } }
        public State CurrentState   {
            get { return m_currentState; }
            set
            {
                if (m_currentState != null)
                {
                    m_currentState.OnExit();
                }

                m_currentState = value;

                if (m_currentState != null)
                {
                    m_currentState.OnEnter();
                }
            }
        }

        public string CurrentStateName
        {
            get
            {
                if (m_currentState == null)
                {
                    return "";
                }

                return m_currentState.ToString();
            }
        }


        public void Update(ref FrameTime frameTime)
        {
            if (m_currentState != null)
            {
                m_currentState.Update(ref frameTime);
            }
        }
    }
}
