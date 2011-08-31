using System;
using System.Collections.Generic;



namespace Momo.Core
{
    public interface INamed
    {
        void SetName(MutableString name);
        void SetName(string name);

        MutableString GetName();
        int GetNameHash();
    }
}
