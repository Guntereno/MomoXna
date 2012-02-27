using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.StateMachine
{
    public class StateMachine
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private int m_currentStateIdx = -1;
        private object m_owner = null;

        private List<KeyValuePair<string, State>> m_states = new List<KeyValuePair<string, State>>();

        public StateMachine(Object owner)
        {
            m_owner = owner;
        }

        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        public object Owner { get { return m_owner; } }

        public State CurrentState {
            get
            {
                if(m_currentStateIdx < 0)
                    return null;
                else
                    return m_states[m_currentStateIdx].Value;
            }
        }

        public string CurrentStateName
        {
            get
            {
                if (m_currentStateIdx < 0)
                    return null;
                else
                    return m_states[m_currentStateIdx].Key;
            }
        }

        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void AddState(string name, State state)
        {
            if (state == null)
                throw new System.ArgumentException();

            m_states.Add(new KeyValuePair<string, State>(name, state));
        }

        public void Update(ref FrameTime frameTime)
        {
            if (m_currentStateIdx >= 0)
            {
                m_states[m_currentStateIdx].Value.Update(ref frameTime);
            }
        }

        public void Stop()
        {
            if (m_currentStateIdx >= 0)
            {
                m_states[m_currentStateIdx].Value.OnExit();
            }

            m_currentStateIdx = -1;
        }

        public void SetCurrentState(string name)
        {
            if (m_currentStateIdx >= 0)
            {
                m_states[m_currentStateIdx].Value.OnExit();
            }

            m_currentStateIdx = FindIndexForName(name);

            m_states[m_currentStateIdx].Value.OnEnter();
        }


        // --------------------------------------------------------------------
        // -- Private Helpers
        // --------------------------------------------------------------------
        private int FindIndexForName(string name)
        {
            for (int i = 0; i < m_states.Count; ++i)
            {
                KeyValuePair<string, State> statePair = m_states[i];
                if (statePair.Key == name)
                {
                    return i;
                }
            }

            throw new System.ArgumentException();
        }
    }
}
