using System;

using Microsoft.Xna.Framework;


namespace Momo.Maths
{
    public class ExtendedMaths2D
    {
        // Points must be a series of connected points.
        public static bool ExtrudePointsAlongNormal(Vector2[] points, int pointCnt, bool closedLoop, float amount, out Vector2[] outExtrudedPoints)
        {
            outExtrudedPoints = new Vector2[pointCnt];

            if (pointCnt < 2)
                return false;

            Vector2 lastPoint = points[0];
            Vector2 point = points[1];
            Vector2 lastEdgeDirection = point - lastPoint;
            lastEdgeDirection.Normalize();
            lastPoint = point;

            Vector2 firstEdgeDirection = lastEdgeDirection;


            // If just two points extrude along the single lines normal.
            if (pointCnt == 2)
            {
                Vector2 offset = Maths2D.Perpendicular(lastEdgeDirection) * amount;
                outExtrudedPoints[0] = points[0] + offset;
                outExtrudedPoints[1] = points[1] + offset;
            }
            else
            {
                for (int i = 2; i < pointCnt; ++i)
                {
                    point = points[i];
                    Vector2 edgeDirection = point - lastPoint;
                    edgeDirection.Normalize();

                    outExtrudedPoints[i - 1] = ExtrudePoint(points[i - 1], lastEdgeDirection, edgeDirection, amount);

                    lastPoint = point;
                    lastEdgeDirection = edgeDirection;
                }

                // Finally sort out the first and last point. This will depend on if its closed loop or not.
                if (closedLoop)
                {
                    outExtrudedPoints[0] = ExtrudePoint(points[0], lastEdgeDirection, firstEdgeDirection, amount);
                    outExtrudedPoints[pointCnt - 1] = outExtrudedPoints[0];
                }
                else
                {
                    outExtrudedPoints[0] = points[0] + Maths2D.Perpendicular(firstEdgeDirection) * amount;
                    outExtrudedPoints[pointCnt - 1] += points[pointCnt - 1] + Maths2D.Perpendicular(lastEdgeDirection) * amount;
                }
            }

            return true;
        }


        public static Vector2 ExtrudePoint(Vector2 point, Vector2 inDirection, Vector2 outDirection, float amount)
        {
            return ExtrudePoint(point, inDirection, Maths2D.Perpendicular(inDirection), outDirection, Maths2D.Perpendicular(outDirection), amount);
        }


        public static Vector2 ExtrudePoint(Vector2 point, Vector2 inDirection, Vector2 inNormal, Vector2 outDirection, Vector2 outNormal, float amount)
        {
            Vector2 newPoint = Vector2.Zero;
            Vector2 inPoint = point + (inNormal * amount);
            Vector2 outPoint = point + (outNormal * amount);

            bool intersect = Maths2D.DoesIntersectInfinite(inPoint, inDirection, outPoint, outDirection, ref newPoint);

            // If it didnt intersect then the lines are parallel.
            if (intersect == false)
                newPoint = point + (inNormal * amount);

            return newPoint;
        }


