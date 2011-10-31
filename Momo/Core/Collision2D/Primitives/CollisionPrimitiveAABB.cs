using System;

using Microsoft.Xna.Framework;



namespace Momo.Core.Collision2D.Primitives
{
    public class CollisionPrimitiveAABB : CollisionPrimitive
    {
        public override void SetPosRotScale(Vector2 pos, Vector2 rot, Vector2 scale)
        {

        }

        public override bool DoesIntersect(CollisionPrimitive primtive, ref Vector2 collisionNormal, ref float overlap)
        {
            return false;
        }

        public override bool DoesIntersect(CollisionPrimitiveAABB primtive, ref Vector2 collisionNormal, ref float overlap)
        {
            return false;
        }

        public override bool DoesIntersect(CollisionPrimitiveCircle primtive, ref Vector2 collisionNormal, ref float overlap)
        {
            return false;
        }
    }
}
