using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MapData
{
    public class PressurePlateData
    {
        public enum Type
        {
            Normal = 0,
            Interactive = 1
        }

        private string m_name;
        private Vector2 m_position;
        private string m_trigger;
        private float m_radius;

        public string Name{ get{ return m_name; } }
        public Vector2 Position{ get{ return m_position; } }
        public string TriggerName{ get{ return m_trigger; } }
        public float Radius { get { return m_radius; } }

        public PressurePlateData(string name, Vector2 position, float radius, string trigger)
        {
            m_name = name;
            m_position = position;
            m_radius = radius;
            m_trigger = trigger;
        }


    }

    public class InteractivePressurePlateData: PressurePlateData
    {
        private float m_interactTime;

        public float InteractTime{ get{ return m_interactTime; } }

        public InteractivePressurePlateData(string name, Vector2 position, float radius, string trigger, float interactTime)
            : base(name, position, radius, trigger)
        {
            m_interactTime = interactTime;
        }
    }
}
