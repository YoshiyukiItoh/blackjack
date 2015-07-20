using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack
{
    public class Blackjack : Game
    {
        private string[] cards;
        private List<Player> players;
        private OutputInterface outputIf;

        public Blackjack(List<Player> players, OutputInterface outputIf)
        {
            this.players = players;
            this.outputIf = outputIf;
        }

        public string Play()
        {
            string winner;
            CreateCards(out cards);

            foreach (Player player in players)
            {
                player.SetCardList(Deal(ref cards, 2));
            }

            winner = Judge(players);

            DebugDealAfterCardsState(cards, players);
            return winner;
        }

        /// <summary>
        /// Creates the cards.
        /// cardkinds : c -> clober
        ///             s -> spade
        ///             d -> dia
        ///             h -> heart
        /// </summary>
        /// <param name="cards">Cards Array</param>
        private void CreateCards(out string[] cards)
        {
            cards = new string[52];
            string[] cardkinds = { "c", "s", "d", "h" };
            string[] cardNumbers = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

            int i = 0;
            foreach (string kind in cardkinds)
            {
                foreach (string number in cardNumbers)
                {
                    cards[i++] = kind + number;
                }
            }
        }

        /// <summary>
        /// 山札からカードを1枚取得します.
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private string Deal(ref string[] cards)
        {
            string ret;
            int seed = Environment.TickCount;

            int rndNum;
            do
            {
                Random rnd = new Random(seed++);
                rndNum = rnd.Next(52);
            } while (cards[rndNum] == "");

            ret = cards[rndNum];
            // 選択されたカードは、選択されないようにマークする.
            cards[rndNum] = "";

            return ret;
        }

        private List<string> Deal(ref string[] cards, int Num)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < Num; i++)
            {
                ret.Add(Deal(ref cards));
            }
            return ret;
        }

        /// <summary>
        /// プレイヤーの勝利判定を行います.
        /// </summary>
        /// <param name="player1">player1のポイントリスト</param>
        /// <param name="player2">player2のポイントリスト</param>
        /// <returns>勝利したプレイヤー名</returns>
        private string Judge(List<Player> players)
        {
            string winner = String.Empty;
            int maxScore = 0;
            foreach (Player player in players)
            {
                int score = player.GetMaxScore();
                if (maxScore <= score)
                {
                    maxScore = score;
                    winner = player.GetName();
                }
            }
            return winner;
        }

        private void DebugDealAfterCardsState(string[] cards, List<Player> players)
        {
            outputIf.WriteLine("====================================================");
            outputIf.WriteLine("cards state");
            outputIf.WriteLine("====================================================");
            int i = 0;
            foreach (string str in cards)
            {
                if (i == 13)
                {
                    i = 0;
                    outputIf.WriteLine(String.Empty);
                }
                outputIf.Write(String.Format("{0,4}", str));
                i++;
            }
            outputIf.WriteLine(String.Empty);
            outputIf.WriteLine("====================================================");
            foreach (Player player in players)
            {
                outputIf.WriteLine("{0} : ", new Object[] { player.GetName() });
                List<string> cardList = player.GetCardList();
                foreach (string card in cardList)
                {
                    outputIf.Write(String.Format("{0,4}", card));
                }
                outputIf.WriteLine(String.Empty);
            }
        }

    }
}
