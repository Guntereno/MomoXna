using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Triggers
{
    public class Trigger : INamed
    {
        List<ITriggerListener> m_listeners = new List<ITriggerListener>();

        const int kMaxNameLength = 32;
        private MutableString m_name = new MutableString(kMaxNameLength);
        private int m_nameHash = 0;

        public Trigger(string name)
        {
            m_name.Set(name);
        }

        public void RegisterListener(ITriggerListener listener)
        {
            m_listeners.Add(listener);
        }

        public void Activate()
        {
            for(int l=0; l<m_listeners.Count; ++l)
            {
                m_listeners[l].OnReceiveTrigger(this);
            }
        }

        // --------------------------------------------------------------------
        // -- INamed interface implementation
        // --------------------------------------------------------------------
        public void SetName(MutableString name)
        {
            m_name.Set(name);
            m_nameHash = Hashing.GenerateHash(m_name.GetCharacterArray(), m_name.GetLength());
        }

        public void SetName(string name)
        {
            m_name.Set(name);
            m_nameHash = Hashing.GenerateHash(m_name.GetCharacterArray(), m_name.GetLength());
        }

        public MutableString GetName()
        {
            return m_name;
        }

        public int GetNameHash()
        {
            return m_nameHash;
        }

    }
}
