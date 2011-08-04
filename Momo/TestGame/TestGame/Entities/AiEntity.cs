using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core;
using Momo.Core.GameEntities;
using Momo.Core.Primitive2D;
using Momo.Core.Spatial;
using Momo.Debug;



namespace TestGame.Entities
{
    public class AiEntity : DynamicGameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private CirclePrimitive2D mCollisionPrimitive = new CirclePrimitive2D(10.0f);



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public override void Update(ref FrameTime frameTime)
        {


            // Update the collision primitive
            mCollisionPrimitive.Centre = GetPosition();
        }


        public override void DebugRender(DebugRenderer debugRenderer)
        {
            debugRenderer.DrawCircle(GetPosition(), mCollisionPrimitive.Radius, new Color(1.0f, 0.0f, 0.0f, 0.5f), Color.Black, true, 3, 12);
        }
    }
}
