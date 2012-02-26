using System;

using Microsoft.Xna.Framework;

using Momo.Core.Primitive2D;
using Momo.Maths;



namespace Momo.Core.Collision2D.Primitives
{
    public class CollisionPrimitiveCircle : CollisionPrimitive
    {
        // --------------------------------------------------------------------
        // -- Variables
        // --------------------------------------------------------------------
        private CirclePrimitive2D m_primtive;
        private float m_radius = 0.0f;


        // --------------------------------------------------------------------
        // -- Properties
        // --------------------------------------------------------------------
        #region Properties
        public Vector2 Centre
        {
            get { return m_primtive.Centre; }
        }

        public float Radius
        {
            get { return m_primtive.Radius; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Methods
        // --------------------------------------------------------------------
        public CollisionPrimitiveCircle(float radius)
        {
            m_radius = radius;
        }

        public override void SetPosRotScale(Vector2 pos, Vector2 rot, float scale)
        {
            m_primtive.Centre = pos;
            m_primtive.Radius = m_radius * scale;
        }

        public override bool DoesIntersect(CollisionPrimitiveCircle primtive, float padding, ref Vector2 outIntersectNormal, ref float outPosDiff)
        {
            bool contact = Maths2D.DoesIntersect(   m_primtive.Centre,
                                                    m_primtive.Radius + padding,
                                                    primtive.Centre,
                                                    primtive.Radius,
                                                    ref outPosDiff,
                                                    ref outIntersectNormal);

            return contact;
        }

        public override bool DoesIntersect(CollisionPrimitiveAABB primtive, float padding, ref Vector2 outIntersectNormal, ref float outPosDiff)
        {
            return false;
        }
    }
}
