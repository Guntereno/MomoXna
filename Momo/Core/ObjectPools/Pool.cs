using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.ObjectPools
{
    public class Pool<T> where T : IPoolItem
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private int m_activeItemCount = 0;
        private int m_inactiveItemCount = 0;

        private int m_itemCapacity = 0;
        private T[] m_activeItems = null;
        private T[] m_inactiveItems = null;




        // --------------------------------------------------------------------
        // -- Public Methods
        // --------------------------------------------------------------------
        public T this[int index]
        {
            get{ return m_activeItems[index]; }
        }

        public T[] ActiveItemList
        {
            get { return m_activeItems; }
        }

        public int ActiveItemListCount
        {
            get { return m_activeItemCount; }
        }


        public Pool(int capacity)
        {
            m_itemCapacity = capacity;
            m_activeItems = new T[capacity];
            m_inactiveItems = new T[capacity];
        }


        // Populate the arrays at the start.
        public void AddItem(T item, bool addToActiveList)
        {
            m_inactiveItems[m_inactiveItemCount++] = item;

            if (addToActiveList)
                CreateItem();
        }


        public T CreateItem()
        {
            T item = m_inactiveItems[--m_inactiveItemCount];

            item.ResetItem();

            m_activeItems[m_activeItemCount++] = item;

            return item;
        }


        public void CoalesceActiveList(bool preserveOrder)
        {
            if (preserveOrder)
                CoalesceActiveListPreserveOrder();
            else
                CoalesceActiveListReorder();
        }


        // This method shifts the order of the the active list. Cheaper but non-ordered.
        private void CoalesceActiveListReorder()
        {
            for (int i = 0; i < m_activeItemCount; ++i)
            {
                if (m_activeItems[i].IsDestroyed())
                {
                    // Move to inactive list
                    m_inactiveItems[m_inactiveItemCount++] = m_activeItems[i];

                    --m_activeItemCount;

                    if (m_activeItemCount > 0)
                    {
                        m_activeItems[i] = m_activeItems[m_activeItemCount];
                        --i;
                    }
                }
            }
        }

        // Maintains the original order of the active list. Slower.
        private void CoalesceActiveListPreserveOrder()
        {
            // Do not check the last one in the main loop, saves an extra conditional.
            for (int i = 0; i < m_activeItemCount - 1; ++i)
            {
                if (m_activeItems[i].IsDestroyed())
                {
                    // Move to inactive list
                    m_inactiveItems[m_inactiveItemCount++] = m_activeItems[i];

                    --m_activeItemCount;

                    if (m_activeItemCount > 0)
                    {
                        Array.Copy(m_activeItems, i + 1, m_activeItems, i, m_activeItemCount - i);
                        --i;
                    }
                }
            }
        }
    }
}
