using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    public struct BinRegionSelection
    {
        internal BinIndex [] m_binIndices;


        public BinRegionSelection(int capacity)
        {
            m_binIndices = new BinIndex[capacity];
        }


        // Creates an array the exact size as the count of the input array, no wastage.
        public BinRegionSelection(ref BinRegionSelection binSelection)
        {
            m_binIndices = new BinIndex[binSelection.m_binIndices.Length];
            binSelection.m_binIndices.CopyTo(m_binIndices, 0);
        }
    }
}
