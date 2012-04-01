using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Momo.Core.GameEntities;
using Momo.Debug;



namespace Momo.Core.Spatial
{
    public class BinTimeStamps
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private Bin m_bin = null;
        private ulong[] m_timeStampList = null;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public BinTimeStamps()
        {

        }


        public void Init(Bin bin)
        {
            m_bin = bin;
            m_timeStampList = new ulong[bin.BinCount];
            for (int i = 0; i < bin.BinCount; ++i)
            {
                m_timeStampList[i] = ulong.MaxValue;
            }
        }


        public void UpdateHeatMap(ref BinRegionUniform binRegion, ulong timeStamp)
        {
            // TODO: Don't use GetBinIndex. You can more efficiently calculate the bin index yourself.

            for (int y = binRegion.m_minLocation.m_y; y <= binRegion.m_maxLocation.m_y; ++y)
            {
                for (int x = binRegion.m_minLocation.m_x; x <= binRegion.m_maxLocation.m_x; ++x)
                {
                    int binIndex = m_bin.GetBinIndex(x, y);
                    m_timeStampList[binIndex] = timeStamp;
                }
            }
        }


        public ulong Query(int binIndex)
        {
            return m_timeStampList[binIndex];
        }


        public void DebugRender(DebugRenderer debugRenderer, ulong currentTimeStamp, ulong maxHeatTime)
        {
            const float kBinAlpha = 0.50f;
            int binIdx = 0;
            float fMaxHeatTime = (float)maxHeatTime;


            for (int y = 0; y < m_bin.BinCountY; ++y)
            {
                Vector2 p1 = new Vector2(0.0f, (float)y * m_bin.BinDimension.Y);
                Vector2 p2 = p1;
                p2.X += m_bin.BinDimension.X;
                Vector2 p3 = p1 + m_bin.BinDimension;
                Vector2 p4 = p1;
                p4.Y += m_bin.BinDimension.Y;


                for (int x = 0; x < m_bin.BinCountX; ++x)
                {
                    ulong dTimeStamp = (currentTimeStamp - m_timeStampList[binIdx]);

                    if (dTimeStamp < maxHeatTime)
                    {
                        float binColourMod = 1.0f - ((float)dTimeStamp / fMaxHeatTime);
                        Color binColour = new Color(1.0f, 0.5f, 0.0f, kBinAlpha * binColourMod);
                        debugRenderer.DrawFilledRect(p1, p3, binColour);
                    }


                    p1 = p2;
                    p4 = p3;

                    p2.X = ((float)(x + 2) * m_bin.BinDimension.X);
                    p3.X = p2.X;

                    ++binIdx;
                }

                binIdx = m_bin.BinCountX * (y + 1);
            }
        }
    }
}