        // Points must be a series of connected points.
        public static bool ExtrudePointsAlongNormalRounded(Vector2[] points, int pointCnt, bool closedLoop, float amount, float maxAngularStep, out Vector2[] outExtrudedPoints)
        {
            outExtrudedPoints = new Vector2[pointCnt * 5];

            if (pointCnt < 2)
                return false;


            int pCnt = 0;

            //ExtrudePointRounded(Vector2.Zero, Vector2.UnitX, -Vector2.UnitY, 1.0f, 0.4f, ref outExtrudedPoints, ref pCnt);
            //ExtrudePointRounded(Vector2.Zero, Vector2.UnitX, Vector2.UnitY, 1.0f, 0.4f, ref outExtrudedPoints, ref pCnt);
            //ExtrudePointRounded(Vector2.Zero, -Vector2.UnitX, Vector2.UnitY, 1.0f, 0.4f, ref outExtrudedPoints, ref pCnt);
            //ExtrudePointRounded(Vector2.Zero, -Vector2.UnitX, Vector2.UnitY, 1.0f, 0.4f, ref outExtrudedPoints, ref pCnt);
            //ExtrudePointRounded(Vector2.Zero, -Vector2.UnitX, -Vector2.UnitY, 1.0f, 0.4f, ref outExtrudedPoints, ref pCnt);

            Vector2 lastPoint = points[0];
            Vector2 lastEdgeDirection = points[1] - lastPoint;
            lastEdgeDirection.Normalize();
            //lastPoint = point;

            //Vector2 firstEdgeDirection = lastEdgeDirection;


            // If just two points extrude along the single lines normal.
            if (pointCnt == 2)
            {
                Vector2 offset = Maths2D.Perpendicular(lastEdgeDirection) * amount;
                outExtrudedPoints[0] = points[0] + offset;
                outExtrudedPoints[1] = points[1] + offset;
            }
            else
            {
                for (int i = 1; i < pointCnt; ++i)
                {
                    Vector2 point = points[i];
                    Vector2 edgeDirection = point - lastPoint;
                    edgeDirection.Normalize();

                    ExtrudePointRounded(point, Maths2D.Perpendicular(lastEdgeDirection), Maths2D.Perpendicular(edgeDirection), lastEdgeDirection, edgeDirection, amount, maxAngularStep, ref outExtrudedPoints, ref pCnt);

                    lastPoint = point;
                    lastEdgeDirection = edgeDirection;
                }

                // Finally sort out the first and last point. This will depend on if its closed loop or not.
                //if (closedLoop)
                //{
                //    outExtrudedPoints[0] = ExtrudePoint(points[0], lastEdgeDirection, firstEdgeDirection, amount);
                //    outExtrudedPoints[pointCnt - 1] = outExtrudedPoints[0];
                //}
                //else
                //{
                //    outExtrudedPoints[0] = points[0] + Maths2D.Perpendicular(firstEdgeDirection) * amount;
                //    outExtrudedPoints[pointCnt - 1] += points[pointCnt - 1] + Maths2D.Perpendicular(lastEdgeDirection) * amount;
                //}
            }

            Array.Copy(outExtrudedPoints, outExtrudedPoints, pCnt);

            return true;
        }


        public static void ExtrudePointRounded(Vector2 point, Vector2 outerPointNormal1, Vector2 outerPointNormal2, Vector2 outerPointDirection1, Vector2 outerPointDirection2, float amount, float maxAngularStep, ref Vector2[] outExtrudedPoints, ref int outExtrudedPointsCnt)
        {
            outExtrudedPoints[outExtrudedPointsCnt++] = point + (outerPointNormal1 * amount);
            outExtrudedPoints[outExtrudedPointsCnt++] = point + (outerPointNormal2 * amount);
            return;



            Vector2 newPoint = Vector2.Zero;
            float angle1 = (float)Math.Acos(Vector2.Dot(outerPointNormal1, Vector2.UnitX));
            float angle2 = (float)Math.Acos(Vector2.Dot(outerPointNormal2, Vector2.UnitX));

            //Console.WriteLine("-----------------------");
            //Console.WriteLine("Normal1:" + outerPointNormal1.ToString());

            //if (Vector2.Dot(outerPointNormal1, outerPointDirection2) <= 0.0f)
            //{
            //    outExtrudedPoints[outExtrudedPointsCnt++] = point;
            //    return;
            //}

            if (outerPointNormal1.Y < 0.0f)
                angle1 = (float)Math.PI + ((float)Math.PI - angle1);

            if (outerPointNormal2.Y < 0.0f)
                angle2 = (float)Math.PI + ((float)Math.PI - angle2);


            float dAngle = angle2 - angle1;

            if (dAngle > Math.PI)
                dAngle = dAngle - ((float)Math.PI * 2.0f);
            else if (dAngle < -Math.PI)
                dAngle = ((float)Math.PI * 2.0f) + dAngle;


            int steps = (int)(Math.Abs(dAngle) / maxAngularStep);
            float angleStep = dAngle / (float)steps;
            float angle = angle1;

            ++steps;

            for (int i = 0; i < steps; ++i)
            {
                Vector2 newPointOnCorner = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * amount;
                //Console.WriteLine("Point:" + newPointOnCorner.ToString());
                newPointOnCorner += point;


                outExtrudedPoints[outExtrudedPointsCnt++] = newPointOnCorner;

                angle += angleStep;
            }

            //Console.WriteLine("Normal2:" + outerPointNormal2.ToString());
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


    }
}
