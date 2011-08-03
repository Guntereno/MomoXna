using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.Spatial;
using Momo.Core.Collision2D;



namespace Momo.Core.GameEntities
{
    public class DynamicGameEntity : GameEntity, IDynamicCollidable
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private Vector2 m_velocity = Vector2.Zero;
        private Vector2 m_force = Vector2.Zero;

        private MassInfo m_massInfo = new MassInfo(1.0f);



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public override void SetPosition(Vector2 position)
        {
            base.SetPosition(position);
        }

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

        public Vector2 GetLastFramesAcceleration()
        {
            return Vector2.Zero;
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
        }


        public override void PostUpdate()
        {

        }
    }
}
