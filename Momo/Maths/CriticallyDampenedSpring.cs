using Microsoft.Xna.Framework;

namespace Momo.Maths
{
    public class CriticallyDampenedSpring
    {
        private float m_smoothTime = 0.3f; //	Max time to reach target

        private Vector2 m_valFrom = Vector2.Zero; //	Start position of spring head
        private Vector2 m_valTo = Vector2.Zero; // Target position of spring head

        private Vector2 m_springVel = Vector2.Zero; // Current velocity of spring head

        private Vector2 m_currentValue = Vector2.Zero; // Current value of the spring 

        private float m_quadraticCoef = 0.48f;

        private float m_cubicCoef = 0.235f;

        public float GetSmoothTime() { return m_smoothTime; }
        public void SetSmoothTime(float val) { m_smoothTime = val; }

        public Vector2 GetCurrentValue() { return m_currentValue; }
        public void SetCurrentValue(Vector2 val) { m_currentValue = val; }

        public Vector2 GetSpringVel() { return m_springVel; }
        public void SetSpringVel(Vector2 val) { m_springVel = val; }

        public float GetQuadraticCoef() { return m_quadraticCoef; }
        public void SetQuadraticCoef(float val) { m_quadraticCoef = val; }

        public float GetCubicCoef() { return m_cubicCoef; }
        public void SetCubicCoef(float val) { m_cubicCoef = val; }

        public void SetTarget(Vector2 target)
        {
            m_valFrom = m_currentValue;
            m_valTo = target;
        }
        public Vector2 GetTarget() { return m_valTo; }

        public void Update(float frameTime)
        {
            Vector2 diff, temp;
            float omega, rX, expo;
     
            omega = 2.0f / m_smoothTime;
            rX = omega * frameTime;
            expo = 1.0f / (1.0f + rX + (m_quadraticCoef * rX * rX) + (m_cubicCoef * rX * rX * rX));
     
            diff = m_valFrom - m_valTo;
            temp = (m_springVel + (diff * omega)) * frameTime;
            m_springVel = (m_springVel - (temp * omega)) * expo;
        
            m_currentValue = m_valTo + ((diff + temp) * expo);
        }

    }
}
