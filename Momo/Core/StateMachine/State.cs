using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.StateMachine
{
    public abstract class State
    {
        private IStateMachineOwner m_owner = null;

        public IStateMachineOwner Owner
        {
            get
            {
                return m_owner;
            }
        }

        public State(IStateMachineOwner owner)
        {
            m_owner = owner;
        }

        public override string ToString()
        {
            return "";
        }

        public virtual void OnEnter() { }
        public abstract void Update(ref FrameTime frameTime);
        public virtual void OnExit() { }
    }
}
