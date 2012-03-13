using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.Triggers;
using Momo.Core.ObjectPools;
using Momo.Core;

namespace TestGame.Events
{
    public class TimerEvent : Event
    {
        float m_time = 0.0f;

        MapData.TimerEventData m_timerData = null;

        public TimerEvent(Zone world, MapData.EventData data)
            : base(world, data)
        {
        }

        public override void Begin()
        {
            base.Begin();

            m_timerData = (MapData.TimerEventData)(EventData);

            m_time = m_timerData.GetTime();
        }

        public override void Update(ref FrameTime frameTime)
        {
            if (!GetIsActive())
                return;

            m_time -= frameTime.Dt;
            if (m_time <= 0.0f)
            {
                End();
            }
        }
    }
}
