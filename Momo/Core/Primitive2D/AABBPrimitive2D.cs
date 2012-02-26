using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core.Primitive2D
{
    public struct AABBPrimitive2D
    {
        // --------------------------------------------------------------------
        // -- Variables
        // --------------------------------------------------------------------
        private Vector2 m_min;
        private Vector2 m_max;


        // --------------------------------------------------------------------
        // -- Properties
        // --------------------------------------------------------------------
        #region Properties
        public Vector2 Min
        {
            get { return m_min; }
            set { m_min = value; }
        }

        public Vector2 Max
        {
            get { return m_max; }
            set { m_max = value; }
        }

        public Vector2 Centre
        {
            get { return (m_min + m_max) * 0.5f; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Methods
        // --------------------------------------------------------------------
        public AABBPrimitive2D(Vector2 centre, float width, float height)
        {
            Vector2 hExtent = new Vector2(width * 0.5f, height * 0.5f);
            m_min = centre - hExtent;
            m_max = centre + hExtent;
        }


        public AABBPrimitive2D(Vector2 min, Vector2 max)
        {
            m_min = min;
            m_max = max;
        }
    }
}
