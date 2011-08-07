using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    struct BinIndex
    {
        internal short m_index;


        public BinIndex(int index)
        {
            m_index = (short)index;
        }
    }
}
