using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.Triggers;
using Momo.Core.ObjectPools;
using TestGame.Events;
using Momo.Core;

namespace TestGame.Systems
{
    public class EventManager : ITriggerListener
    {
        private GameWorld m_world;
        private Pool<SpawnEvent> m_spawnEvents = new Pool<SpawnEvent>(64);

        Dictionary<Trigger, List<MapData.EventData>> m_triggerDict = new Dictionary<Trigger, List<MapData.EventData>>();

        public EventManager(GameWorld world)
        {
            m_world = world;
        }

        public void RegisterEventTrigger(Trigger trigger, MapData.SpawnEventData data)
        {
            if(!m_triggerDict.ContainsKey(trigger))
            {
                m_triggerDict[trigger] = new List<MapData.EventData>();
            }
            m_triggerDict[trigger].Add(data);
        }

        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < m_spawnEvents.ActiveItemListCount; ++i)
            {
                m_spawnEvents[i].Update(ref frameTime);
            }
        }


        // --------------------------------------------------------------------
        // -- ITriggerListener interface implementation
        // --------------------------------------------------------------------
        public void OnReceiveTrigger(Trigger trigger)
        {
            if (m_triggerDict.ContainsKey(trigger))
            {
                List<MapData.EventData> eventList = m_triggerDict[trigger];
                for (int i = 0; i < eventList.Count; ++i)
                {
                    Event newEvent = null;
                    if(eventList[i].GetType() == typeof(MapData.SpawnEventData))
                    {
                        newEvent = m_spawnEvents.CreateItem();
                    }

                    if (newEvent != null)
                    {
                        newEvent.Begin(eventList[i]);
                    }
                }
            }
        }
    }
}
