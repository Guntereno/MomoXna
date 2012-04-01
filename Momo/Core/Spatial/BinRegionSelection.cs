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



        public BinIndex[] BinIndices
        {
            get { return m_binIndices; }
        }

        public int BinCount
        {
            get { return m_binCnt; }
        }



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


        public void Clear()
        {
            m_binCnt = 0;
        }


        public void AddBinIndex(BinIndex index)
        {
            m_binIndices[m_binCnt++] = index;
        }


        public void AddBinIndex(int index)
        {
            m_binIndices[m_binCnt++] = new BinIndex(index);
        }


        public bool HasBinIndex(BinIndex index)
        {
            for (int i = 0; i < m_binCnt; ++i)
            {
                if (m_binIndices[i].m_index == index.m_index)
                    return true;
            }

            return false;
        }


        public bool HasBinIndex(int index)
        {
            return HasBinIndex(new BinIndex(index));
        }
    }
}
