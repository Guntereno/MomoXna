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
        // -- Public Properties
        // --------------------------------------------------------------------
        public static float ContactDimensionPadding { get { return kContactDimensionPadding; } }

        public float Mass
        {
            get { return m_massInfo.Mass; }
            set { m_massInfo.Mass = value;  }
        }

        public float InverseMass                { get { return m_massInfo.InverseMass; } }

        public virtual Vector2 Velocity
        {
            get { return m_velocity; }
            set { m_velocity = value; }
        }

        public Vector2 Force
        {
            get { return m_force; }
            set { m_force = value; }
        }

        public Vector2 LastFrameAcceleration    { get { return m_lastFrameAcceleration; } }


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public void AddVelocity(Vector2 velocity)
        {
            m_velocity += velocity;
        }


        public void AddForce(Vector2 force)
        {
            m_force += force;
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

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
