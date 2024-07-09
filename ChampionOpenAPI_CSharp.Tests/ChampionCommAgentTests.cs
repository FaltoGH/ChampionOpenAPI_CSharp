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

                Console.WriteLine("Done 1");
                Application.Exit();
            });

            Application.Run();
            Console.WriteLine("Done 2");
        }

        [TestMethod]
        public void GetExpCodeTests()
        {
            TestInside(agent =>
            {
                string expCode = agent.GetExpCode("000660");
                Assert.AreEqual("KR7000660001", expCode);

                string shCode = agent.GetShCode(expCode);
                Assert.AreEqual("000660", shCode);

                string name = agent.GetNameByCode(shCode);
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
                string code = codeList[2];
                Console.WriteLine(agent.GetOverseaStockInfo(code, 0));
                Assert.AreEqual(code, agent.GetOverseaStockInfo(code, 1));
                Console.WriteLine(agent.GetOverseaStockInfo(code, 5));
                Console.WriteLine(agent.GetOverseaStockInfo(code, 6));
            });
        }

    }
}
