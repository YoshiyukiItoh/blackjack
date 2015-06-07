using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blackjack
{
    abstract class Player
    {
        protected string name;
        protected List<string> cardList;

        protected Player(string name, List<string> cardList)
        {
            this.name = name;
            this.cardList = cardList;
        }

        protected string GetName()
        {
            return name;
        }

        protected void SetName(string name)
        {
            this.name = name;
        }

        protected List<string> GetCardList()
        {
            return cardList;
        }

        protected void SetCardList(List<string> cardList)
        {
            this.cardList = cardList;
        }
    }
}
