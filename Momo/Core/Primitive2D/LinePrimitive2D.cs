using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core.Primitive2D
{
    public struct LinePrimitive2D
    {
        internal Vector2 m_point;
        internal Vector2 m_normal;
        internal Vector2 m_difference;
        internal float m_lengthSqList;



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
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

        public float LengthSqList
        {
            get { return m_lengthSqList; }
        }


        public LinePrimitive2D(Vector2 point1, Vector2 point2)
        {
            m_point = point1;
            m_difference = point2 - point1;

            // TODO: Use the normalize function below, unroll, and take the lengthSq out first.
            m_normal = new Vector2(-m_difference.Y, m_difference.X);
            m_normal.Normalize();

            m_lengthSqList = m_difference.LengthSquared();
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
