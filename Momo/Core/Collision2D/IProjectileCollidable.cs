using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Core.Collision2D
{
    public interface IProjectileCollidable
    {
        Vector2 Position
        {
            get;
        }

        Vector2 DifferenceFromLastPosition
        {
            get;
        }

        Vector2 Direction
        {
            get;
        }

        float Speed
        {
            get;
        }

        Rectangle BoundingRectangle
        {
            get;
        }

        //void OnBoundaryCollision(Boundary boundary);
    }
}
