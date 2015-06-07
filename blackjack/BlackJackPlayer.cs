using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blackjack
{
    class BlackJackPlayer : Player
    {
        private int maxScore;

        public BlackJackPlayer(string name, List<string> cardList)
            : base(name, cardList)
        {
            SetMaxScore();
        }

        private void SetMaxScore()
        {
        }
    }
}
