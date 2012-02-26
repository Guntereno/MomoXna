using System;

using Microsoft.Xna.Framework;



namespace Momo.Core.Collision2D.Primitives
{
    public abstract class CollisionPrimitive
    {
        public abstract void SetPosRotScale(Vector2 pos, Vector2 rot, float scale);

        public bool DoesIntersect(CollisionPrimitive primtive, float padding, ref Vector2 outIntersectNormal, ref float outPosDiff)
        {
            CollisionPrimitiveCircle primtiveCircle = primtive as CollisionPrimitiveCircle;
            if (primtiveCircle != null)
                return DoesIntersect(primtiveCircle, padding, ref outIntersectNormal, ref outPosDiff);

            CollisionPrimitiveAABB primtiveAABB = primtive as CollisionPrimitiveAABB;
            if (primtiveAABB != null)
                return DoesIntersect(primtiveAABB, padding, ref outIntersectNormal, ref outPosDiff);

            return false;
        }


        public abstract bool DoesIntersect(CollisionPrimitiveCircle primtive, float padding, ref Vector2 outIntersectNormal, ref float outPosDiff);
        public abstract bool DoesIntersect(CollisionPrimitiveAABB primtive, float padding, ref Vector2 outIntersectNormal, ref float outPosDiff);
    }
}
