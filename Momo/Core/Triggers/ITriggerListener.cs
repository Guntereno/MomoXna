using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Triggers
{
    public interface ITriggerListener
    {
        void OnReceiveTrigger(Trigger trigger);
    }
}
