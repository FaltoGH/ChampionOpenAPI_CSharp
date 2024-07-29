using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            AxChampionCommAgent2 agent = new AxChampionCommAgent2();
            Console.Write("Version check? (y/n)");
            if (Console.ReadLine() == "y")
            {
                Concurrent<int> a = new Concurrent<int>(-1);
                agent.VersionCheck(a);
                a.WaitWhile(x => x == -1);
                Console.WriteLine("Version check done.");
            }
        }

    }

}
