using System;
using System.Collections.Generic;
using System.Text;



namespace Momo.Core
{
    public struct RadiusInfo
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private float m_radius;
        private float m_radiusSq;



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public float Radius
        {
            get { return m_radius; }
            set
            {
                m_radius = value;
                m_radiusSq = m_radius * m_radius;
            }
        }

        public float RadiusSq
        {
            get { return m_radiusSq; }
        }



        public RadiusInfo(float radius)
        {
            m_radius = radius;
            m_radiusSq = m_radius * m_radius;
        }
    }
}
