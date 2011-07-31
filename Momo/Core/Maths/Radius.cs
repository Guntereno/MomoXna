using System;
using System.Collections.Generic;
using System.Text;



namespace Momo.Core.Maths
{
    struct RadiusInfo
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private float mRadius;
        private float mRadiusSq;



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public float Radius
        {
            get { return mRadius; }
            set
            {
                mRadius = value;
                mRadiusSq = mRadius * mRadius;
            }
        }

        public float RadiusSq
        {
            get { return mRadiusSq; }
        }



        public RadiusInfo(float radius)
        {
            mRadius = radius;
            mRadiusSq = (float)Math.Sqrt(radius);
        }
    }
}
