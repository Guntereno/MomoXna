using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.GameEntities;
using Momo.Debug;



namespace Momo.Core.Spatial
{
	public class Bin
	{
		// Add sential 1 wide border


		private int m_binCountX = 0;
		private int m_binCountY = 0;
		private int m_binCount = 0;

		private int m_binEntryPoolCapacity = 0;
		private int m_binEntryPoolFreeCount = 0;

		private Vector2 m_binDimension = Vector2.Zero;
		private Vector2 m_invBinDimension = Vector2.Zero;

		private BinEntry[] m_binEntryPool = null;
		private BinEntry[] m_binEntries = null;

		private BinQueryResults m_queryResults = null;



		public Bin(int binCountX, int binCountY, float areaWidth, float areaHeight, int itemCapacity, int queryResultsCapacity)
		{
			m_binCountX = binCountX;
			m_binCountY = binCountY;
			m_binCount = m_binCountX * m_binCountY;

			m_binEntryPoolCapacity = itemCapacity;
			m_binEntryPoolFreeCount = itemCapacity;

			m_binDimension.X = areaWidth / (float)m_binCountX;
			m_binDimension.Y = areaHeight / (float)m_binCountY;
			m_invBinDimension = Vector2.One / m_binDimension;

			m_queryResults = new BinQueryResults(queryResultsCapacity);


			m_binEntryPool = new BinEntry[itemCapacity];
			m_binEntries = new BinEntry[m_binCount];

			// Fill pool
			for (int i = 0; i < itemCapacity; ++i)
			{
				m_binEntryPool[i] = new BinEntry();
			}

			// Add sentitals
			for (int i = 0; i < m_binCount; ++i)
			{
				m_binEntries[i] = new BinEntry();
			}


			Clear();
		}


		public void Clear()
		{
			m_binEntryPoolFreeCount = m_binEntryPoolCapacity;

			for (int i = 0; i < m_binCount; ++i)
			{
				m_binEntries[i].m_nextEntry = null;
			}
		}


		public void AddBinItem(BinItem item)
		{
			AddBinItem(item, ref item.m_region);
		}


		public void AddBinItem(BinItem item, ref BinRegionUniform region)
		{
			int y = region.m_minLocation.m_y;

			do
			{
				int binIdx = GetBinIndex(region.m_minLocation.m_x, y);

				for (int x = region.m_minLocation.m_x; x <= region.m_maxLocation.m_x; ++x)
				{
					BinEntry startBinEntry = m_binEntries[binIdx];
					BinEntry freeBinEntry = GetFreeBinEntry();
					freeBinEntry.m_item = item;
					freeBinEntry.m_nextEntry = startBinEntry.m_nextEntry;
					startBinEntry.m_nextEntry = freeBinEntry;

					++binIdx;
				}

				++y;

			} while (y <= region.m_maxLocation.m_y);
		}


		public void RemoveBinItem(BinItem item)
		{
			RemoveBinItem(item, ref item.m_region);
		}


		public void RemoveBinItem(BinItem item, ref BinRegionUniform region)
		{
			int y = region.m_minLocation.m_y;

			do
			{
				int binIdx = GetBinIndex(region.m_minLocation.m_x, y);

				for (int x = region.m_minLocation.m_x; x <= region.m_maxLocation.m_x; ++x)
				{
					RemoveBinItem(item, m_binEntries[binIdx]);

					++binIdx;
				}

				++y;

			} while (y <= region.m_maxLocation.m_y);
		}


		public void RemoveBinItem(BinItem item, BinEntry startEntry)
		{
			BinEntry previousBinEntry = startEntry;
			BinEntry currentBinEntry = startEntry.m_nextEntry;

			while (currentBinEntry != null)
			{
				if (currentBinEntry.m_item == item)
				{
					previousBinEntry.m_nextEntry = currentBinEntry.m_nextEntry;
					m_binEntryPool[m_binEntryPoolFreeCount] = currentBinEntry;
					++m_binEntryPoolFreeCount;
					return;
				}

				previousBinEntry = currentBinEntry;
				currentBinEntry = currentBinEntry.m_nextEntry;
			}
		}


		public void UpdateBinItemRegion(BinItem item, ref BinRegionUniform prevRegion, ref BinRegionUniform newRegion)
		{
			// Skip update if they are the same.
			if (prevRegion.IsEqual(ref newRegion))
				return;


			RemoveBinItem(item, ref prevRegion);
			AddBinItem(item, ref newRegion);

			item.m_region = newRegion;
		}


		public void UpdateBinItemRegion(BinItem item, ref BinRegionUniform newRegion)
		{
			AddBinItem(item, ref newRegion);

			item.m_region = newRegion;
		}


		public int CountBinList(BinEntry startEntry)
		{
			int itemCnt = 0;
			BinEntry currentBinEntry = startEntry.m_nextEntry;

			while (currentBinEntry != null)
			{
				++itemCnt;
				currentBinEntry = currentBinEntry.m_nextEntry;
			}

			return itemCnt;
		}


		public void StartQuery()
		{
			m_queryResults.Clear();
		}


