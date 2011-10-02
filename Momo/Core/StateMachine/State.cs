using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.StateMachine
{
    public abstract class State
    {
        private StateMachine m_machine = null;

        public State(StateMachine machine)
        {
            m_machine = machine;
        }

        public override string ToString()
        {
            return "";
        }

        public StateMachine GetMachine() { return m_machine; }

        public virtual void OnEnter() { }
        public abstract void Update(ref FrameTime frameTime);
        public virtual void OnExit() { }
    }
}
