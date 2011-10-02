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

        private int m_itemCapacity = 0;
        private T[] m_activeItems = null;

        private T[][] m_inactiveItems = null;
        private int[] m_inactiveItemCount;

        private Type[] m_registeredTypes = null;
        private int m_typeCount = 0;


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


        public Pool(int totalCapacity, int maxTypeCount)
        {
            m_itemCapacity = totalCapacity;
            m_activeItems = new T[totalCapacity];
            m_inactiveItems = new T[maxTypeCount][];
            m_inactiveItemCount = new int[maxTypeCount];
            m_registeredTypes = new Type[maxTypeCount];
        }


        public void RegisterPoolObjectType(Type type, int capacity)
        {
            m_registeredTypes[m_typeCount] = type;
            m_inactiveItems[m_typeCount] = new T[capacity];
            ++m_typeCount;
        }


        // Populate the arrays at the start.
        public void AddItem(T item, bool addToActiveList)
        {
            AddToInactiveList(item);

            if (addToActiveList)
                CreateItem(item.GetType());
        }


        public T CreateItem(Type type)
        {
            int inactiveListIdx = GetInactiveListIndexForType(type);

            int inactiveListCount = m_inactiveItemCount[inactiveListIdx];
            T item = m_inactiveItems[inactiveListIdx][inactiveListCount - 1];
            --m_inactiveItemCount[inactiveListIdx];

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
                    AddToInactiveList(m_activeItems[i]);
                    
                    m_activeItems[i] = default(T);
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
                    AddToInactiveList(m_activeItems[i]);

                    m_activeItems[i] = default(T);
                    --m_activeItemCount;

                    if (m_activeItemCount > 0)
                    {
                        Array.Copy(m_activeItems, i + 1, m_activeItems, i, m_activeItemCount - i);
                        --i;
                    }
                }
            }
        }


        private void AddToInactiveList(T item)
        {
            int inactiveListIdx = GetInactiveListIndexForType(item.GetType());

            int inactiveListCount = m_inactiveItemCount[inactiveListIdx];
            m_inactiveItems[inactiveListIdx][inactiveListCount] = item;
            ++m_inactiveItemCount[inactiveListIdx];
        }


        private int GetInactiveListIndexForType(Type type)
        {
            for (int i = 0; i < m_typeCount; ++i)
            {
                if (m_registeredTypes[i] == type)
                    return i;
            }

            System.Diagnostics.Debug.Assert(false, "Type not registered for pool");
            return -1;
        }
    }
}
