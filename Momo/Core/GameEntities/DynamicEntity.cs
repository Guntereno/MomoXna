using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.Spatial;
using Momo.Core.Collision2D;



namespace Momo.Core.GameEntities
{
    public class DynamicEntity : BaseEntity, IDynamicCollidable
    {
        // --------------------------------------------------------------------
        // -- Private Static Members
        // --------------------------------------------------------------------
        private static float ms_contactDimensionPadding = 1.0f;


        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private Vector2 m_velocity = Vector2.Zero;
        private Vector2 m_force = Vector2.Zero;
        private Vector2 m_lastFrameAcceleration = Vector2.Zero;

        private MassInfo m_massInfo = new MassInfo(1.0f);


        // --------------------------------------------------------------------
        // -- Public Static Methods
        // --------------------------------------------------------------------
        public static float GetContactDimensionPadding()
        {
            return ms_contactDimensionPadding;
        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void SetVelocity(Vector2 velocity)
        {
            m_velocity = velocity;
        }

        public override Vector2 GetVelocity()
        {
            return m_velocity;
        }

        public void SetForce(Vector2 force)
        {
            m_force = force;
        }

        public Vector2 GetForce()
        {
            return m_force;
        }

        public Vector2 GetLastFrameAcceleration()
        {
            return m_lastFrameAcceleration;
        }

        public void SetMass(float mass)
        {
            m_massInfo.Mass = mass;
        }


        public float GetMass()
        {
            return m_massInfo.Mass;
        }


        public float GetInverseMass()
        {
            return m_massInfo.InverseMass;
        }


        public override void Update(ref FrameTime frameTime)
        {
            base.Update(ref frameTime);

            Vector2 newPosition = GetPosition() + (m_velocity * frameTime.Dt);

            m_lastFrameAcceleration = (m_force * frameTime.Dt);
            m_velocity = m_velocity + m_lastFrameAcceleration;
            m_velocity *= 0.90f;

            m_force = Vector2.Zero;

    
            SetPosition(newPosition);
        }


        public override void PostUpdate()
        {

        }


        public virtual void OnCollisionEvent(ref IDynamicCollidable collidable)
        {

        }
    }
}
