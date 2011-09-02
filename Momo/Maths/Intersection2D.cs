using System;

using Microsoft.Xna.Framework;



namespace Momo.Maths
{
    public struct IntersectInfo2D
    {
        private Vector2 m_dPositions;
        private float m_distance;
        private float m_resolveDistance;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public Vector2 PositionDifference
        {
            get { return m_dPositions; }
            set { m_dPositions = value; }
        }

        public float PositionDistance
        {
            get { return m_distance; }
            set { m_distance = value; }
        }

        public float ResolveDistance
        {
            get { return m_resolveDistance; }
            set { m_resolveDistance = value; }
        }
    }
}
