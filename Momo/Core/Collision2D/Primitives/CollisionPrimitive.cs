using System;

using Microsoft.Xna.Framework;



namespace Momo.Core.Collision2D.Primitives
{
    public abstract class CollisionPrimitive
    {
        public abstract void SetPosRotScale(Vector2 pos, Vector2 rot, Vector2 scale);

        public abstract bool DoesIntersect(CollisionPrimitive primtive, ref Vector2 collisionNormal, ref float overlap);

        public abstract bool DoesIntersect(CollisionPrimitiveAABB primtive, ref Vector2 collisionNormal, ref float overlap);
        public abstract bool DoesIntersect(CollisionPrimitiveCircle primtive, ref Vector2 collisionNormal, ref float overlap);
    }
}
