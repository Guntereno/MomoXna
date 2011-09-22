using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapData
{
    public class EventData : NamedData
    {
        private string m_startTrigger;
        private string m_endTrigger;

        public EventData(string name, string startTrigger, string endTrigger): base(name)
        {
            m_startTrigger = startTrigger;
            m_endTrigger = endTrigger;
        }

        public string GetStartTrigger() { return m_startTrigger; }
        public string GetEndTrigger() { return m_endTrigger; }
    }


    

}
