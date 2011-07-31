using System;
using System.Collections.Generic;
using System.Text;

using Momo.Core.GameEntiies;



namespace Momo.Core.Spatial
{
    public class BinQueryResults
    {
        private int m_binItemCount = 0;
        private BinItem[] m_binItemQueryResults;


        public BinQueryResults(int queryResultsCapacity)
        {
            m_binItemQueryResults = new BinItem[queryResultsCapacity];
        }


        public void Clear()
        {
            m_binItemCount = 0;
        }


        public void AddBinItem(BinItem binItem)
        {
            m_binItemQueryResults[m_binItemCount] = binItem;
            ++m_binItemCount;
        }

    }
}
