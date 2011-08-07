using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
	public struct BinLocation
	{
		internal short m_x;
		internal short m_y;


        public BinLocation(short x, short y)
        {
            m_x = x;
            m_y = y;
        }
	}
}
