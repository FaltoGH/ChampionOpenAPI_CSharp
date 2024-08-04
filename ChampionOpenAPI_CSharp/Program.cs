using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ChampionOpenAPI_CSharp
{

    internal class Program
    {
        public static readonly Random rng = new Random();
        private readonly AxChampionCommAgent2 agent;

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
            string[] sample = RandomHelper.Sample(rng, codeList, 9);
            Console.WriteLine("Random sample codes are:");
            for (byte i = 0; i < 9; i++)
            {
                Console.WriteLine(sample[i]);
            }
        }

        private void PrintChart()
        {
            Console.Write("Enter code of jongmok: ");
            string jmcode = Console.ReadLine();
            gbdays[] gbdayss = agent.gbdayf(jmcode, "1");
            for(int i = 0; i < gbdayss.Length; i++)
            {
                Console.WriteLine(gbdayss[i]);
            }
        }

        private void GetAccInfo()
        {
            Console.WriteLine(agent.GetAccInfo());
        }

        public Program()
        {
            agent = new AxChampionCommAgent2();
            if (!VersionCheck()) return;
            if (!Login()) return;
        cmd:
            Console.WriteLine("vvvvvvvvvvvvvvvv");
            Console.WriteLine("Input one of the following commands:");
            Console.WriteLine("exit");
            Console.WriteLine("codelst");
            Console.WriteLine("chart");
            Console.WriteLine("GetAccInfo");
            Console.WriteLine("^^^^^^^^^^^^^^^^");
            Console.Write(">>> ");
            string cmd = Console.ReadLine();
            switch (cmd)
            {
                case "exit":
                    agent.Dispose();
                    return;
                case "codelst":
                    PrintCodeList();
                    break;
                case "chart":
                    PrintChart();
                    break;
                case "GetAccInfo":
                    GetAccInfo();
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
            goto cmd;
        }

        private static void Main(string[] args)
        {
            new Program();
        }
    }

}
