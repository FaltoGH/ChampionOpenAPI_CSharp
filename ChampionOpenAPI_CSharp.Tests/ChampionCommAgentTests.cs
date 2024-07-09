using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ChampionOpenAPI_CSharp.Tests
{
    [TestClass]
    public class ChampionCommAgentTests
    {
        private const string SECRET = "../../SECRET.tmp";

        [TestMethod]
        public void GetCodeListTests()
        {
            IChampionCommAgent agent = new AxChampionCommAgent2();
            IReadOnlyList<string> codeList = agent.GetCodeList();
            Assert.IsTrue(codeList.Count > 2000);
            Assert.IsFalse(codeList.Any(string.IsNullOrWhiteSpace));
            Assert.IsTrue(codeList.All(x => x.Length > 0));
            Assert.IsTrue(codeList.All(x => x.First() != ' '));
            Assert.IsTrue(codeList.All(x => x.Last() != ' '));
            Console.WriteLine(codeList[0]);
            Console.WriteLine(codeList[2000]);
        }

        private void TestInside(Action<IChampionCommAgent> x)
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            IChampionCommAgent agent = new AxChampionCommAgent2();

            string[] lines = File.ReadAllLines(SECRET);

            agent.VersionCheck(nVersionPassKey =>
            {
                int ret = agent.CommLogin(nVersionPassKey, lines[0], lines[1], lines[2]);
                Assert.AreEqual(0, ret);

                x?.Invoke(agent);

                Console.WriteLine("Done! (1)");
                Application.Exit();
            });

            Application.Run();
            Console.WriteLine("Done! (2)");
        }

        [TestMethod]
        public void GetExpCodeTests()
        {
            const string shCode = "000660";
            const string expCode = "KR7000660001";
            const string name = "SK하이닉스";

            TestInside(agent =>
            {
                Assert.AreEqual(expCode, agent.GetExpCode(shCode));
                Assert.AreEqual(shCode, agent.GetShCode(expCode));
                Assert.AreEqual(name, agent.GetNameByCode(shCode));
                Assert.AreEqual(name, agent.GetNameByCode(expCode));
                Assert.AreEqual(shCode, agent.GetShCodeByName(name));
            });
        }

        [TestMethod]
        public void GetOverseaStockInfoTests()
        {
            TestInside(agent =>
            {
                IReadOnlyList<string> codeList = agent.GetCodeList();

                // 0066AAMC
                string code = codeList[2];

                string shCode = agent.GetShCode(code);
                // AMX_AAMC
                Console.WriteLine("GetShCode=" + shCode);

                Assert.AreEqual(code, agent.GetExpCode(shCode));

                // not work
                string name = agent.GetNameByCode(code);
                Console.WriteLine("GetNameByCode=" + name);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    Assert.AreEqual(shCode, agent.GetShCodeByName(name));
                }

                Assert.AreEqual(shCode, agent.GetOverseaStockInfo(code, 0)); // shortcode

                Assert.AreEqual(code, agent.GetOverseaStockInfo(code, 1)); // expcode

                string s;
                for(byte i = 5; i < 15; i++)
                {
/*
5: 알티소스 에셋 매니지먼트
6: ALTISOURCE ASSET MANAGEMENT CORP
7: VI02153X1080
8: 4540
9: 1
10: 00000001
11: 4
12: 1
13: USD
14: 20240709
*/
                    s = agent.GetOverseaStockInfo(code, i);
                    Assert.IsFalse(string.IsNullOrWhiteSpace(s));
                    Console.WriteLine("{0}: {1}", i, s);
                }

                // 15: 020
                s = agent.GetOverseaStockInfo(code, 15);
                Assert.IsFalse(string.IsNullOrWhiteSpace(s));
                Console.WriteLine("15: " + s); // 계정계용 거래소 코드

                // not work
                Console.WriteLine("16: " + agent.GetOverseaStockInfo(s, 16)); // 계정계용 거래소+심볼코드를 풀코드로 변환
            });
        }

    }
}
