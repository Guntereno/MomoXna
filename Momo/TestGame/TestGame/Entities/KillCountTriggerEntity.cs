using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestGame.Entities
{
    class KillCountTriggerEntity: TriggerEntity
    {
        private int m_threshold;

        public KillCountTriggerEntity(GameWorld world, String name, int threshold)
            : base(world, name)
        {
            m_threshold = threshold;
        }

        protected override bool TriggerCondition()
        {
            return (GetWorld().GetEnemyManager().GetKillCount() >= m_threshold);
        }

    }


}
