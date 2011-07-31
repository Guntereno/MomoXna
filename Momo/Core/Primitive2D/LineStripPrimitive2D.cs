using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core.Primitives2D
{
    public class LineStripPrimitive2D : Primitive2D
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private int mPointCapacity = 0;
        private int mPointCount = 0;

        private Vector2 [] mPointList;
        private Vector2 [] mNormalList;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public LineStripPrimitive2D(int pointCapacity)
        {
            mPointList = new Vector2[pointCapacity];
            mNormalList = new Vector2[pointCapacity - 1];

            mPointCapacity = pointCapacity;
        }


        public void StartAddingPoints()
        {

        }


        public void AddPoint(Vector2 point)
        {
            mPointList[mPointCount] = point;
            ++mPointCount;
        }


        public void EndAddingPoints()
        {
            CalculateNormalList();
        }


        public void RecalculateNormalList()
        {
            CalculateNormalList();
        }


        // --------------------------------------------------------------------
        // -- Private Methods
        // --------------------------------------------------------------------
        private void CalculateNormalList()
        {
            for (int i = 1; i < mPointCount; ++i)
            {
                Vector2 dPoints = mPointList[i] - mPointList[i-1];
                Vector2 normal = new Vector2(-dPoints.Y, dPoints.X);
                normal.Normalize();

                mNormalList[i-1] = normal;
            }
        }

    }
}
