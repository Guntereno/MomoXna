using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.ObjectPools
{
    public struct DestroyedPoolItem<T> where T : IPoolItem
    {
        public T m_item;
        public int m_tickCount;
    }
}
