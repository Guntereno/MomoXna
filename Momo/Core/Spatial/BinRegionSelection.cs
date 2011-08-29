using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    public struct BinRegionSelection
    {
        internal BinIndex [] m_binIndices;
        internal int m_binCnt;


        public BinRegionSelection(int capacity)
        {
            m_binIndices = new BinIndex[capacity];
            m_binCnt = 0;
        }


        // Creates an array the exact size as the count of the input array, no wastage.
        public BinRegionSelection(ref BinRegionSelection binSelection)
        {
            m_binIndices = new BinIndex[binSelection.m_binIndices.Length];
            binSelection.m_binIndices.CopyTo(m_binIndices, 0);

            m_binCnt = binSelection.m_binCnt;
        }
    }
}
