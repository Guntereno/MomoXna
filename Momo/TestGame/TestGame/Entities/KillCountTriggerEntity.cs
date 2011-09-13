using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestGame.Entities
{
    class KillCountTriggerEntity: TriggerEntity
    {
        private int m_threshold;

        public KillCountTriggerEntity(GameWorld world, String name, float triggeredTime, float downTime, int threshold)
            : base(world, name, triggeredTime, downTime)
        {
            m_threshold = threshold;
        }

        public KillCountTriggerEntity(GameWorld world, MapData.KillCountTrigger triggerData)
            : base(world, triggerData)
        {
            m_threshold = triggerData.GetThreshold();
        }

        protected override bool TriggerCondition()
        {
            return (GetWorld().GetEnemyManager().GetKillCount() >= m_threshold);
        }

        protected override void UpdateDebugString()
        {
            m_debugString.Clear();
            m_debugString.Append(GetName());
            m_debugString.Append("(");
            m_debugString.Append(GetWorld().GetEnemyManager().GetKillCount());
            m_debugString.Append("/");
            m_debugString.Append(m_threshold);
            m_debugString.Append(")");
            m_debugString.EndAppend();

            m_debugString = GetName();
        }
    }


}
