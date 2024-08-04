using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public static class RandomHelper
    {
        //theta(n)
        public static void Shuffle<T>(Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n);
                n--;

                // swap n and k
                T tmp = array[n];
                array[n] = array[k];
                array[k] = tmp;
            }
        }

        //theta(n)
        public static T[] Sample<T>(Random rng, IList<T> array, int count)
        {
            int[] range = Enumerable.Range(0, array.Count).ToArray();
            Shuffle(rng, range);
            T[] ret = new T[count];
            for(int i = 0; i < count; i++)
            {
                ret[i] = array[range[i]];
            }
            return ret;
        }

    }
}
