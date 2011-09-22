using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.Triggers;
using Momo.Core;

namespace TestGame.Systems
{
    public class TriggerManager
    {
        List<Trigger> m_triggers = new List<Trigger>();

        MutableString lookupBuffer = new MutableString(128);

        public TriggerManager()
        {
            // Default trigger called at start of stage
            Trigger defaultTrigger = new Trigger("worldStart");
            m_triggers.Add(defaultTrigger);
        }

        public Trigger GetTrigger(string name)
        {
            // TODO: This is horrible
            lookupBuffer.Set(name);
            return GetTrigger(lookupBuffer);
        }

        public Trigger GetTrigger(MutableString name)
        {
            int hash = name.GetHashCode();
            for (int t = 0; t < m_triggers.Count; ++t)
            {
                if (m_triggers[t].GetHashCode() == hash)
                {
                    return m_triggers[t];
                }
            }

            return null;
        }
    }
}