		public void Query(BinRegionUniform region)
		{
			int itemCnt = m_queryResults.BinItemCount;

			int y = region.m_minLocation.m_y;

			do
			{
				int binIdx = GetBinIndex(region.m_minLocation.m_x, y);

				for (int x = region.m_minLocation.m_x; x <= region.m_maxLocation.m_x; ++x)
				{
					int lastBinCnt = itemCnt;

					// Dont add sentital.
					BinEntry entry = m_binEntries[binIdx].m_nextEntry;
					while (entry != null)
					{
						itemCnt = m_queryResults.AddBinItem(entry.m_item, lastBinCnt);

						entry = entry.m_nextEntry;
					}

					++binIdx;
				}

				++y;

			} while (y <= region.m_maxLocation.m_y);
		}


		public BinQueryResults EndQuery()
		{
			return m_queryResults;
		}


		public int GetBinIndex(ref BinLocation binLocation)
		{
			return (binLocation.m_y * m_binCountX) + binLocation.m_x;
		}


		public int GetBinIndex(int binLocationX, int binLocationY)
		{
			return (binLocationY * m_binCountX) + binLocationX;
		}


		public void GetBinRegionCorners(Vector2 minCorner, Vector2 maxCorner, ref BinRegionUniform outBinRegion)
		{
			GetBinLocation(minCorner, ref outBinRegion.m_minLocation);
			GetBinLocation(maxCorner, ref outBinRegion.m_maxLocation);
		}


		public void GetBinRegionFromCentre(Vector2 centre, Vector2 halfDimension, ref BinRegionUniform outBinRegion)
		{
			Vector2 minCorner = centre - halfDimension;
			Vector2 maxCorner = centre + halfDimension;

			GetBinRegionCorners(minCorner, maxCorner, ref outBinRegion);
		}


		public void GetBinRegionFromCentre(Vector2 centre, float radius, ref BinRegionUniform outBinRegion)
		{
			Vector2 halfDimension = new Vector2(radius, radius);
			GetBinRegionFromCentre(centre, halfDimension, ref outBinRegion);
		}


		public void GetBinLocation(Vector2 position, ref BinLocation outBinLocation)
		{
			Vector2 binPosition = GetBinLocation(position);

			outBinLocation.m_x = (short)binPosition.X;
			outBinLocation.m_y = (short)binPosition.Y;
		}


		public Vector2 GetBinLocation(Vector2 position)
		{
			return position * m_invBinDimension;
		}


		//public void GetBinDimension(Vector2 dimension, ref BinDimension outBinDimension)
		//{
		//	Vector2 binDimension = GetBinDimension(dimension);

		//	outBinDimension.m_width = (short)binDimension.X;
		//	outBinDimension.m_height = (short)binDimension.Y;
		//}


		//public Vector2 GetBinDimension(Vector2 dimension)
		//{
		//	return dimension * m_invBinDimension;
		//}


		//public void GetBinDimension(BinLocation negativeCornerLocation, BinLocation positiveCornerLocation, ref BinDimension outBinDimension)
		//{
		//	outBinDimension.m_width = (short)(negativeCornerLocation.m_x - positiveCornerLocation.m_x + 1);
		//	outBinDimension.m_height = (short)(negativeCornerLocation.m_y - positiveCornerLocation.m_y + 1);
		//}


		public void DebugRender(DebugRenderer debugRenderer, int maxColourCount)
		{
			float binAlpha = 0.5f;
			int binIdx = 0;


			for (int y = 0; y < m_binCountY; ++y)
			{
				Vector2 p1 = new Vector2(0.0f, (float)y * m_binDimension.Y);
				Vector2 p2 = p1;
				p2.X += m_binDimension.X;
				Vector2 p3 = p1 + m_binDimension;
				Vector2 p4 = p1;
				p4.Y += m_binDimension.Y;


				for (int x = 0; x < m_binCountX; ++x)
				{
					int entryCnt = CountBinList(m_binEntries[binIdx]);

					float binColourMod = (float)entryCnt / (float)maxColourCount;
					if(binColourMod > 1.0f)
						binColourMod = 1.0f;

					Color binColour = new Color(binColourMod * 2.0f, 1.0f, 0.0f, binAlpha);

					if( binColourMod > 0.5f)
					{
						binColourMod = (binColourMod - 0.5f) / 0.5f;
						binColour = new Color(1.0f, 1.0f - binColourMod, 0.0f, binAlpha);
					}


					debugRenderer.DrawQuad(p1, p2, p3, p4, binColour, new Color(0.0f, 0.0f, 0.0f, binAlpha), true, 0.0f);


					p1 = p2;
					p4 = p3;

					p2.X = (float)(x + 2) * m_binDimension.X;
					p3.X = p2.X;

					++binIdx;
				}

				binIdx = m_binCountX * (y + 1);
			}
		}


		private BinEntry GetFreeBinEntry()
		{
			--m_binEntryPoolFreeCount;

			return m_binEntryPool[m_binEntryPoolFreeCount];
		}
	}
}
