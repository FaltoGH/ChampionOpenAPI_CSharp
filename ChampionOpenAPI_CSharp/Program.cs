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
            string[] sample = rng.Sample(codeList, Math.Min(codeList.Count, 9));
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
            ValueTuple2<List<gbday_struct>, string> gbdayss = agent.gbdayf2(jmcode, "1", nRequestCount);
            Console.WriteLine(gbdayss.Item1.Count + " gbdays were fetched.");
            gbday_struct[] gbdaysss = rng.Sample(gbdayss.Item1, Math.Min(gbdayss.Item1.Count, 9));
            foreach (var s in gbdaysss)
            {
                Console.WriteLine(s);
            }
        }

        private void GetAccounts()
        {
            Account[] accs = agent.GetAccounts();
            if (accs.Length == 1)
                Console.WriteLine("1 account found.");
            else
                Console.WriteLine(accs.Length + " accounts found.");
            foreach(var acc in accs)
                Console.WriteLine(acc.Number);
        }

        private Account SelectAccount()
        {
            Account[] accs = agent.GetAccounts();
            if (accs.Length == 0)
            {
                Console.WriteLine("error: You have no account to send order.");
                return null;
            }
            if (accs.Length == 1)
                return accs[0];
            Console.WriteLine("Select account:");
            for (sbyte i = 0; i < accs.Length; i++)
                Console.WriteLine("[" + i + "] " + accs[i]);
            int index;
            unchecked
            {
                index = Console.ReadKey().KeyChar - '0';
            }
            if (index >= 0 && index < accs.Length)
                return accs[index];
            Console.WriteLine("error: Index is out of bound. Exit.");
            return null;
        }

        private string input(string x)
        {
            Console.Write(x);
            return Console.ReadLine();
        }

        private OrderType SelectOrderType()
        {
            Console.WriteLine("Select order type:");
            for(sbyte i = 0; i < OrderType.AllOrderTypes.Length; i++)
                Console.WriteLine("[" + i + "] " + OrderType.AllOrderTypes[i]);
            int index = Console.ReadKey().KeyChar - '0';
            if (index >= 0 && index < OrderType.AllOrderTypes.Length)
                return OrderType.AllOrderTypes[index];
            Console.WriteLine("error: Index is out of bound. Exit.");
            return null;
        }

        private void SendOrder()
        {
            Account acc = SelectAccount();
            if (acc == null) return;
            if (acc.Password == null)
            {
                Console.Write("Account password is not set. Input the password: ");
                string pwd = ReadPwd();
                acc.Password = pwd;
            }
            string sExgCode = input("Input exg code: ");
            string sJmCode = input("Input jm code: ");
            string sOrdQty = input("Input ord qty: ");
            string sOrderPrice = input("Input ord prc: ");
            bool bTradeGb = input("Buy or sell? ([b]/s): ") == "s";
            OrderType orderType = SelectOrderType();
            if (orderType == null) return;
            string sOrderNo = agent.SendBSOrderGB(acc, sExgCode, sJmCode, sOrdQty, sOrderPrice, bTradeGb, orderType);
            Console.WriteLine("Order number is " + sOrderNo);
        }

        public Program()
        {
            agent = new axcca2();
            if (!VersionCheck()) return;
            if (!Login()) return;
            while (true)
            {
                Console.WriteLine(@"vvvvvvvvvvvvvvvv
Input the digit of the following commands:
[0] exit the program
[1] print code list
[2] print chart
[3] print account info
[4] send order
^^^^^^^^^^^^^^^^");
                char cmd = Console.ReadKey(true).KeyChar;
                switch (cmd)
                {
                    case '0':
                        agent.Dispose();
                        return;
                    case '1':
                        PrintCodeList();
                        break;
                    case '2':
                        PrintChart();
                        break;
                    case '3':
                        GetAccounts();
                        break;
                    case '4':
                        SendOrder();
                        break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }

        private static void Main(string[] args)
        {
            new Program();
        }
    }

}
