using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Triggers
{
    public class Trigger /*: INamed*/
    {
        List<ITriggerListener> m_listeners = new List<ITriggerListener>();

        private string m_name;

        public Trigger(string name)
        {
            m_name = name;
        }

        public void RegisterListener(ITriggerListener listener)
        {
            if (!m_listeners.Contains(listener))
            {
                m_listeners.Add(listener);
            }
        }

        public void Activate()
        {
            //Console.Out.WriteLine("Trigger {0} activated", m_name);
            
            for(int l=0; l<m_listeners.Count; ++l)
            {
                m_listeners[l].OnReceiveTrigger(this);
            }
        }

        // --------------------------------------------------------------------
        // -- INamed interface implementation
        // --------------------------------------------------------------------
        // Not currently using the INamed as I've had some problems with the mutable string

        public string GetName()
        {
            return m_name;
        }

        public int GetNameHash()
        {
            return m_name.GetHashCode();
        }

    }
}
