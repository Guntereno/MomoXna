using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.Spatial
{
	public class BinItem
	{
		internal BinRegionUniform m_region;



		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public void GetBinRegion(ref BinRegionUniform region)
		{
			region = m_region;
		}


		public void SetBinRegion(BinRegionUniform region)
		{
			m_region = region;
		}
	}
}
