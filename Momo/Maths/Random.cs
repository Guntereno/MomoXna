using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Momo.Maths
{
    public class RandUtil
    {
        // Shuffle the array.
        // Uses a Fisher Yates shuffle
        public static void Shuffle<T>(T[] array, Random random)
        {
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

    }
}
