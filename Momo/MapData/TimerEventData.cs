using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapData
{
    public class TimerEventData : EventData
    {
        private float m_time;

        public TimerEventData(string name, string startTrigger, string endTrigger, float time)
            : base(name, startTrigger, endTrigger)
        {
            m_time = time;
        }

        public float GetTime() { return m_time; }
    }
}
