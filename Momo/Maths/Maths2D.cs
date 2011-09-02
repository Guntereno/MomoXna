using System;

using Microsoft.Xna.Framework;



namespace Momo.Maths
{
    public class Maths2D
    {
        public static Vector2 Perpendicular(Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }


        public static void CapVectorMagnitude(ref Vector2 vector, float magnitude, float magnitudeSq)
        {
            float vectorMagSq = vector.LengthSquared();
            if (vectorMagSq > magnitudeSq)
            {
                float vectorMag = (float)Math.Sqrt(vectorMagSq);
                vector = (vector / vectorMag) * magnitude;
            }
        }


        public static bool DoesIntersect(Vector2 centre1, float radius1, Vector2 centre2, float radius2, ref IntersectInfo2D outIntersectInfo)
        {
            Vector2 dCentre = centre1 - centre2;
            float distSq = dCentre.LengthSquared();

            float resolveDist = radius1 + radius2;

            if (distSq > resolveDist * resolveDist)
                return false;

            float dist = (float)System.Math.Sqrt(distSq);

            outIntersectInfo.PositionDifference = dCentre;
            outIntersectInfo.PositionDistance = dist;
            outIntersectInfo.ResolveDistance = resolveDist;

            return true;
        }


        public static bool DoesIntersect(Vector2 centre, float radius, float radiusSq, Vector2 linePoint1, Vector2 linePoint2, Vector2 dLinePoints, float lineLengthSq, ref IntersectInfo2D outIntersectInfo)
        {
            Vector2 diffFromEnd = centre - linePoint1;
            float dot = Vector2.Dot(diffFromEnd, dLinePoints);
            float param = dot / lineLengthSq;

            Vector2 position;

            if (param < 0)
                position = linePoint1;
            else if (param > 1)
                position = linePoint2;
            else
                position = linePoint1 + (dLinePoints * param);


            Vector2 diff = position - centre;
            float penerationSqrd = diff.LengthSquared();


            // Do we have a collision?
            if (penerationSqrd > radiusSq)
                return false;


            float overlap = (float)Math.Sqrt(penerationSqrd);

            outIntersectInfo.PositionDifference = diff;
            outIntersectInfo.PositionDistance = overlap;
            outIntersectInfo.ResolveDistance = radius;

            return true;
        }


        public static bool DoesIntersect(Vector2 lineStart1, Vector2 lineStep1, Vector2 lineStart2, Vector2 lineStep2)
        {
            float div = (-lineStep2.X * lineStep1.Y) + (lineStep1.X * lineStep2.Y);
            //if (div == 0.0f)
            //    return false;

            float s = (-lineStep1.Y * (lineStart1.X - lineStart2.X) + lineStep1.X * (lineStart1.Y - lineStart2.Y)) / div;
            float t = (lineStep2.X * (lineStart1.Y - lineStart2.Y) - lineStep2.Y * (lineStart1.X - lineStart2.X)) / div;

            if (t < 0.0f || t > 1.0f || s < 0.0f || s > 1.0f)
                return false;

            return true;
        }


        public static bool DoesIntersect(Vector2 lineStart1, Vector2 lineStep1, Vector2 lineStart2, Vector2 lineStep2, ref Vector2 outIntersectPoint)
        {
            float div = (-lineStep2.X * lineStep1.Y) + (lineStep1.X * lineStep2.Y);
            //if (div == 0.0f)
            //    return false;

            float s = (-lineStep1.Y * (lineStart1.X - lineStart2.X) + lineStep1.X * (lineStart1.Y - lineStart2.Y)) / div;
            float t = (lineStep2.X * (lineStart1.Y - lineStart2.Y) - lineStep2.Y * (lineStart1.X - lineStart2.X)) / div;

            if (t < 0.0f || t > 1.0f || s < 0.0f || s > 1.0f)
                return false;

            outIntersectPoint = lineStart1 + (lineStep1 * t);
            return true;
        }


        public static bool DoesIntersectInfinite(Vector2 lineStart1, Vector2 lineStep1, Vector2 lineStart2, Vector2 lineStep2, ref Vector2 outIntersectPoint)
        {
            float div = (-lineStep2.X * lineStep1.Y) + (lineStep1.X * lineStep2.Y);
            if (div == 0.0f)
                return false;

            float s = (-lineStep1.Y * (lineStart1.X - lineStart2.X) + lineStep1.X * (lineStart1.Y - lineStart2.Y)) / div;
            float t = (lineStep2.X * (lineStart1.Y - lineStart2.Y) - lineStep2.Y * (lineStart1.X - lineStart2.X)) / div;

            outIntersectPoint = lineStart1 + (lineStep1 * t);
            return true;
        }
    }
}
