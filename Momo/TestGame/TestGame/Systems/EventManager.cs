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

        static readonly int kMaxTimers = 64;
        private Pool<TimerEvent> m_timerEvents = new Pool<TimerEvent>(kMaxTimers);

        static readonly int kMaxSpawns = 64;
        private Pool<SpawnEvent> m_spawnEvents = new Pool<SpawnEvent>(kMaxSpawns);

        Dictionary<Trigger, List<MapData.EventData>> m_triggerDict = new Dictionary<Trigger, List<MapData.EventData>>();

        public EventManager(GameWorld world)
        {
            m_world = world;
        }

        internal void LoadEvents(MapData.Map m_map)
        {
            // Add the objects to the pools
            for (int i = 0; i < kMaxTimers; ++i)
            {
                m_timerEvents.AddItem(new TimerEvent(m_world), false);
            }
            for (int i = 0; i < kMaxSpawns; ++i)
            {
                m_spawnEvents.AddItem(new SpawnEvent(m_world), false);
            }

            // Register the triggers from the data
            for (int i = 0; i < m_map.TimerEvents.Length; ++i)
            {
                RegisterEventData(m_map.TimerEvents[i]);
            }
            for (int i = 0; i < m_map.SpawnEvents.Length; ++i)
            {
                RegisterEventData(m_map.SpawnEvents[i]);
            }
        }

        public void Update(ref FrameTime frameTime)
        {
            {
                bool needsCoalesce = false;
                for (int i = 0; i < m_timerEvents.ActiveItemListCount; ++i)
                {
                    m_timerEvents[i].Update(ref frameTime);
                    if (m_timerEvents[i].IsDestroyed())
                    {
                        needsCoalesce = true;
                    }
                }
                if (needsCoalesce)
                {
                    m_timerEvents.CoalesceActiveList(false);
                }
            }

            {
                bool needsCoalesce = false;
                for (int i = 0; i < m_spawnEvents.ActiveItemListCount; ++i)
                {
                    m_spawnEvents[i].Update(ref frameTime);
                    if (m_spawnEvents[i].IsDestroyed())
                    {
                        needsCoalesce = true;
                    }
                }
                if (needsCoalesce)
                {
                    m_spawnEvents.CoalesceActiveList(false);
                }
            }
        }

        private void RegisterEventData(MapData.EventData data)
        {
            // Add the start trigger to the registry
            Trigger trigger = null;
            if (data.GetStartTrigger() != null)
            {
                trigger = m_world.GetTriggerManager().GetTrigger(data.GetStartTrigger());
            }
            else
            {
                trigger = m_world.GetTriggerManager().GetTrigger(TriggerManager.kDefaultTriggerName);
            }

            RegisterTriggerEvent(trigger, data);
            trigger.RegisterListener(this);
        }

        private void RegisterTriggerEvent(Trigger trigger, MapData.EventData data)
        {
            if (!m_triggerDict.ContainsKey(trigger))
            {
                m_triggerDict[trigger] = new List<MapData.EventData>();
            }
            m_triggerDict[trigger].Add(data);
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
                    if (eventList[i].GetType() == typeof(MapData.TimerEventData))
                    {
                        newEvent = m_timerEvents.CreateItem();
                    }
                    else if(eventList[i].GetType() == typeof(MapData.SpawnEventData))
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
