using System;

using Microsoft.Xna.Framework;



namespace Momo.Core.Collision2D.Primitives
{
    public class CollisionPrimitiveAABB : CollisionPrimitive
    {
        // --------------------------------------------------------------------
        // -- Variables
        // --------------------------------------------------------------------


        // --------------------------------------------------------------------
        // -- Properties
        // --------------------------------------------------------------------
        #region Properties

        #endregion


        // --------------------------------------------------------------------
        // -- Methods
        // --------------------------------------------------------------------
        public override void SetPosRotScale(Vector2 pos, Vector2 rot, float scale)
        {

        }

        public override bool DoesIntersect(CollisionPrimitiveCircle primtive, float padding, ref Vector2 outIntersectNormal, ref float outPosDiff)
        {
            return false;
        }

        public override bool DoesIntersect(CollisionPrimitiveAABB primtive, float padding, ref Vector2 outIntersectNormal, ref float outPosDiff)
        {
            return false;
        }
    }
}
