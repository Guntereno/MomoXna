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
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private int m_binCountX = 0;
        private int m_binCountY = 0;
        private int m_binCountMinusOneX = 0;
        private int m_binCountMinusOneY = 0;

        private int m_binCount = 0;
        private int m_binLayerCount = 0;

        private int m_binEntryPoolCapacity = 0;
        private int m_binEntryPoolFreeCount = 0;


        private Vector2 m_areaDimension = Vector2.Zero;

        private Vector2 m_binDimension = Vector2.Zero;
        private Vector2 m_halfBinDimension = Vector2.Zero;
        private Vector2 m_invBinDimension = Vector2.Zero;

        private BinEntry[] m_binEntryPool = null;
        private SentinelBinEntry[][] m_binEntries = null;

        private BinQueryResults m_sharedQueryResults = null;
        private BinQueryLocalityResults m_sharedQueryLocalityResults = null;



        public int BinCountX
        {
            get { return m_binCountX; }
        }

        public int BinCountY
        {
            get { return m_binCountY; }
        }

        public int BinCount
        {
            get { return m_binCount; }
        }

        public Vector2 BinDimension
        {
            get { return m_binDimension; }
        }



        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public Bin()
        {

        }


        public void Init(int binCountX, int binCountY, Vector2 area, int layerCount, int itemCapacity, int sharedQueryResultsCapacity, int regionSelectionCapacity)
        {
            m_binCountX = binCountX;
            m_binCountY = binCountY;
            m_binCountMinusOneX = m_binCountX - 1;
            m_binCountMinusOneY = m_binCountY - 1;

            m_binCount = m_binCountX * m_binCountY;
            m_binLayerCount = layerCount;

            m_binEntryPoolCapacity = itemCapacity;
            m_binEntryPoolFreeCount = itemCapacity;

            m_binDimension.X = area.X / (float)binCountX;
            m_binDimension.Y = area.Y / (float)binCountY;
            m_halfBinDimension = m_binDimension * 0.5f;
            m_invBinDimension = Vector2.One / m_binDimension;

            m_areaDimension = area;

            m_sharedQueryResults = new BinQueryResults(sharedQueryResultsCapacity);
            m_sharedQueryLocalityResults = new BinQueryLocalityResults(sharedQueryResultsCapacity);

            m_binEntryPool = new BinEntry[itemCapacity];
            m_binEntries = new SentinelBinEntry[layerCount][];


            // Fill pool
            for (int i = 0; i < itemCapacity; ++i)
            {
                m_binEntryPool[i] = new BinEntry();
            }

            // Add sentitals
            for (int i = 0; i < m_binLayerCount; ++i)
            {
                m_binEntries[i] = new SentinelBinEntry[m_binCount];
                for (int j = 0; j < m_binCount; ++j)
                {
                    m_binEntries[i][j] = new SentinelBinEntry();
                }
            }


            Clear();
        }


        public void Clear()
        {
            m_binEntryPoolFreeCount = m_binEntryPoolCapacity;

            for (int i = 0; i < m_binLayerCount; ++i)
            {
                for (int j = 0; j < m_binCount; ++j)
                {
                    m_binEntries[i][j].m_nextEntry = null;
                }
            }
        }


        public BinQueryResults GetSharedQueryResults()
        {
            return m_sharedQueryResults;
        }


        public BinQueryLocalityResults GetSharedQueryLocalityResults()
        {
            return m_sharedQueryLocalityResults;
        }


        public void AddBinItem(BinItem item, int layerIdx)
        {
            AddBinItem(item, ref item.mRegion, layerIdx);
        }


        public void AddBinItem(BinItem item, Vector2 corner1, Vector2 corner2, int layerIdx)
        {
            BinRegionUniform region = new BinRegionUniform();
            GetBinRegionFromUnsortedCorners(corner1, corner2, ref region);
            item.SetBinRegion(region);

            AddBinItem(item, ref region, layerIdx);
        }


        public void AddBinItem(BinItem item, Vector2 centre, float radius, int layerIdx)
        {
            BinRegionUniform region = new BinRegionUniform();
            GetBinRegionFromCentre(centre, radius, ref region);
            item.SetBinRegion(region);

            AddBinItem(item, ref region, layerIdx);
        }


        public void AddBinItem(BinItem item, ref BinRegionUniform region, int layerIdx)
        {
            int y = region.m_minLocation.m_y;

            do
            {
                int binIdx = GetBinIndex(region.m_minLocation.m_x, y);

                for (int x = region.m_minLocation.m_x; x <= region.m_maxLocation.m_x; ++x)
                {
                    SentinelBinEntry startBinEntry = m_binEntries[layerIdx][binIdx];
                    BinEntry freeBinEntry = GetFreeBinEntry();
                    freeBinEntry.m_item = item;
                    freeBinEntry.m_nextEntry = startBinEntry.m_nextEntry;
                    startBinEntry.m_nextEntry = freeBinEntry;

                    ++binIdx;
                }

                ++y;

            } while (y <= region.m_maxLocation.m_y);
        }


        public void AddBinItem(BinItem item, ref BinRegionSelection region, int layerIdx)
        {
            for (int i = 0; i < region.m_binIndices.Length; ++i)
            {
                SentinelBinEntry startBinEntry = m_binEntries[layerIdx][region.m_binIndices[i].m_index];
                BinEntry freeBinEntry = GetFreeBinEntry();
                freeBinEntry.m_item = item;
                freeBinEntry.m_nextEntry = startBinEntry.m_nextEntry;
                startBinEntry.m_nextEntry = freeBinEntry;
            }
        }


        public void RemoveBinItem(BinItem item, int layerIdx)
        {
            RemoveBinItem(item, ref item.mRegion, layerIdx);
        }


        public void RemoveBinItem(BinItem item, ref BinRegionUniform region, int layerIdx)
        {
            int y = region.m_minLocation.m_y;

            do
            {
                int binIdx = GetBinIndex(region.m_minLocation.m_x, y);

                for (int x = region.m_minLocation.m_x; x <= region.m_maxLocation.m_x; ++x)
                {
                    RemoveBinItem(item, m_binEntries[layerIdx][binIdx]);

                    ++binIdx;
                }

                ++y;

            } while (y <= region.m_maxLocation.m_y);
        }


        public void RemoveBinItem(BinItem item, ref BinRegionSelection region, int layerIdx)
        {
            for (int i = 0; i < region.m_binIndices.Length; ++i)
            {
                RemoveBinItem(item, m_binEntries[layerIdx][region.m_binIndices[i].m_index]);
            }
        }


        public void RemoveBinItem(BinItem item, SentinelBinEntry startEntry)
        {
            SentinelBinEntry previousBinEntry = startEntry;
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


        public void UpdateBinItem(BinItem item, ref BinRegionUniform prevRegion, ref BinRegionUniform newRegion, int layerIdx)
        {
            // Skip update if they are the same.
            if (prevRegion.IsEqual(ref newRegion))
                return;

            RemoveBinItem(item, ref prevRegion, layerIdx);
            AddBinItem(item, ref newRegion, layerIdx);
        }


        public void UpdateBinItem(BinItem item, ref BinRegionUniform newRegion, int layerIdx)
        {
            AddBinItem(item, ref newRegion, layerIdx);
        }


        public void UpdateBinItem(BinItem item, ref BinRegionSelection newRegion, int layerIdx)
        {
            AddBinItem(item, ref newRegion, layerIdx);
        }


        public void UpdateBinItem(BinItem item, ref BinRegionSelection prevRegion, ref BinRegionSelection newRegion, int layerIdx)
        {
            RemoveBinItem(item, ref prevRegion, layerIdx);
            AddBinItem(item, ref newRegion, layerIdx);
        }


        public int CountBinList(SentinelBinEntry startEntry)
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


        public void Query(ref BinRegionUniform region, int layerIdx, BinQueryResults results)
        {
            int itemCnt = results.BinItemCount;

            int y = region.m_minLocation.m_y;

            do
            {
                int binIdx = GetBinIndex(region.m_minLocation.m_x, y);

                for (int x = region.m_minLocation.m_x; x <= region.m_maxLocation.m_x; ++x)
                {
                    int lastBinCnt = itemCnt;

                    // Dont add sentital.
                    BinEntry entry = m_binEntries[layerIdx][binIdx].m_nextEntry;
                    while (entry != null)
                    {
                        itemCnt = results.AddBinItem(entry.m_item, lastBinCnt);
                        entry = entry.m_nextEntry;
                    }

                    ++binIdx;
                }

                ++y;

            } while (y <= region.m_maxLocation.m_y);
        }


        public void Query(ref BinRegionSelection region, int layerIdx, BinQueryResults results)
        {
            for (int i = 0; i < region.m_binCnt; ++i)
            {
                int lastBinCnt = results.BinItemCount;

                // Dont add sentital.
                BinEntry entry = m_binEntries[layerIdx][region.m_binIndices[i].m_index].m_nextEntry;
                while (entry != null)
                {
                    results.AddBinItem(entry.m_item, lastBinCnt);
                    entry = entry.m_nextEntry;
                }
            }
        }


        public void Query(ref BinRegionSelection region, int binIndexOffset, int layerIdx, BinQueryResults results)
        {
            for (int i = 0; i < region.m_binCnt; ++i)
            {
                int lastBinCnt = results.BinItemCount;

                // Dont add sentital.
                BinEntry entry = m_binEntries[layerIdx][binIndexOffset + region.m_binIndices[i].m_index].m_nextEntry;
                while (entry != null)
                {
                    results.AddBinItem(entry.m_item, lastBinCnt);
                    entry = entry.m_nextEntry;
                }
            }
        }


        public int GetBinIndex(ref BinLocation binLocation)
        {
            return (binLocation.m_y * m_binCountX) + binLocation.m_x;
        }


        public int GetBinIndex(int binLocationX, int binLocationY)
        {
            return ((binLocationY * m_binCountX) + binLocationX);
        }


        public int GetBinIndex(Vector2 binLocation)
        {
            Vector2 binSpacePos = GetBinLocation(binLocation);
            return (((int)binSpacePos.Y * m_binCountX) + (int)binSpacePos.X);
        }


        public void GetBinRegionFromCorners(Vector2 minCorner, Vector2 maxCorner, ref BinRegionUniform outBinRegion)
        {
            GetBinLocation(minCorner, ref outBinRegion.m_minLocation);
            GetBinLocation(maxCorner, ref outBinRegion.m_maxLocation);
        }


        public void GetBinRegionFromUnsortedCorners(Vector2 corner1, Vector2 corner2, ref BinRegionUniform outBinRegion)
        {
            Vector2 min = corner1;
            Vector2 max = corner2;

            if (min.X > max.X)
            {
                min.X = corner2.X;
                max.X = corner1.X;
            }

            if (min.Y > max.Y)
            {
                min.Y = corner2.Y;
                max.Y = corner1.Y;
            }

            GetBinLocation(min, ref outBinRegion.m_minLocation);
            GetBinLocation(max, ref outBinRegion.m_maxLocation);
        }


        public void GetBinRegionFromCentre(Vector2 centre, Vector2 halfDimension, ref BinRegionUniform outBinRegion)
        {
            Vector2 minCorner = centre - halfDimension;
            Vector2 maxCorner = centre + halfDimension;

            GetBinRegionFromCorners(minCorner, maxCorner, ref outBinRegion);
        }


        public void GetBinRegionFromCentre(Vector2 centre, float radius, ref BinRegionUniform outBinRegion)
        {
            Vector2 halfDimension = new Vector2(radius, radius);
            GetBinRegionFromCentre(centre, halfDimension, ref outBinRegion);
        }


        public void GetBinRegionFromLine(Vector2 p1, Vector2 diff, ref BinRegionSelection outBinRegion)
        {
            int binCnt = 0;

            BinLocation binLocation = new BinLocation();
            GetBinLocation(p1, ref binLocation);

            Vector2 p1BinSpace = p1 / m_binDimension;
            Vector2 diffBinSpace = diff / m_binDimension;

            int binMoveX = 1;
            int binMoveY = 1;
            int binBaseX = 1;
            int binBaseY = 1;

            if (diffBinSpace.X < 0.0f)
            {
                binMoveX = -1;
                binBaseX = 0;
            }

            if (diffBinSpace.Y < 0.0f)
            {
                binMoveY = -1;
                binBaseY = 0;
            }


            outBinRegion.m_binIndices[binCnt++].m_index = GetBinIndex(ref binLocation);


            // The one will have to change based on -/+ direction of y.
            float tX = float.MaxValue;
            float tY = float.MaxValue;

            // Stop divide by 0.
            if (diffBinSpace.X != 0.0f)
                tX = ((float)(binLocation.m_x + binBaseX) - p1BinSpace.X) / diffBinSpace.X;

            if (diffBinSpace.Y != 0.0f)
                tY = ((float)(binLocation.m_y + binBaseY) - p1BinSpace.Y) / diffBinSpace.Y;


            while (tX < 1.0f || tY < 1.0f)
            {
                // Y boundary hit first (up or down it goes).
                if (tY < tX)
                {
                    binLocation.m_y += binMoveY;
                    outBinRegion.m_binIndices[binCnt++].m_index = GetBinIndex(ref binLocation);

                    tY = ((float)(binLocation.m_y + binBaseY) - p1BinSpace.Y) / diffBinSpace.Y;
                }
                else
                {
                    binLocation.m_x += binMoveX;
                    outBinRegion.m_binIndices[binCnt++].m_index = GetBinIndex(ref binLocation);

                    tX = ((float)(binLocation.m_x + binBaseX) - p1BinSpace.X) / diffBinSpace.X;
                }
            }

            outBinRegion.m_binCnt = binCnt;
        }


        // Relatively slow. Use at load time.
        public void GetBinRegionFromCircle(BinLocation centre, float radius, float resolution, ref BinRegionSelection[] minusRegion, int minusRegionCnt, ref BinRegionSelection outBinRegion)
        {
            const float kHalfPI = (float)Math.PI * 0.5f;

            BinLocation lastLocation = BinLocation.kInvalidBinLocation;
            BinLocation location = BinLocation.kInvalidBinLocation;

            int centreBinIndex = GetBinIndex(ref centre);

            // Calculate around the top left corner of the bin (this is often negative due to the dead zone).
            Vector2 edgePosition = m_halfBinDimension + new Vector2(0.0f, radius);
            GetBinLocation(edgePosition, ref location);
            lastLocation = location;


            float angle = 0.0f;
            bool endLoop = false;

            do
            {
                angle += resolution;

                edgePosition = m_halfBinDimension + new Vector2((float)Math.Sin(angle) * radius, (float)Math.Cos(angle) * radius);
                GetBinLocation(edgePosition, ref location);


                if (angle > kHalfPI)
                {
                    GetBinLocation(m_halfBinDimension + new Vector2(radius, 0.0f), ref lastLocation);
                    endLoop = true;
                }


                if (location.m_y != lastLocation.m_y || endLoop)
                {
                    int binIndex = centreBinIndex + GetBinIndex(ref lastLocation);
                    int binIndexYOffset = m_binCountX * (lastLocation.Y * 2);

                    binIndex -= lastLocation.X * 2;

                    for (int x = 0; x <= lastLocation.X * 2; ++x)
                    {
                        bool addBin = true;;
                        for (int i = 0; i < minusRegionCnt; ++i)
                        {
                            if (minusRegion[i].HasBinIndex(binIndex))
                            {
                                addBin = false;
                                break;
                            }
                        }

                        if (addBin)
                        {
                            outBinRegion.AddBinIndex(binIndex);
                            if (lastLocation.Y > 0)
                                outBinRegion.AddBinIndex(binIndex - binIndexYOffset);
                        }

                        ++binIndex;
                    }
                }


                lastLocation = location;
            } while (!endLoop);
        }


        public void GetBinLocation(Vector2 position, ref BinLocation outBinLocation)
        {
            Vector2 binPosition = GetBinLocation(position);

            outBinLocation.m_x = (int)binPosition.X;
            outBinLocation.m_y = (int)binPosition.Y;
        }


        public Vector2 GetBinLocation(Vector2 position)
        {
            return position * m_invBinDimension;
        }


        public Vector2 GeTopLeftPositionOfBin(BinLocation location)
        {
            return new Vector2(location.m_x * m_binDimension.X, location.m_y * m_binDimension.Y);
        }


        public Vector2 GetCentrePositionOfBin(BinLocation location)
        {
            return GeTopLeftPositionOfBin(location) + m_halfBinDimension;
        }


        public void DebugRender(DebugRenderer debugRenderer, int maxColourCount, int layerIdx)
        {
            const float kBinAlpha = 0.25f;
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
                    int entryCnt = CountBinList(m_binEntries[layerIdx][binIdx]);
                    

                    if (entryCnt > 0)
                    {
                        float binColourMod = (float)entryCnt / (float)maxColourCount;
                        if (binColourMod > 1.0f)
                            binColourMod = 1.0f;


                        Color binColour = new Color(binColourMod, 1.0f, 0.0f, kBinAlpha * binColourMod);

                        debugRenderer.DrawFilledRect(p1, p3, binColour);
                    }

                    p1 = p2;
                    p4 = p3;

                    p2.X = ((float)(x + 2) * m_binDimension.X);
                    p3.X = p2.X;

                    ++binIdx;
                }

                binIdx = m_binCountX * (y + 1);
            }
        }


        public void DebugRender(DebugRenderer debugRenderer, BinRegionSelection region, Color regionColour)
        {
            for (int i = 0; i < region.m_binCnt; ++i)
            {
                int binIdx = region.m_binIndices[i].m_index;
                int x = binIdx % m_binCountX;
                int y = binIdx / m_binCountX;

                Vector2 p1 = new Vector2((float)x * m_binDimension.X, (float)y * m_binDimension.Y);
                Vector2 p2 = p1;
                p2.X += m_binDimension.X;
                Vector2 p3 = p1 + m_binDimension;
                Vector2 p4 = p1;
                p4.Y += m_binDimension.Y;

                debugRenderer.DrawFilledRect(p1, p3, regionColour);
            }
        }


        public void DebugRenderGrid(DebugRenderer debugRenderer, Color gridColour, Color deadZoneColour)
        {
            for (int x = 0; x <= m_binCountX; ++x)
            {
                Vector2 p1 = new Vector2((float)x * m_binDimension.X, 0.0f);
                Vector2 p2 = new Vector2((float)x * m_binDimension.X, m_areaDimension.Y);

                debugRenderer.DrawLine(p1, p2, gridColour);
            }

            for (int y = 0; y <= m_binCountY; ++y)
            {
                Vector2 p1 = new Vector2(0.0f, (float)y * m_binDimension.Y);
                Vector2 p2 = new Vector2(m_areaDimension.X, (float)y * m_binDimension.Y);

                debugRenderer.DrawLine(p1, p2, gridColour);
            }

        }


        private BinEntry GetFreeBinEntry()
        {
            --m_binEntryPoolFreeCount;

            return m_binEntryPool[m_binEntryPoolFreeCount];
        }
    }
}
