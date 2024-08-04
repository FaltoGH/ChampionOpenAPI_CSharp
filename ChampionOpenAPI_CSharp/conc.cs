using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{
    public class conc<T>
    {
        private T __value;
        private readonly object __lock = new object();

        public conc() { }

        public conc(T value)
        {
            __value = value;
        }

        public void Set(T value)
        {
            lock (__lock)
            {
                __value = value;
            }
        }

        public T Get()
        {
            lock (__lock)
            {
                return __value;
            }
        }

        public void WaitWhile(Predicate<T> predicate)
        {
            while(predicate(__value))
            {
                Thread.Sleep(1);
            }
        }
        
    }

}
