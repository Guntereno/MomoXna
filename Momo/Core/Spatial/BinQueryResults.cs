using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Momo.Core.GameEntities;



namespace Momo.Core.Spatial
{
    public class BinQueryResults
    {
        protected int m_binItemCount = 0;
        protected BinItem[] m_binItemQueryResults;



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

        public virtual void StartQuery()
        {
            Clear();
        }


        public void EndQuery()
        {
            
        }



        public void Clear()
        {
            m_binItemCount = 0;
        }


        public int GetClosestBinItemIndex(Vector2 position)
        {
            int closestIdx = 0;
            float closestDistanceSq = float.MaxValue;

            for (int i = 0; i < m_binItemCount; ++i)
            {
                float distanceSq = (m_binItemQueryResults[i].GetPosition() - position).LengthSquared();

                if (distanceSq < closestDistanceSq)
                {
                    closestIdx = i;
                    closestDistanceSq = distanceSq;
                }
            }

            return closestIdx;
        }


        public virtual int AddBinItem(BinItem item)
        {
            m_binItemQueryResults[m_binItemCount] = item;
            ++m_binItemCount;

            return m_binItemCount;
        }


        public int AddBinItem(BinItem item, int checkToIndex)
        {
            bool itemInList = IsItemInList(item, checkToIndex);

            if (itemInList == false)
                return AddBinItem(item);

            return m_binItemCount;
        }


        // Moves end of list to the empty slot in index.
        public virtual void RemoveBinItem(int index)
        {
            --m_binItemCount;

            if(m_binItemCount > 0)
                m_binItemQueryResults[index] = m_binItemQueryResults[m_binItemCount];
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


    public class BinQueryLocalityResults : BinQueryResults
    {
        public struct ExtendedInfo
        {
            public float m_distanceSq;
        }


        private ExtendedInfo[] m_binItemQueryLocalityResults;
        private Vector2 m_localityPosition = Vector2.Zero;


        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public BinQueryLocalityResults(int queryResultsCapacity)
            : base(queryResultsCapacity)
        {
            m_binItemQueryLocalityResults = new ExtendedInfo[queryResultsCapacity];
        }


        public void SetLocalityInfo(Vector2 position)
        {
            m_localityPosition = position;
        }


        public int GetClosestBinItemIndex()
        {
            int closestIdx = 0;
            float closestDistanceSq = float.MaxValue;

            for (int i = 0; i < m_binItemCount; ++i)
            {
                if (m_binItemQueryLocalityResults[i].m_distanceSq < closestDistanceSq)
                {
                    closestIdx = i;
                    closestDistanceSq = m_binItemQueryLocalityResults[i].m_distanceSq;
                }
            }

            return closestIdx;
        }


        public override int AddBinItem(BinItem item)
        {
            m_binItemQueryResults[m_binItemCount] = item;
            m_binItemQueryLocalityResults[m_binItemCount].m_distanceSq = (item.GetPosition() - m_localityPosition).LengthSquared();
            ++m_binItemCount;

            return m_binItemCount;
        }



        // Moves end of list to the empty slot in index.
        public override void RemoveBinItem(int index)
        {
            --m_binItemCount;

            if (m_binItemCount > 0)
            {
                m_binItemQueryResults[index] = m_binItemQueryResults[m_binItemCount];
                m_binItemQueryLocalityResults[index] = m_binItemQueryLocalityResults[m_binItemCount];
            }
        }
    }
}
