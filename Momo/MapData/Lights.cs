using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Map
{
    public class Light
    {
        public enum Type
        {
            PointLight
        }

        public string Name {get; private set;}
        public Color Colour { get; set; }
        public Rectangle Area
        {
            get
            {
                return m_areaRect;
            }
        }

        public Light(string name)
        {
            Name = name;
            Colour = Color.Wheat;
            m_areaRect = new Rectangle();
        }

        protected Rectangle m_areaRect;
    }

    public class PointLight : Light
    {
        public PointLight(string name) :
            base (name)
        {
            Strength = 1.0f;
            Position = new Vector3(0.0f, 0.0f, 200.0f);
            Radius = 256.0f;

            ConstantAttenuation = 0.0f;
            LinearAttenuation = 0.004f;
            QuadraticAttenuation = 0.00005f;

            Colour = new Color(219, 248, 255);
        }

        public float Strength { get; set; }

        public float Radius
        {
            get { return m_radius; }
            set
            {
                m_radius = value;
                CalculateAreaRect();
                CalculateWorldMatrix();
            }
        }
        public Vector3 Position
        {
            get { return m_position; }
            set
            {
                m_position = value;
                CalculateAreaRect();
                CalculateWorldMatrix();
            }
        }

        public Matrix WorldMatrix { get; private set; }

        public float ConstantAttenuation { get; set; }
        public float LinearAttenuation { get; set; }
        public float QuadraticAttenuation { get; set; }

        private void CalculateAreaRect()
        {
            m_areaRect.X = (int)(m_position.X - Radius);
            m_areaRect.Y = (int)(m_position.Y - Radius);

            m_areaRect.Width = (int)(Radius * 2.0f);
            m_areaRect.Height = (int)(Radius * 2.0f);
        }

        private void CalculateWorldMatrix()
        {
            Matrix matrix = Matrix.CreateScale(m_radius);
            matrix *= Matrix.CreateTranslation(m_position);
            WorldMatrix = matrix;
        }

        private Vector3 m_position;
        private float m_radius;
    }
}
