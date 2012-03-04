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



        public static float ProjectPointOntoInfiniteLine(Vector2 point, Vector2 lineStart, Vector2 lineStep)
        {
            return Vector2.Dot(point - lineStart, lineStep) / Vector2.Dot(lineStep, lineStep);
        }


        public static bool ProjectPointOntoLine(Vector2 point, Vector2 lineStart, Vector2 lineStep, ref Vector2 pointOnLine)
        {
            float wLine = Vector2.Dot(point - lineStart, lineStep) / Vector2.Dot(lineStep, lineStep);
            pointOnLine = lineStart + (lineStep * wLine);

            return (wLine >= 0.0f && wLine <= 1.0f);
        }


        public static Vector2 NearestPointOntoLine(Vector2 point, Vector2 lineStart, Vector2 lineStep)
        {
            float wLine = Vector2.Dot(point - lineStart, lineStep) / Vector2.Dot(lineStep, lineStep);

            if (wLine < 0.0f)
            {
                return lineStart;
            }
            else if (wLine > 1.0f)
            {
                return lineStart + lineStep;
            }

            return lineStart + (lineStep * wLine);
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


        public static bool DoesIntersect(Vector2 centre, float radius, float radiusSq, Vector2 linePoint1, Vector2 linePoint2, Vector2 dLinePoints, float lineLengthSq, ref float outIntersectDelta)
        {
            //Vector2 diffFromEnd = linePoint1 - centre;
            Vector2 diffFromEnd = linePoint2 - centre;

            float a = Vector2.Dot(dLinePoints, dLinePoints);
            float b = 2.0f * Vector2.Dot(diffFromEnd, dLinePoints);
            float c = Vector2.Dot(diffFromEnd, diffFromEnd) - radiusSq;

            float discriminant = (b * b) - (4.0f * a * c);
            if (discriminant < 0.0f)
                return false;


            // Ray doesnt miss sphere
            discriminant = (float)Math.Sqrt(discriminant);
            float t1 = (-b + discriminant) / (2.0f * a);
            float t2 = (-b - discriminant) / (2.0f * a);

            if (t1 >= 0.0f && t1 <= 1.0f)
            {
                // Solution on is on the ray
                outIntersectDelta = t1;
                return true;
            }

            // t2 would be the second contact point on the circle
            return false;
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


        public static bool DoesIntersect(Vector2 lineStart1, Vector2 lineStep1, Vector2 lineStart2, Vector2 lineStep2, ref float outIntersectDelta)
        {
            float div = (-lineStep2.X * lineStep1.Y) + (lineStep1.X * lineStep2.Y);
            //if (div == 0.0f)
            //    return false;

            float s = (-lineStep1.Y * (lineStart1.X - lineStart2.X) + lineStep1.X * (lineStart1.Y - lineStart2.Y)) / div;
            float t = (lineStep2.X * (lineStart1.Y - lineStart2.Y) - lineStep2.Y * (lineStart1.X - lineStart2.X)) / div;

            if (t < 0.0f || t > 1.0f || s < 0.0f || s > 1.0f)
                return false;

            outIntersectDelta = t;
            return true;
        }



        //public static bool DoesIntersect(Vector2 lineStart1, Vector2 lineStep1, float lineWidthSq1, Vector2 lineStart2, Vector2 lineStep2)
        //{
        //    float wLineP1 = Maths2D.ProjectPointOntoInfiniteLine(lineStart2, lineStart1, lineStep1);
        //    float wLineP2 = Maths2D.ProjectPointOntoInfiniteLine(lineStart2 + lineStep2, lineStart1, lineStep1);

        //    if ((wLineP1 < 0.0f && wLineP2 < 0.0f) || (wLineP1 > 1.0f && wLineP2 > 1.0f))
        //        return false;

        //    float wGap = Math.Abs(wLineP2 - wLineP1);
        //    float wLineP1_2 = 0.0f;
        //    float wLineP2_2 = 1.0f;

        //    if (wLineP1 < 0.0f)
        //        wLineP1_2 = -wLineP1 / wGap;
        //    else if (wLineP1 > 1.0f)
        //        wLineP1_2 = (wLineP1 - 1.0f) / wGap;

        //    if (wLineP2 < 0.0f)
        //        wLineP2_2 = wLineP1 / wGap;
        //    else if (wLineP2 > 1.0f)
        //        wLineP2_2 = (1.0f - wLineP1) / wGap;


        //    Vector2 lineP1_2 = lineStart2 + (lineStep2 * wLineP1_2);
        //    Vector2 lineP2_2 = lineStart2 + (lineStep2 * wLineP2_2);

        //    if (wLineP1 > 1.0f)
        //        wLineP1 = 1.0f;
        //    else if (wLineP1 < 0.0f)
        //        wLineP1 = 0.0f;

        //    if (wLineP2 > 1.0f)
        //        wLineP2 = 1.0f;
        //    else if (wLineP2 < 0.0f)
        //        wLineP2 = 0.0f;


        //    Vector2 lineP1 = lineStart1 + (lineStep1 * wLineP1);
        //    Vector2 lineP2 = lineStart1 + (lineStep1 * wLineP2);

        //    Vector2 dLinePos1 = lineP1 - lineP1_2;
        //    Vector2 dLinePos2 = lineP2 - lineP2_2;


        //    if (Vector2.Dot(dLinePos1, dLinePos2) < 0.0f)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        if (dLinePos1.LengthSquared() < lineWidthSq1 || dLinePos2.LengthSquared() < lineWidthSq1)
        //            return true;
        //    }

        //    return false;
        //}


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


        public static bool DoesIntersectInfinite(Vector2 lineStart1, Vector2 lineStep1, Vector2 lineStart2, Vector2 lineStep2, ref float outIntersectDelta)
        {
            float div = (-lineStep2.X * lineStep1.Y) + (lineStep1.X * lineStep2.Y);
            if (div == 0.0f)
                return false;

            float s = (-lineStep1.Y * (lineStart1.X - lineStart2.X) + lineStep1.X * (lineStart1.Y - lineStart2.Y)) / div;
            float t = (lineStep2.X * (lineStart1.Y - lineStart2.Y) - lineStep2.Y * (lineStart1.X - lineStart2.X)) / div;

            outIntersectDelta = t;
            return true;
        }
    }
}
