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
        Dictionary<string, Trigger> m_triggers = new Dictionary<string, Trigger>();

        MutableString m_lookupBuffer = new MutableString(128);

        Trigger m_defaultTrigger = null;
        public static readonly string kDefaultTriggerName = "worldStart";


        public TriggerManager()
        {
            // Default trigger called at start of stage
            m_defaultTrigger = new Trigger(kDefaultTriggerName);
            m_triggers[kDefaultTriggerName] = m_defaultTrigger;
        }

        public Trigger GetTrigger(string name)
        {
            if (m_triggers.ContainsKey(name))
            {
                return m_triggers[name];
            }
            else
            {
                // If the trigger doesn't exist yet, create it
                Trigger newTrigger = new Trigger(name);
                m_triggers[name] = newTrigger;
                return newTrigger;
            }
        }
    }
}
