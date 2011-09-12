using System;
using Microsoft.Xna.Framework;

namespace MapData
{
    public class Object
    {
        private String m_name;
        private Vector2 m_position;

        public Object(string name, Vector2 position)
        {
            m_name = name;
            m_position = position;
        }

        public string GetName() { return m_name; }
        public Vector2 GetPosition() { return m_position; }
    }
}
