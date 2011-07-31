using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    public struct BinRegion
    {
        internal BinLocation m_cornerLocation1;
        internal BinLocation m_cornerLocation2;
        //internal BinDimension m_dimension;


        bool IsEqual(ref BinRegion binRegion)
        {
            if (m_cornerLocation1.m_x != binRegion.m_cornerLocation1.m_x ||
                m_cornerLocation1.m_y != binRegion.m_cornerLocation1.m_y ||
                m_cornerLocation2.m_x != binRegion.m_cornerLocation2.m_x ||
                m_cornerLocation2.m_y != binRegion.m_cornerLocation2.m_y)
            {
                return false;
            }

            return true;
        }
    }
}
