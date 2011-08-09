using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core
{
	public struct IntersectInfo2D
	{
		private Vector2 m_dPositions;
		private float m_distance;
		private float m_resolveDistance;


		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public Vector2 PositionDifference
		{
			get { return m_dPositions; }
			set { m_dPositions = value; }
		}

		public float PositionDistance
		{
			get { return m_distance; }
			set { m_distance = value; }
		}

		public float ResolveDistance
		{
			get { return m_resolveDistance; }
			set { m_resolveDistance = value; }
		}
	}



	public class Math2D
	{
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


        public static bool DoesIntersect(Vector2 lineStart1, Vector2 lineStep1, Vector2 lineStart2, Vector2 lineStep2, ref Vector2 outIntersectPoint)
        {
            float div = (-lineStep2.X * lineStep1.Y) + (lineStep1.X * lineStep2.Y);
            float s = (-lineStep1.Y * (lineStart1.X - lineStart2.X) + lineStep1.X * (lineStart1.Y - lineStart2.Y)) / div;
            float t = (lineStep2.X * (lineStart1.Y - lineStart2.Y) - lineStep2.Y * (lineStart1.X - lineStart2.X)) / div;

            if (t < 0.0f || t > 1.0f || s < 0.0f || s > 1.0f)
                return false;

            outIntersectPoint = lineStart1 + (lineStep1 * t);
            return true;
        }
	}
}
