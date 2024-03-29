﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapData;
using Momo.Core.Triggers;

namespace Game.Events
{
    class TriggerCounterEvent : Event, ITriggerListener
    {
        private MapData.TriggerCounterEventData m_triggerCounterData = null;

        private Trigger m_countTrigger = null;
        private int m_count = 0;

        public TriggerCounterEvent(Zone zone, MapData.TriggerCounterEventData triggerCounterEventData)
            : base(zone, triggerCounterEventData)
        {
            m_triggerCounterData = triggerCounterEventData;

            m_countTrigger = zone.TriggerManager.RegisterTrigger(m_triggerCounterData.GetCountTrigger());
            m_countTrigger.RegisterListener(this);
        }

        public override void Begin()
        {
            base.Begin();

            m_count = 0;
        }

        public override void Update(ref Momo.Core.FrameTime frameTime)
        {
            if (!GetIsActive())
                return;

            if (m_count == m_triggerCounterData.GetCount())
            {
                End();
            }
        }


        // --------------------------------------------------------------------
        // -- ITriggerListener interface implementation
        // --------------------------------------------------------------------
        public void OnReceiveTrigger(Trigger trigger)
        {
            if (trigger == m_countTrigger)
            {
                ++m_count;
            }
        }
    }
}
