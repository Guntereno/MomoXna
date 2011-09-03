using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Maths;
using Momo.Core.Spatial;
using Momo.Core.Collision2D;



namespace Momo.Core.GameEntities
{
    public class DynamicEntity : BaseEntity, IDynamicCollidable
    {
        // --------------------------------------------------------------------
        // -- Private Static Members
        // --------------------------------------------------------------------
        private static readonly float kContactDimensionPadding = 1.0f;

        private static readonly float kMaxVelocity = 500.0f;
        private static readonly float kMaxVelocitySq = kMaxVelocity * kMaxVelocity;

        private static readonly float kMaxAcceleration = 500.0f;
        private static readonly float kMaxAccelerationSq = kMaxAcceleration * kMaxAcceleration;
        
        private static readonly float kFriction = 9000.0f;


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
            return kContactDimensionPadding;
        }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void SetVelocity(Vector2 velocity)
        {
            m_velocity = velocity;
        }

        public void AddVelocity(Vector2 velocity)
        {
            m_velocity += velocity;
        }

        public override Vector2 GetVelocity()
        {
            return m_velocity;
        }

        public void SetForce(Vector2 force)
        {
            m_force = force;
        }

        public void AddForce(Vector2 force)
        {
            m_force += force;
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

            // Acceleration update
            Vector2 acceleration = ((m_force * m_massInfo.InverseMass) * frameTime.Dt);
           
            // Cap the acceleration
            ExtendedMaths2D.CapVectorMagnitude(ref acceleration, kMaxAcceleration, kMaxAccelerationSq);


            // Velocity update
            m_velocity = m_velocity + acceleration;

            // Cap the velocity
            float velocityMagSq = m_velocity.LengthSquared();
            if (velocityMagSq > 0.0f)
            {
                float velocityMag = (float)Math.Sqrt(velocityMagSq);
                float velocityMagAfterFriction = velocityMag - (kFriction * m_massInfo.InverseMass * frameTime.Dt);

                if (velocityMagAfterFriction < 0.0f)
                {
                    m_velocity = Vector2.Zero;
                }
                else
                {
                    if (velocityMagAfterFriction > kMaxVelocity)
                        velocityMagAfterFriction = kMaxVelocity;

                    Vector2 normalisedVelocity = (m_velocity / velocityMag);
                    m_velocity = (normalisedVelocity * velocityMagAfterFriction);
                }
            }


            m_force = Vector2.Zero;
            m_lastFrameAcceleration = acceleration;
    
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
