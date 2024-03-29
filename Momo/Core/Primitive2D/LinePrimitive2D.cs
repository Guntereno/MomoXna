﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core.Primitive2D
{
    public struct LinePrimitive2D
    {
        // --------------------------------------------------------------------
        // -- Variables
        // --------------------------------------------------------------------
        internal Vector2 m_point;
        internal Vector2 m_normal;
        internal Vector2 m_difference;
        internal float m_lengthSq;


        // --------------------------------------------------------------------
        // -- Properties
        // --------------------------------------------------------------------
        #region Properties
        public Vector2 Point
        {
            get { return m_point; }
        }

        public Vector2 Normal
        {
            get { return m_normal; }
        }

        public Vector2 Difference
        {
            get { return m_difference; }
        }

        public float LengthSq
        {
            get { return m_lengthSq; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Methods
        // --------------------------------------------------------------------
        public LinePrimitive2D(Vector2 point1, Vector2 point2)
        {
            m_point = point1;
            m_difference = point2 - point1;

            m_lengthSq = m_difference.LengthSquared();
            m_normal = new Vector2(-m_difference.Y, m_difference.X);
            m_normal /= (float)Math.Sqrt(m_lengthSq);
        }


        public void CalculateMinMax(out Vector2 outMin, out Vector2 outMax)
        {
            outMin = new Vector2();
            outMax = new Vector2();

            if (m_difference.X > 0.0f)
            {
                outMin.X = m_point.X;
                outMax.X = m_point.X + m_difference.X;
            }
            else
            {
                outMin.X = m_point.X + m_difference.X;
                outMax.X = m_point.X;
            }

            if (m_difference.Y > 0.0f)
            {
                outMin.Y = m_point.Y;
                outMax.Y = m_point.Y + m_difference.Y;
            }
            else
            {
                outMin.Y = m_point.Y + m_difference.Y;
                outMax.Y = m_point.Y;
            }
        }
    }
}
