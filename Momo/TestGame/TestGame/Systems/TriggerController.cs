using System;
using System.Diagnostics;

using Momo.Core;
using Momo.Debug;
using Momo.Fonts;

using TestGame.Entities;



namespace TestGame.Systems
{
    class TriggerController
    {
        private GameWorld m_world;
        private TriggerEntity[] m_triggers;


        public TriggerController(GameWorld world)
        {
            m_world = world;
        }


        public void LoadFromMapData(MapData.Map mapData)
        {
            int triggerCount = mapData.Triggers.Length;
            m_triggers = new TriggerEntity[triggerCount];

            for (int triggerIdx = 0; triggerIdx < triggerCount; ++triggerIdx)
            {
                MapData.Trigger triggerData = mapData.Triggers[triggerIdx];
                Type triggerDataType = triggerData.GetType();

                TriggerEntity trigger = null;
                if (triggerDataType == typeof(MapData.KillCountTrigger))
                {
                    trigger = new KillCountTriggerEntity(m_world, (MapData.KillCountTrigger)triggerData);
                }
                else
                {
                    Debug.Assert(false, "Invalid trigger type!");
                }

                trigger.SetActive(true);
                m_triggers[triggerIdx] = trigger;
            }
        }


        public void Update(ref FrameTime frameTime)
        {
            for (int i = 0; i < m_triggers.Length; ++i)
            {
                m_triggers[i].Update(ref frameTime, i);
            }
        }


        public void DebugRender(DebugRenderer debugRenderer, TextBatchPrinter debugTextBatchPrinter, TextStyle debugTextStyle)
        {
            for (int i = 0; i < m_triggers.Length; ++i)
            {
                m_triggers[i].DebugRender(debugRenderer, debugTextBatchPrinter, debugTextStyle);
            }
        }
    }
}
