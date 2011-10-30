using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Momo.Core.Collision2D
{
    public interface IDynamicCollidable
    {
        // --------------------------------------------------------------------
        // -- Internal Members
        // --------------------------------------------------------------------
        void SetPosition(Vector2 position);
        Vector2 GetPosition();


        Vector2 Velocity                { get; set; }
        Vector2 Force                   { get; set; }
        Vector2 LastFrameAcceleration   { get; }

        float Mass                      { get; }
        float InverseMass               { get; }


        void OnCollisionEvent(ref IDynamicCollidable collidable);


        //float Rotation
        //{
        //    get;
        //    set;
        //}

        //float Scale
        //{
        //    get;
        //    set;
        //}

        //bool CanSleep
        //{
        //    get;
        //    set;
        //}

        //bool IsAwake
        //{
        //    get;
        //}

        //float AngularVelocity
        //{
        //    get;
        //    set;
        //}


        //void SetAwake(bool isAwake);
    }
}
