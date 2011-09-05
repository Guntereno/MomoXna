using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MapData
{
    public struct Enemy
    {
        private String m_name;
        private Vector2 m_position;

        public Enemy(string name, Vector2 position)
        {
            m_name = name;
            m_position = position;
        }

        public string GetName() { return m_name; }
        public Vector2 GetPosition() { return m_position; }
    }
}
