using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.Spatial;
using Momo.Debug;



namespace Momo.Core.GameEntities
{
    public abstract class GameEntity : BinItem
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private Vector2 mPosition = Vector2.Zero;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public abstract bool IsMoveable();
        public abstract bool GetMovedSinceLastUpdate();
        public abstract Vector2 GetVelocity();


        public Vector2 GetPosition()
        {
            return mPosition;
        }


        public virtual void Update(ref FrameTime frameTime)
        {

        }


        public virtual void DebugRender(DebugRenderer debugRenderer)
        {

        }
    }
}
