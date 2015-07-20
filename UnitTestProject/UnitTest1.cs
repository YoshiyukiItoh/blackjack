using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using blackjack;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string name = "player1";
            List<Player> players = new List<blackjack.Player>();
            players.Add(new Player(name));

            var obj = new Blackjack(players, new CliInterface());
            string str = obj.Play();
            Assert.AreEqual(name, str);
        }
    }

    [TestClass]
    public class UnitTestPlayerClass
    {
        [TestMethod]
        public void TestMethod_CreateACombiPattern1()
        {
            Player p = new Player("p");
            var pObj = new PrivateObject(p);

            var actualList = (List<int>)pObj.Invoke("CreateACombiPattern", new Object[] { 1 });
            actualList.Sort();
            var expectedList = new List<int>() {1,11};
            for(int i = 0;i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i], actualList[i]);
            }
        }

        [TestMethod]
        public void TestMethod_CreateACombiPattern2()
        {
            Player p = new Player("p");
            var pObj = new PrivateObject(p);

            var actualList = (List<int>)pObj.Invoke("CreateACombiPattern", new Object[] { 2 });
            actualList.Sort();
            var expectedList = new List<int>() { 2, 12 };
            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i], actualList[i]);
            }
        }

        [TestMethod]
        public void TestMethod_CreateACombiPattern3()
        {
            Player p = new Player("p");
            var pObj = new PrivateObject(p);

            var actualList = (List<int>)pObj.Invoke("CreateACombiPattern", new Object[] { 3 });
            actualList.Sort();
            var expectedList = new List<int>() { 3, 13 };
            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i], actualList[i]);
            }
        }

        [TestMethod]
        public void TestMethod_CreateACombiPattern4()
        {
            Player p = new Player("p");
            var pObj = new PrivateObject(p);

            var actualList = (List<int>)pObj.Invoke("CreateACombiPattern", new Object[] { 4 });
            actualList.Sort();
            var expectedList = new List<int>() { 4, 14 };
            for (int i = 0; i < expectedList.Count; i++)
            {
                Assert.AreEqual(expectedList[i], actualList[i]);
            }
        }
    }
}
