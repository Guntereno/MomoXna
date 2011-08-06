using System;
using System.Collections.Generic;
using System.Text;

using Momo.Core.GameEntities;



namespace Momo.Core.Spatial
{
	public class BinQueryResults
	{
		private int m_binItemCount = 0;
		private BinItem[] m_binItemQueryResults;



		// --------------------------------------------------------------------
		// -- Public Methods
		// --------------------------------------------------------------------
		public int BinItemCount
		{
			get { return m_binItemCount; }
		}

		public BinItem[] BinItemQueryResults
		{
			get { return m_binItemQueryResults; }
		}



		public BinQueryResults(int queryResultsCapacity)
		{
			m_binItemQueryResults = new BinItem[queryResultsCapacity];
		}


		public void Clear()
		{
			m_binItemCount = 0;
		}


		public int AddBinItem(BinItem item)
		{
			m_binItemQueryResults[m_binItemCount] = item;
			++m_binItemCount;

			return m_binItemCount;
		}


		public int AddBinItem(BinItem item, int checkToIndex)
		{
			bool itemInList = IsItemInList(item, checkToIndex);

			if (itemInList == false)
			{
				m_binItemQueryResults[m_binItemCount] = item;
				++m_binItemCount;
			}

			return m_binItemCount;
		}


		public bool IsItemInList(BinItem item, int checkToIndex)
		{
			for (int i = 0; i < checkToIndex; ++i)
			{
				if (m_binItemQueryResults[i] == item)
					return true;
			}

			return false;
		}

	}
}
