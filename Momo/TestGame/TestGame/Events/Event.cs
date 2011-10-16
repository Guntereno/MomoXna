using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.Triggers;
using Momo.Core.ObjectPools;
using Momo.Core;

namespace TestGame.Events
{
    public abstract class Event
    {
        private GameWorld m_world;

        private bool m_isActive = false;

        MapData.EventData m_data = null;

        Trigger m_endTrigger = null;



        public Event(GameWorld world, MapData.EventData data)
        {
            m_world = world;
            m_data = data;

            if (m_data.GetEndTrigger() != null)
            {
                m_endTrigger = world.TriggerManager.RegisterTrigger(m_data.GetEndTrigger());
            }
        }

        public bool GetIsActive() { return m_isActive; }

        public virtual void Begin()
        {
            Trigger.Log("Event {0} began", m_data.GetName());
            m_isActive = true;
        }

        public abstract void Update(ref FrameTime frameTime);

        public GameWorld GetWorld()
        {
            return m_world;
        }

        public MapData.EventData GetData()
        {
            return m_data;
        }

        protected virtual void End()
        {
            Trigger.Log("Event {0} ended", m_data.GetName());

            m_isActive = false;

            if (m_endTrigger != null)
            {
                m_endTrigger.Activate();
            }
        }
    }
}
