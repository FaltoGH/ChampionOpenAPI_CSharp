using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{

    internal class progrm
    {
        public static readonly Random rng = new Random();
        private readonly axcca2 agent;

        private bool VersionCheck()
        {
            Console.Write("Version check? (y/n)");
            if (Console.ReadLine() != "y")
            {
                return false;
            }
            agent.VersionCheck();
            Console.WriteLine("Version check done.");
            return true;
        }

        private string ReadPwd()
        {
            string pass = string.Empty;
            ConsoleKey key;
            do
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass.Substring(0, pass.Length - 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();
            return pass;
        }

        private bool Login()
        {
            while (true)
            {
                Console.Write("Login? (y/n)");
                if (Console.ReadLine() != "y")
                {
                    return false;
                }
                Console.Write("User ID: ");
                string id = Console.ReadLine();
                Console.Write("Password: ");
                string pwd = ReadPwd().ToString();
                Console.Write("Certificate Password: ");
                string certpwd = ReadPwd().ToString();
                if (string.IsNullOrWhiteSpace(certpwd)) certpwd = null;
                int ret = agent.Login(id, pwd, certpwd);
                Console.WriteLine("Login returned " + ret);
                if (ret == 0)
                {
                    return true;
                }
                Console.WriteLine("LastErrMsg: " + agent.GetLastErrMsg());
            }
        }

        private void PrintCodeList()
        {
            List<string> codeList = agent.GetCodeList();
            Console.WriteLine(codeList.Count + " codes");
            string[] sample = rng.Sample(codeList, 9);
            Console.WriteLine("Random sample codes are:");
            foreach(var s in sample)
            {
                Console.WriteLine(s);
            }
        }

        private void PrintChart()
        {
            Console.Write("Enter code of jongmok: ");
            string jmcode = Console.ReadLine();
            Console.Write("Enter request count: ");
            short nRequestCount = short.Parse(Console.ReadLine());
            gbdayfr gbdayss = agent.gbdayf2(jmcode, "1", nRequestCount);
            if (gbdayss.success)
            {
                Console.WriteLine(gbdayss.gbdayss.Count + " gbdays were fetched.");
                gbdays[] gbdaysss = rng.Sample(gbdayss.gbdayss, 9);
                foreach(var s in gbdaysss)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine(gbdayss);
            }
        }

        private void GetAccInfo()
        {
            string accInfo = agent.GetAccInfo();
            string[] accnos = accInfo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (accnos.Length == 1)
            {
                Console.WriteLine("1 account found.");
            }
            else
            {
                Console.WriteLine(accnos.Length + " accounts found.");
            }
            for(int i = 0; i < accnos.Length; i++)
            {
                Console.WriteLine(accnos[i]);
            }
        }

        public progrm()
        {
            agent = new axcca2();
            if (!VersionCheck()) return;
            if (!Login()) return;
        cmd:
            Console.WriteLine("vvvvvvvvvvvvvvvv");
            Console.WriteLine("Input the digit of the following commands:");
            Console.WriteLine("0: exit the program");
            Console.WriteLine("1: print code list");
            Console.WriteLine("2: print chart");
            Console.WriteLine("3: print account info");
            Console.WriteLine("^^^^^^^^^^^^^^^^");
            Console.Write(">>> ");
            string cmd = Console.ReadLine();
            switch (cmd)
            {
                case "0":
                    agent.Dispose();
                    return;
                case "1":
                    PrintCodeList();
                    break;
                case "2":
                    PrintChart();
                    break;
                case "3":
                    GetAccInfo();
                    break;
                default:
                    if(!string.IsNullOrWhiteSpace(cmd))
                        Console.WriteLine("Unknown command.");
                    break;
            }
            goto cmd;
        }

        private static void Main(string[] args)
        {
            new progrm();
        }
    }

}
