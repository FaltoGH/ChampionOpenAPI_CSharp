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

        [TestMethod]
        public void GetExpCodeTests()
        {
            IChampionCommAgent agent = new AxChampionCommAgent2();

            string[] lines = File.ReadAllLines("../../SECRET.tmp");

            agent.VersionCheck(nVersionPassKey =>
            {
                int ret = agent.CommLogin(nVersionPassKey, lines[0], lines[1], lines[2]);
                Assert.AreEqual(0, ret);

                string expCode = agent.GetExpCode("000660");
                Assert.AreEqual("KR7000660001", expCode);

                Console.WriteLine("Done 1");
                Application.Exit();
            });

            Application.Run();
            Console.WriteLine("Done 2");
        }

    }
}
