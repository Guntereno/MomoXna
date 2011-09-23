using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapData
{
    public class TriggerCounterEventData : EventData
    {
        private string m_countTrigger;
        private int m_count;

        public TriggerCounterEventData(string name, string startTrigger, string endTrigger, string countTrigger, int count)
            : base(name, startTrigger, endTrigger)
        {
            m_countTrigger = countTrigger;
            m_count = count;
        }

        public string GetCountTrigger() { return m_countTrigger; }
        public int GetCount() { return m_count; }
    }
}
