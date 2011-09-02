using System;

using Microsoft.Xna.Framework;


namespace Momo.Maths
{
    public class ExtendedMaths2D
    {
        // Points must be a series of connected points.
        public static void ExtrudePointsAlongNormal(Vector2[] points, int pointCnt, bool closedLoop, float amount)
        {
            if (pointCnt < 2)
                return;

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
                points[0] += offset;
                points[1] += offset;
            }
            else
            {
                for (int i = 2; i < pointCnt; ++i)
                {
                    point = points[i];
                    Vector2 edgeDirection = point - lastPoint;
                    edgeDirection.Normalize();

                    points[i - 1] = ExtrudePoint(points[i - 1], lastEdgeDirection, edgeDirection, amount);

                    lastPoint = point;
                    lastEdgeDirection = edgeDirection;
                }

                // Finally sort out the first and last point. This will depend on if its closed loop or not.
                if (closedLoop)
                {
                    points[0] = ExtrudePoint(points[0], lastEdgeDirection, firstEdgeDirection, amount);
                    points[pointCnt - 1] = points[0];
                }
                else
                {
                    points[0] += Maths2D.Perpendicular(firstEdgeDirection) * amount;
                    points[pointCnt - 1] += Maths2D.Perpendicular(lastEdgeDirection) * amount;
                }
            }

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

    }
}
