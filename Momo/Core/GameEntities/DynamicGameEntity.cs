using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core.GameEntities
{
    public class DynamicGameEntity : GameEntity
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private Vector2 mVelocity = Vector2.Zero;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public override bool IsMoveable()
        {
            return true;
        }


        // Replace with proper check.
        public override bool GetMovedSinceLastUpdate()
        {
            return true;
        }


        public override Vector2 GetVelocity()
        {
            return mVelocity;
        }
    }
}
