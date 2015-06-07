using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack
{
    abstract class Player
    {
        protected string name;
        protected string[] cards;

        protected Player(string name, string[] cards)
        {
            this.name = name;
            this.cards = cards;
        }

        protected string GetName()
        {
            return name;
        }

        protected string[] GetCards()
        {
            return cards;
        }
    }
}
