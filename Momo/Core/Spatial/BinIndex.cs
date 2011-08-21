using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    public struct BinIndex
    {
        public const short kInvalidIndex = short.MaxValue;

        internal short m_index;


        public BinIndex(int index)
        {
            m_index = (short)index;
        }
    }
}
