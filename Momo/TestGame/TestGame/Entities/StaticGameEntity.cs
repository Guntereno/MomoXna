using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Momo.Core.GameEntities;
using Microsoft.Xna.Framework;

namespace TestGame.Entities
{
    class StaticGameEntity: StaticEntity
    {
        private GameWorld m_world;
        private Color m_debugColor = Color.White;


        public StaticGameEntity(GameWorld world)
        {
            m_world = world;
        }


        public Color DebugColor
        {
            get { return m_debugColor; }
            set { m_debugColor = value; }
        }


        public GameWorld GetWorld()
        {
            return m_world;
        }

    }
}
