using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blackjack
{
    class Blackjack : Game
    {
        private const int borderline_point = 21;
        private string[] cards;
        private List<Player> players;
		private OutputInterface outputIf;

		private Dictionary<string, int> pointTable = new Dictionary<string, int>()
		{
			{"A", 1},{"2", 2},{"3", 3},{"4", 4},
			{"5", 5},{"6", 6},{"7", 7},{"8", 8},
			{"9", 9},{"J", 10},{"Q", 10},{"K", 10}
		};


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
        /// プレイヤーの手札をもとにスコアの計算を行います.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private List<int> CalcScore(List<string> player)
        {
            List<int> scoreList = new List<int>();
            int cntA = 0;
            foreach (string str in player)
            {
                // ex.
                // num -> 10  str -> h10
                // num -> A   str -> dA
                string num = str.Substring(1);
                int score = GetScore(num);
                if (score == 1)
                    cntA++;
                scoreList.Add(GetScore(num));
            }
            scoreList.Sort();

            List<int> ret = CreateCombi(cntA);
            List<int> removeList = new List<int>();

            for (int src_index = cntA; src_index < scoreList.Count; src_index++)
            {
                for (int ret_index = 0; ret_index < ret.Count; ret_index++)
                {
                    ret[ret_index] += scoreList[src_index];
                    // 21 以上なら削除リスト追加
                    if (ret[ret_index] > borderline_point)
                        removeList.Add(ret_index);
                }
            }

            // 21以上をretから削除
            foreach (int index in removeList)
                ret.RemoveAt(index);

            return ret;
        }

        /// <summary>
        /// 点数を返します.
        /// </summary>
        /// <returns>The score.</returns>
        /// <param name="number">Number.</param>
        private int GetScore(string number)
		{
			return pointTable[number] == null ? -1 : pointTable[number];
        }

        /// <summary>
        /// Aの数によって21未満の得点パターンを返却します.
        /// A = 2の場合(2 pattern)
        ///   o  1,1  -> 2
        ///   o  1,11 -> 12
        ///   x 11,11 -> 22
        /// </summary>
        /// <returns>A = 2の場合　{2,12}</returns>
        /// <param name="cnt">Aの数(1 <= cnt <= 4)</param>
        /// 
        private List<int> CreateCombi(int cnt)
        {
            List<int> ret = new List<int>();
            int N = cnt + 1;
            for (int i = 0; i < N; i++)
            {
                int num;
                num = 1 * cnt - i;
                num += 11 * i;
                if (num <= borderline_point)
                    ret.Add(num);
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
                int score = CalcScore(player.GetCardList()).Max();
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
				outputIf.WriteLine("{0} : ", new Object[] {player.GetName()});
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
