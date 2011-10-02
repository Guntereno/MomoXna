using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core.ObjectPools
{
    public interface IPoolItem
    {

        bool IsDestroyed();

        void DestroyItem();
        void ResetItem();
    }
}
