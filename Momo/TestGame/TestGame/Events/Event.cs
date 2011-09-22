using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.Triggers;
using Momo.Core.ObjectPools;
using Momo.Core;

namespace TestGame.Events
{
    public abstract class Event : IPoolItem
    {
        private GameWorld m_world;

        private bool m_isFinished = true;

        MapData.EventData m_data = null;




        public Event(GameWorld world)
        {
            m_world = world;
        }


        public virtual void Begin(MapData.EventData data)
        {
            m_data = data;
            Console.Out.WriteLine("Event {0} began", m_data.GetName());
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

        // --------------------------------------------------------------------
        // -- IPool interface implementation
        // --------------------------------------------------------------------
        public bool IsDestroyed()
        {
            return m_isFinished;
        }

        public void DestroyItem()
        {
            Console.Out.WriteLine("Event {0} ended", m_data.GetName());

            m_isFinished = true;

            if (m_data.GetEndTrigger() != null)
            {
                Trigger onFinish = m_world.GetTriggerManager().GetTrigger(m_data.GetEndTrigger());
                onFinish.Activate();
            }
        }

        public void ResetItem()
        {
            m_isFinished = false;
            m_data = null;
        }
    }
}
