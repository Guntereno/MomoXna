using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Debug;

namespace Momo.Core.StateMachine
{
    public abstract class State
    {
        #region Fields

        private IStateMachineOwner m_owner = null;

        #endregion


        #region Constructor

        public State(IStateMachineOwner owner)
        {
            m_owner = owner;
        }

        #endregion


        #region Public Interface

        public IStateMachineOwner Owner
        {
            get
            {
                return m_owner;
            }
        }

        public override string ToString()
        {
            return "";
        }

        public virtual void OnEnter() { }
        public abstract void Update(ref FrameTime frameTime, uint updateToken);
        public virtual void OnExit() { }

        public virtual void DebugRender(DebugRenderer debugRenderer) { }
        
        #endregion
    }
}
