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
            if (Console.ReadLine() != "y")
            {
                return;
            }
            agent.VersionCheck();
            Console.WriteLine("Version check done.");
            Console.Write("Login? (y/n)");
            if(Console.ReadLine() != "y")
            {
                return;
            }
            Console.Write("User ID: ");
            string id = Console.ReadLine();
            Console.Write("Password: ");
            string pwd = Console.ReadLine();
            int ret = agent.Login(id, pwd, null);
            Console.WriteLine("Login returned " + ret);
            cmd:
            Console.WriteLine("\nType a number.");
            Console.WriteLine("0: Exit");
            Console.WriteLine("1: Print code list");
            Console.Write(">>> ");
            string cmd = Console.ReadLine();
            switch (cmd)
            {
                case "0":
                    agent.Dispose();
                    return;
                case "1":
                    List<string> codeList = agent.GetCodeList();
                    Console.WriteLine(codeList.Count + " codes");
                    break;
            }
            goto cmd;
        }

    }

}
