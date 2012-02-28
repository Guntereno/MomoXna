using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.StateMachine
{
    public interface IStateMachineOwner
    {
        StateMachine StateMachine { get; }
    }
}
