using System;

using Microsoft.Xna.Framework;

using Momo.Debug;


namespace TestGame.Objects
{
    public struct Explosion
    {
        private Vector2 m_position;
        private float m_range;
        private float m_force;



        public Explosion(Vector2 position, float range, float force)
        {
            m_position = position;
            m_range = range;
            m_force = force;
        }


        public Vector2 GetPosition()
        {
            return m_position;
        }

        public void SetPosition(Vector2 position)
        {
            m_position = position;
        }

        public float GetForce()
        {
            return m_force;
        }

        public void SetForce(float force)
        {
            m_force = force;
        }

        public float GetRange()
        {
            return m_range;
        }

        public void SetRange(float range)
        {
            m_range = range;
        }


        public void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawFilledCircle(GetPosition(), GetRange(), new Color(1.0f, 0.0f, 0.0f, 0.6f));
        }
    }
}
