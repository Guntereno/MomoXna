using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core.Primitive2D
{
    public struct CirclePrimitive2D
    {
        // --------------------------------------------------------------------
        // -- Variables
        // --------------------------------------------------------------------
        private RadiusInfo m_radiusInfo;
        private Vector2 m_centre;


        // --------------------------------------------------------------------
        // -- Properties
        // --------------------------------------------------------------------
        #region Properties
        public float Radius
        {
            get { return m_radiusInfo.Radius; }
            set { m_radiusInfo.Radius = value; }
        }

        public float RadiusSq
        {
            get { return m_radiusInfo.RadiusSq; }
        }

        public RadiusInfo RadiusInfo
        {
            get { return m_radiusInfo; }
        }

        public Vector2 Centre
        {
            get { return m_centre; }
            set { m_centre = value; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Methods
        // --------------------------------------------------------------------
        public CirclePrimitive2D(float radius)
        {
            m_radiusInfo = new RadiusInfo(radius);
            m_centre = Vector2.Zero;
        }


        public CirclePrimitive2D(float radius, Vector2 centre)
        {
            m_radiusInfo = new RadiusInfo(radius);
            m_centre = centre;
        }
    }
}
