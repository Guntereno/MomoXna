using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core.GameEntiies
{
    public class StaticGameEntity : GameEntity
    {
        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public override bool IsMoveable()
        {
            return false;
        }


        public override bool GetMovedSinceLastUpdate()
        {
            return false;
        }


        public override Vector2 GetVelocity()
        {
            return Vector2.Zero;
        }
    }
}
