using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChampionOpenAPI_CSharp.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IChampionCommAgent agent = new AxChampionCommAgent2();
            List<string> codeList = agent.GetCodeList();
            Assert.IsTrue(codeList.Count > 2000);
            Assert.IsFalse(codeList.Any(string.IsNullOrWhiteSpace));
            Assert.IsTrue(codeList.All(x => x.Length > 0));
            Assert.IsTrue(codeList.All(x => x.First() != ' '));
            Assert.IsTrue(codeList.All(x => x.Last() != ' '));
            Console.WriteLine(codeList[0]);
            Console.WriteLine(codeList[2000]);
        }
    }
}
