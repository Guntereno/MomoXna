using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
	public struct BinLocation
	{
        public static BinLocation kInvalidBinLocation = new BinLocation(BinIndex.kInvalidIndex, BinIndex.kInvalidIndex);


		internal short m_x;
		internal short m_y;

        public short X
        {
            get { return m_x; }
        }

        public short Y
        {
            get { return m_y; }
        }


        public BinLocation(short x, short y)
        {
            m_x = x;
            m_y = y;
        }


        public void Invalidate()
        {
            m_x = BinIndex.kInvalidIndex;
            m_y = BinIndex.kInvalidIndex;
        }

	}
}
