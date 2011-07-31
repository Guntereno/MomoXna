using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.GameEntiies;
using Momo.Debug;



namespace Momo.Core.Spatial
{
    public class Bin
    {
        // Add sential 1 wide border


        private int m_binCountX = 0;
        private int m_binCountY = 0;
        private int m_binCount = 0;

        private Vector2 m_binDimension = Vector2.Zero;

        // Optimisation
        private Vector2 m_invBinDimension = Vector2.Zero;

        private BinItem[] m_binItems = null;
        private BinItem m_startBinItemSentital = new BinItem();
        
        private BinQueryResults m_queryResults = null;



        public Bin(int binCountX, int binCountY, float areaWidth, float areaHeight, int queryResultsCapacity)
        {
            m_binCountX = binCountX;
            m_binCountY = binCountY;
            m_binCount = m_binCountX * m_binCountY;

            m_binDimension.X = areaWidth / (float)m_binCountX;
            m_binDimension.Y = areaHeight / (float)m_binCountY;
            m_invBinDimension = Vector2.One / m_binDimension;

            m_binItems = new BinItem[m_binCount];

            m_queryResults = new BinQueryResults(queryResultsCapacity);

            Clear();
        }


        public void Clear()
        {
            for (int i = 0; i < m_binCount; ++i)
            {
                m_binItems[i] = m_startBinItemSentital;
            }
        }


        public void AddBinItem(BinItem item)
        {
            AddBinItem(item, ref item.m_binRegion);
        }


        public void AddBinItem(BinItem item, ref BinRegion region)
        {
            int binY = region.m_cornerLocation1.m_y;

            do
            {
                int binIdx = GetBinIndex(region.m_cornerLocation1.m_x, binY);

                for (int x = region.m_cornerLocation1.m_x; x < region.m_cornerLocation2.m_x; ++x)
                {
                    BinItem startBinItem = m_binItems[binIdx];
                    item.m_nextEntry = startBinItem.m_nextEntry;
                    startBinItem.m_nextEntry = item;

                    ++binIdx;
                }

                ++binY;

            } while (binY < region.m_cornerLocation2.m_y);
        }


        public void RemoveBinItem(BinItem item)
        {
            RemoveBinItem(item, ref item.m_binRegion);
        }


        public void RemoveBinItem(BinItem item, ref BinRegion region)
        {
            int binY = region.m_cornerLocation1.m_y;

            do
            {
                int binIdx = GetBinIndex(region.m_cornerLocation1.m_x, binY);

                for (int x = region.m_cornerLocation1.m_x; x < region.m_cornerLocation2.m_x; ++x)
                {
                    RemoveBinItem(item, m_binItems[binIdx]);

                    ++binIdx;
                }

                ++binY;

            } while (binY < region.m_cornerLocation2.m_y);
        }


        public void RemoveBinItem(BinItem item, BinItem startItem)
        {
            BinItem previousBinItem = startItem;
            BinItem currentBinItem = startItem.m_nextEntry;

            while (currentBinItem != null)
            {
                if (currentBinItem == item)
                {
                    previousBinItem.m_nextEntry = currentBinItem.m_nextEntry;
                    //startItem.m_prevEntry.m_nextEntry = startItem.m_nextEntry;
                    //startItem.m_nextEntry.m_prevEntry = startItem.m_prevEntry;
                    return;
                }

                previousBinItem = currentBinItem;
                currentBinItem = currentBinItem.m_nextEntry;
            }
        }


        public int CountBinItemList(BinItem startItem)
        {
            BinItem currentBinItem = startItem.m_nextEntry;
            int itemCnt = 0;


            while (currentBinItem != null)
            {
                ++itemCnt;
                currentBinItem = currentBinItem.m_nextEntry;
            }

            return itemCnt;
        }


        public void StartQuery()
        {
            m_queryResults.Clear();
        }


        // Does not properly support multiple queries between clears. Could get duplicate lists.
        public void Query(BinRegion region)
        {
            int binY = region.m_cornerLocation1.m_y;

            do
            {
                int binIdx = GetBinIndex(region.m_cornerLocation1.m_x, binY);

                for (int x = region.m_cornerLocation1.m_x; x < region.m_cornerLocation2.m_x; ++x)
                {
                    // Add the first item in the linked list to the query results (dont add sentital).
                    if (m_binItems[binIdx].m_nextEntry != null)
                    {
                        m_queryResults.AddBinItem(m_binItems[binIdx].m_nextEntry);
                    }
                    ++binIdx;
                }

                ++binY;

            } while (binY < region.m_cornerLocation2.m_y);
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


        public void GetBinArea(Vector2 centre, Vector2 dimension, ref BinRegion outBinArea)
        {
            Vector2 halfDimension = dimension * 0.5f;
            Vector2 negativeCorner = centre - halfDimension;
            Vector2 positiveCorner = centre - halfDimension;

            GetBinLocation(negativeCorner, ref outBinArea.m_cornerLocation1);
            GetBinLocation(positiveCorner, ref outBinArea.m_cornerLocation2);
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


        public void GetBinDimension(Vector2 dimension, ref BinDimension outBinDimension)
        {
            Vector2 binDimension = GetBinDimension(dimension);

            outBinDimension.m_width = (short)binDimension.X;
            outBinDimension.m_height = (short)binDimension.Y;
        }


        public Vector2 GetBinDimension(Vector2 dimension)
        {
            return dimension * m_invBinDimension;
        }


        public void GetBinDimension(BinLocation negativeCornerLocation, BinLocation positiveCornerLocation, ref BinDimension outBinDimension)
        {
            outBinDimension.m_width = (short)(negativeCornerLocation.m_x - positiveCornerLocation.m_x + 1);
            outBinDimension.m_height = (short)(negativeCornerLocation.m_y - positiveCornerLocation.m_y + 1);
        }


        public void DebugRender(DebugRenderer debugRenderer, int maxColourCount)
        {
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
                    int itemCnt = CountBinItemList(m_binItems[binIdx]);
                    float binColourMod = (float)itemCnt / (float)maxColourCount;
                    if(binColourMod > 1.0f)
                        binColourMod = 1.0f;

                    Color binColour = new Color(binColourMod * 2.0f, 1.0f, 0.0f, 0.25f);

                    if( binColourMod > 0.5f)
                    {
                        binColourMod = (binColourMod - 0.5f) / 0.5f;
                        binColour = new Color(1.0f, 1.0f - binColourMod, 0.0f, 0.25f);

                    }


                    debugRenderer.DrawQuad(p1, p2, p3, p4, binColour, new Color(0.0f, 0.0f, 0.0f, 0.5f), true, 3.0f);


                    p1 = p2;
                    p4 = p3;

                    p2.X = (float)(x + 1) * m_binDimension.X;
                    p3.X = p2.X;
                }

                binIdx = m_binCountX * y;
            }
        }
    }
}
