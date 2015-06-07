using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace blackjack
{
	class Program
	{
		public static void Main (string[] args)
		{
            List<Player> players = new List<Player>();
            players.Add(new Player("player1"));
            players.Add(new Player("player2"));

            Main main = new Main(new Blackjack(players)
                                ,new DatabasePersistence());
            main.Start();
		}

	}
}
