using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Momo.Core
{
    public struct MassInfo
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private float m_mass;
        private float m_invMass;



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public float Mass
        {
            get { return m_mass; }
            set
            {
                m_mass = value;
                m_invMass = 1.0f / m_mass;
            }
        }

        public float InverseMass
        {
            get { return m_invMass; }
        }



        public MassInfo(float mass)
        {
            m_mass = mass;
            m_invMass = 1.0f / m_mass;
        }
    }
}
