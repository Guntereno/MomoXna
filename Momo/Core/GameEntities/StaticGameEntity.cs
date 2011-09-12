using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core.GameEntities
{
    public class StaticEntity : BaseEntity
    {
        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public override Vector2 GetVelocity()
        {
            return Vector2.Zero;
        }
    }
}
