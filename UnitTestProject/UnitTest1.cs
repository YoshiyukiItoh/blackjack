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
}
