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

        private TimerEvent[] m_timerEvents = null;
        private SpawnEvent[] m_spawnEvents = null;
        private TriggerCounterEvent[] m_triggerCounterEvents = null;

        private Dictionary<Trigger, List<Event>> m_triggerDict = new Dictionary<Trigger, List<Event>>();



        public EventManager(GameWorld world)
        {
            m_world = world;
        }


        internal void LoadEvents(MapData.Map map)
        {
            // Add the objects
            m_timerEvents = new TimerEvent[map.TimerEvents.Length];
            for (int i = 0; i < map.TimerEvents.Length; ++i)
            {
                m_timerEvents[i] = new TimerEvent(m_world, map.TimerEvents[i]);
                RegisterEventData(m_timerEvents[i]);
            }
            m_spawnEvents = new SpawnEvent[map.SpawnEvents.Length];
            for (int i = 0; i < map.SpawnEvents.Length; ++i)
            {
                m_spawnEvents[i] = new SpawnEvent(m_world, map.SpawnEvents[i]);
                RegisterEventData(m_spawnEvents[i]);
            }
            m_triggerCounterEvents = new TriggerCounterEvent[map.TriggerCounterEvents.Length];
            for (int i = 0; i < map.TriggerCounterEvents.Length; ++i)
            {
                m_triggerCounterEvents[i] = new TriggerCounterEvent(m_world, map.TriggerCounterEvents[i]);
                RegisterEventData(m_triggerCounterEvents[i]);
            }
        }

        public void Update(ref FrameTime frameTime)
        {
            // TODO: Candidate for update list
            for (int i = 0; i < m_timerEvents.Length; ++i)
            {
                m_timerEvents[i].Update(ref frameTime);
            }
            for (int i = 0; i < m_spawnEvents.Length; ++i)
            {
                m_spawnEvents[i].Update(ref frameTime);
            }
            for (int i = 0; i < m_triggerCounterEvents.Length; ++i)
            {
                m_triggerCounterEvents[i].Update(ref frameTime);
            }
        }

        private void RegisterEventData(Event eventInst)
        {
            // Add the start trigger to the registry
            Trigger trigger = null;
            if (eventInst.EventData.GetStartTrigger() != null)
            {
                trigger = m_world.TriggerManager.RegisterTrigger(eventInst.EventData.GetStartTrigger());
            }
            else
            {
                trigger = m_world.TriggerManager.RegisterTrigger(TriggerManager.kDefaultTriggerName);
            }

            RegisterTriggerEvent(trigger, eventInst);
            trigger.RegisterListener(this);
        }

        private void RegisterTriggerEvent(Trigger trigger, Event eventInst)
        {
            if (!m_triggerDict.ContainsKey(trigger))
            {
                m_triggerDict[trigger] = new List<Event>();
            }
            m_triggerDict[trigger].Add(eventInst);
        }

        // --------------------------------------------------------------------
        // -- ITriggerListener interface implementation
        // --------------------------------------------------------------------
        public void OnReceiveTrigger(Trigger trigger)
        {
            if (m_triggerDict.ContainsKey(trigger))
            {
                List<Event> eventList = m_triggerDict[trigger];
                for (int i = 0; i < eventList.Count; ++i)
                {
                    if(!eventList[i].GetIsActive())
                        eventList[i].Begin();
                }
            }
        }
    }
}
