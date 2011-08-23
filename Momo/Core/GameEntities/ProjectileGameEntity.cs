using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.Spatial;
using Momo.Core.Collision2D;



namespace Momo.Core.GameEntities
{
    public class ProjectileGameEntity : BaseEntity
    {
        // --------------------------------------------------------------------
        // -- Private Static Members
        // --------------------------------------------------------------------
        private static float ms_contactDimensionPadding = 1.0f;


        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private Vector2 m_velocity = Vector2.Zero;
        private Vector2 m_lastFramePosition = Vector2.Zero;
        private Vector2 m_positionDifferenceFromlastFrame = Vector2.Zero;


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
        public override void SetPosition(Vector2 position)
        {
            //m_lastFramePosition = position;
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

        public Vector2 GetLastFramePosition()
        {
            return m_lastFramePosition;
        }

        public Vector2 GetPositionDifferenceFromLastFrame()
        {
            return m_positionDifferenceFromlastFrame;
        }



        public override void Update(ref FrameTime frameTime)
        {
            base.Update(ref frameTime);

            m_lastFramePosition = m_position;
            m_position = m_position + (m_velocity * frameTime.Dt);
            m_positionDifferenceFromlastFrame = m_position - m_lastFramePosition;
        }


        public override void PostUpdate()
        {

        }


        // --------------------------------------------------------------------
        // -- Private Methods
        // --------------------------------------------------------------------
    }
}
