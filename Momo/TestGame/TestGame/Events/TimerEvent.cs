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
        Trigger m_onFinish = null;

        MapData.TimerEventData m_timerData = null;

        public TimerEvent(GameWorld world): base(world)
        {
        }

        public override void Begin(MapData.EventData data)
        {
            System.Diagnostics.Debug.Assert(GetData() != null);
            m_timerData = (MapData.TimerEventData)(GetData());

            m_time = m_timerData.GetTime();

            base.Begin(data);
        }

        public override void Update(ref FrameTime frameTime)
        {
            m_time -= frameTime.Dt;
            if (m_time <= 0.0f)
            {
                DestroyItem();
            }
        }
    }
}
