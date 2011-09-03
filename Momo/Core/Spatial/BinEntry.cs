using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
    public class BinEntry : SentinelBinEntry
    {
        internal BinItem m_item = null;
    }

    public class SentinelBinEntry
    {
        internal BinEntry m_nextEntry = null;
    }
}
