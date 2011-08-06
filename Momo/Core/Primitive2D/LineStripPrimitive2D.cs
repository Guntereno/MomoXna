using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;



namespace Momo.Core.Primitive2D
{
	public class LineStripPrimitive2D : Primitive2D
	{
		// --------------------------------------------------------------------
		// -- Private Members
		// --------------------------------------------------------------------
		private int m_pointCapacity = 0;
		private int m_pointCount = 0;
		private Vector2 m_lastPoint = Vector2.Zero;

		private LinePrimitive2D[] m_lineList = null;



		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public int PointCapacity
		{
			get { return m_pointCapacity; }
		}

		public int PointCount
		{
			get { return m_pointCount; }
		}

		public int LineCount
		{
			get { return m_pointCount - 1; }
		}   

		public LinePrimitive2D[] LineList
		{
			get { return m_lineList; }
		}



		public LineStripPrimitive2D(int pointCapacity)
		{
			m_lineList = new LinePrimitive2D[pointCapacity];

			m_pointCapacity = pointCapacity;
		}


		public void StartAddingPoints()
		{

		}


		public void AddPoint(Vector2 point)
		{
			m_lineList[m_pointCount].m_point = point;
			++m_pointCount;
		}


		public void EndAddingPoints()
		{
			RecalculateLists();
		}


		private void RecalculateLists()
		{
			m_lastPoint = m_lineList[0].m_point;

			for (int i = 1; i < m_pointCount; ++i)
			{
				Vector2 point = m_lineList[i].m_point;
				m_lineList[i - 1] = new LinePrimitive2D(m_lastPoint, point);
				m_lastPoint = point;
			}
		}


		public void CalculateBoundingArea(out Vector2 minCorner, out Vector2 maxCorner)
		{
			System.Diagnostics.Debug.Assert(m_pointCount > 0);

			float minX = m_lastPoint.X;
			float minY = m_lastPoint.Y;

			float maxX = m_lastPoint.X;
			float maxY = m_lastPoint.Y;

			for (int i = 0; i < m_pointCount; ++i)
			{
				Vector2 point = m_lineList[i].m_point;

				// Max
				if (point.X > maxX)
					maxX = point.X;
				if (point.Y > maxY)
					maxY = point.Y;

				// Min
				if (point.X < minX)
					minX = point.X;
				if (point.Y < minY)
					minY = point.Y;
			}

			minCorner.X = minX;
			minCorner.Y = minY;
			maxCorner.X = maxX;
			maxCorner.Y = maxY;
		}
	}
}
