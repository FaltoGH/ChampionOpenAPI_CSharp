﻿using System;
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
            return pass;
        }

        private bool Login()
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
            int ret = agent.Login(id, pwd, null);
            Console.WriteLine("Login returned " + ret);
            return true;
        }

        private void PrintCodeList()
        {
            List<string> codeList = agent.GetCodeList();
            Console.WriteLine(codeList.Count + " codes");
            string[] sample = RandomHelper.Sample(rng, codeList.ToArray(), 9);
            Console.WriteLine("Random sample codes are:");
            for (byte i = 0; i < 9; i++)
            {
                Console.WriteLine(sample[i]);
            }
        }

        public Program()
        {
            agent = new AxChampionCommAgent2();
            if (!VersionCheck()) return;
            if(!Login()) return;
        cmd:
            Console.WriteLine("vvvvvvvvvvvvvvvv");
            Console.WriteLine("Type a number.");
            Console.WriteLine("0: Exit");
            Console.WriteLine("1: Print code list");
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
                default:
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
