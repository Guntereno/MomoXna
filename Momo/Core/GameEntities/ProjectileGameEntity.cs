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
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties
        public static float ContactDimensionPadding     { get { return ms_contactDimensionPadding; } }

        public Vector2 LastFramePosition                { get { return m_lastFramePosition; } }
        public Vector2 PositionDifferenceFromLastFrame  { get { return m_positionDifferenceFromlastFrame; } }
        
        public Vector2 Velocity
        {
            get { return m_velocity; }
            set { m_velocity = value; } 
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public override void SetPosition(Vector2 position)
        {
            //m_lastFramePosition = position;
            base.SetPosition(position);
        }


        public override void Update(ref FrameTime frameTime, uint updateToken)
        {
            base.Update(ref frameTime, updateToken);

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
