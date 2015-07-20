using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blackjack
{
    public class Player
    {
        private const int borderline_point = 21;
        private Dictionary<string, int> pointTable = new Dictionary<string, int>()
		{
			{"A", 1},{"2", 2},{"3", 3},{"4", 4},
			{"5", 5},{"6", 6},{"7", 7},{"8", 8},
			{"9", 9},{"10",10},{"J", 10},{"Q", 10},
            {"K", 10}
		};

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

        /// <summary>
        /// 点数を返します.
        /// </summary>
        /// <returns>The score.</returns>
        /// <param name="number">Number.</param>
        private int GetScore(string number)
        {
            if (pointTable.ContainsKey(number))
            {
                return pointTable[number];
            }
            return -1;
        }

        public int GetMaxScore()
        {
            return GetScores().Max();
        }

        /// <summary>
        /// プレイヤーの手札をもとにスコアの計算を行います.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private List<int> GetScores()
        {
            List<int> scoreList = new List<int>();
            int cntA = 0;
            foreach (string str in this.cardList)
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

            List<int> ret = CreateACombiPattern(cntA);
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
        /// Aの数によって21未満の得点パターンを返却します.
        /// A = 2の場合(2 pattern)
        ///   o  1,1  -> 2
        ///   o  1,11 -> 12
        ///   x 11,11 -> 22
        /// </summary>
        /// <returns>A = 2の場合　{2,12}</returns>
        /// <param name="cnt">Aの数(1 <= cnt <= 4)</param>
        /// 
        private List<int> CreateACombiPattern(int cnt)
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
    }
}
