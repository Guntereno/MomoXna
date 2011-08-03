using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    public struct BinRegionUniform
    {
        internal BinLocation m_minLocation;
        internal BinLocation m_maxLocation;
        //internal BinDimension m_dimension;


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
