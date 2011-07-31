using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.Maths;


namespace Momo.Core.Primitives2D
{
    public class CirclePrimitive2D : Primitive2D
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private RadiusInfo mRadiusInfo;
        private Vector2 mCentre = Vector2.Zero;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public float Radius
        {
            get { return mRadiusInfo.Radius; }
            set { mRadiusInfo.Radius = value; }
        }


        public float RadiusSq
        {
            get { return mRadiusInfo.RadiusSq; }
        }


        public Vector2 Centre
        {
            get { return mCentre; }
            set { mCentre = value; }
        }



        public CirclePrimitive2D(float radius)
        {
            mRadiusInfo = new RadiusInfo(radius);
        }


        public CirclePrimitive2D(float radius, Vector2 centre)
        {
            mRadiusInfo = new RadiusInfo(radius);
            mCentre = centre;
        }
    }
}
