using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    public struct BinIndex
    {
        public static readonly BinIndex kInvalidIndex = new BinIndex(int.MaxValue);

        internal int m_index;


        public BinIndex(int index)
        {
            m_index = index;
        }
    }
}
