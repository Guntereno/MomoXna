using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Core
{
    public class Hashing
    {
        public static int GenerateHash(char[] str, int length)
        {
            int hash = 0;
            for (int i = 0; i < length; i++)
            {
                hash = (hash << 5) - hash + str[i];
            }

            return hash;
        }
    }
}
