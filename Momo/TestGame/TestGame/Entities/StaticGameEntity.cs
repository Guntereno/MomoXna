using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.GameEntities;

namespace TestGame.Entities
{
    class StaticGameEntity: StaticEntity
    {
        GameWorld m_world;

        public StaticGameEntity(GameWorld world)
        {
            m_world = world;
        }

        public GameWorld GetWorld()
        {
            return m_world;
        }

    }
}
