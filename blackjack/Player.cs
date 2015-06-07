using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack
{
    class Player
    {
        private string name;
        private string[] cards;

        public Player(string name,string[] cards)
        {
            this.name = name;
            this.cards = cards;
        }

        public string GetName()
        {
            return name;
        }

        public string[] GetCards()
        {
            return cards;
        }
    }
}
