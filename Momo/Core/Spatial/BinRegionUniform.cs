﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    public struct BinRegionUniform
    {
        public static BinRegionUniform kInvalidBinRegionUniform = new BinRegionUniform(BinLocation.kInvalidBinLocation, BinLocation.kInvalidBinLocation);


        internal BinLocation m_minLocation;
        internal BinLocation m_maxLocation;



        public BinRegionUniform(BinLocation minLocation, BinLocation maxLocation)
        {
            m_minLocation = minLocation;
            m_maxLocation = maxLocation;
        }


        public BinLocation MinLocation
        {
            get{ return m_minLocation; }
        }

        public BinLocation MaxLocation
        {
            get { return m_maxLocation; }
        }

        public int GetHeight()
        {
            return m_maxLocation.m_y - m_minLocation.m_y;
        }


        public int GetWidth()
        {
            return m_maxLocation.m_x - m_minLocation.m_x;
        }


        public bool IsEqual(ref BinRegionUniform binRegion)
        {
            if (m_minLocation.m_x != binRegion.m_minLocation.m_x ||
                m_minLocation.m_y != binRegion.m_minLocation.m_y ||
                m_maxLocation.m_x != binRegion.m_maxLocation.m_x ||
                m_maxLocation.m_y != binRegion.m_maxLocation.m_y)
            {
                return false;
            }

            return true;
        }


        public bool IsInRegion(int x, int y)
        {
            if (m_minLocation.m_x < x ||
                m_maxLocation.m_x > x ||
                m_maxLocation.m_y < y ||
                m_maxLocation.m_y > y)
            {
                return false;
            }

            return true;
        }


        public bool IsInRegion(BinRegionUniform region)
        {
            if (!(region.m_minLocation.m_x <= m_maxLocation.m_x && region.m_maxLocation.m_x >= m_minLocation.m_x))
                return false;

            if (!(region.m_minLocation.m_y <= m_maxLocation.m_y && region.m_maxLocation.m_y >= m_minLocation.m_y))
                return false;

            return true;
        }
    }
}
