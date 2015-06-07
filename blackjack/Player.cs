using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blackjack
{
    class Player
    {
        private string name;
        private List<string> cardList;

        public Player(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public List<string> GetCardList()
        {
            return cardList;
        }

        public void SetCardList(List<string> cardList)
        {
            this.cardList = cardList;
        }
    }
}
