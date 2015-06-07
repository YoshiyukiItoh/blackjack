﻿using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace blackjack
{
	class MainClass
	{
		private const int borderline_point = 21;

		public static void Main (string[] args)
		{
			MainClass instance = new MainClass ();
			instance.Solve ();
		}

		private void Solve ()
		{
			string[] cards = new string[52];

			// create card data
			CreateCards (ref cards);
			// deal
			string[] player1_cards = Deal (ref cards);
			string[] player2_cards = Deal (ref cards);

			// debug
            DebugDealAfterCardsState(cards, player1_cards, player2_cards);

			// calc
			// debug
			//calcScore(new string[] {"dA","hA", "h10"});
			List<int> player1_scores = CalcScore (player1_cards);
			List<int> player2_scores = CalcScore (player2_cards);

			// judge
			string winner = Judge (player1_scores, player2_scores);
            Console.WriteLine("====================================================");
            Console.WriteLine("Result");
            Console.WriteLine("====================================================");
			Console.WriteLine ("win : {0}", winner);
            Console.WriteLine("player1 score : {0}  card1 : {1} card2 : {2}", player1_scores.Max(), player1_cards[0], player1_cards[1]);
            Console.WriteLine("player2 score : {0}  card1 : {1} card2 : {2}", player2_scores.Max(), player2_cards[0], player2_cards[1]);
            
			// save result
			saveResult (winner);
            Console.ReadLine();
		}

		/// <summary>
		/// Creates the cards.
		/// cardkinds : c -> clober
		///             s -> spade
		///             d -> dia
		///             h -> heart
		/// </summary>
		/// <param name="cards">Cards Array</param>
		private void CreateCards (ref string[] cards)
		{
			string[] cardkinds = { "c", "s", "d", "h" };
			string[] cardNumbers = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

			int i = 0;
			foreach (string kind in cardkinds) {
				foreach (string number in cardNumbers) {
					cards [i++] = kind + number;
				}
			}
		}

        /// <summary>
        /// 山札から2枚カードを取得します.
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
		private string[] Deal (ref string[] cards)
		{
			string[] ret = new string[2];
			int seed = Environment.TickCount;

			for (int i = 0; i < 2; i++) {
				int rndNum;
				do {
					Random rnd = new Random (seed++);
					rndNum = rnd.Next (52);
				} while(cards [rndNum] == "");

				// 選択されたカードは、選択されないようにマークする.
				ret [i] = cards [rndNum];
				cards [rndNum] = "";

			}
			return ret;
		}

        /// <summary>
        /// プレイヤーの手札をもとにスコアの計算を行います.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
		private List<int> CalcScore (string[] player)
		{
			List<int> scoreList = new List<int> ();
			int cntA = 0;
			foreach (string str in player) {
				// ex.
				// num -> 10  str -> h10
				// num -> A   str -> dA
				string num = str.Substring (1);
				int score = GetScore (num);
				if (score == 1)
					cntA++;
				scoreList.Add (GetScore (num));
			}
			scoreList.Sort ();

			List<int> ret = CreateCombi (cntA);
			List<int> removeList = new List<int> ();

			for (int src_index = cntA; src_index < scoreList.Count; src_index++) {
				for (int ret_index = 0; ret_index < ret.Count; ret_index++) {
					ret [ret_index] += scoreList [src_index];
					// 21 以上なら削除リスト追加
					if (ret [ret_index] > borderline_point)
						removeList.Add (ret_index);
				}
			}

			// 21以上をretから削除
			foreach (int index in removeList)
				ret.RemoveAt (index);

			return ret;
		}

		/// <summary>
		/// 点数を返します.
		/// </summary>
		/// <returns>The score.</returns>
		/// <param name="number">Number.</param>
		private int GetScore (string number)
		{
			int ret = -1;
			switch (number) {
			case "A":
				ret = 1;
				break;
			case "2":
			case "3":
			case "4":
			case "5":
			case "6":
			case "7":
			case "8":
			case "9":
				ret = int.Parse (number);
				break;
			case "10":
			case "J":
			case "Q":
			case "K":
				ret = 10;
				break;
			default:
				break;
			}
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
		private List<int> CreateCombi (int cnt)
		{
			List<int> ret = new List<int> ();
			int N = cnt + 1;
			for (int i = 0; i < N; i++) {
				int num;
				num = 1 * cnt - i;
				num += 11 * i;
				if (num <= borderline_point)
					ret.Add (num);
			}
			return ret;
		}

        /// <summary>
        /// プレイヤーの勝利判定を行います.
        /// </summary>
        /// <param name="player1">player1のポイントリスト</param>
        /// <param name="player2">player2のポイントリスト</param>
        /// <returns>勝利したプレイヤー名</returns>
		private string Judge (List<int> player1, List<int> player2)
		{
			return player1.Max () > player2.Max () ? "player1" : "player2";
		}

        /// <summary>
        /// プレイヤーの勝利を保存します.
        /// </summary>
        /// <param name="playerName"></param>
		private void saveResult (string playerName)
		{
            string tableName = "player_info";
            
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = CreateConnection();
            OracleTransaction tran = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();

                cmd.Connection = conn;
                cmd.Transaction = tran;

                string sql = String.Format("select name,win_count from {0} where name = '{1}' for update", tableName, playerName);
                int win_count = SelectWinCount(cmd, sql);

                if (win_count == 0)
                {
                    sql = String.Format("insert into {0} values ('{1}',{2})", tableName, playerName, 1);
                }
                else
                {
                    sql = String.Format("update {0} set win_count = {1} where name = '{2}'", tableName, win_count + 1, playerName);
                }
                UpdateWinCount(cmd, sql);
                tran.Commit();
            }
            catch (Exception e)
            {
                if (tran != null) tran.Rollback();
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
		}

        /// <summary>
        /// OracleConnectionのオブジェクトを作成します.
        /// </summary>
        /// <returns>対象DBへのoracleConnectionオブジェクト</returns>
		private OracleConnection CreateConnection ()
		{
			string connStr = String.Format (@"User Id={0}; Password={0}; Data Source="
                                          + "(DESCRIPTION="
                                          + " (LOAD_BALANCE=ON)(FAILOVER=ON)"
                                          + "  (ADDRESS=(PROTOCOL=TCP)(HOST={1})(PORT={3}))"
                                          + "  (ADDRESS=(PROTOCOL=TCP)(HOST={2})(PORT={3}))"
                                          + "  (CONNECT_DATA=(SERVICE_NAME={4}))"
                                          + " )", "test", "192.168.56.111", "192.168.56.112", "1521","ORCL");
			return new OracleConnection (connStr);
		}

        /// <summary>
        /// 勝利したプレイヤーの勝利数を取得します.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
		private int SelectWinCount (OracleCommand cmd, string sql)
		{
			//SQL実行
			cmd.CommandText = sql;
			OracleDataReader reader = cmd.ExecuteReader ();
			int win_count = 0;

			while (reader.Read ()) {
                //win_count = reader.GetInt32(1);
                win_count = int.Parse(reader["win_count"].ToString());
			}
			reader.Close ();
			return win_count;
		}

        /// <summary>
        /// 更新系sqlを発行します.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="sql"></param>
        /// <returns>更新件数</returns>
        private int UpdateWinCount(OracleCommand cmd, string sql)
		{
			cmd.CommandText = sql;
			return cmd.ExecuteNonQuery ();
		}

        

		static void DebugDealAfterCardsState (string[] cards, string[] player1, string[] player2)
		{
            Console.WriteLine("====================================================");
            Console.WriteLine("cards state");
            Console.WriteLine("====================================================");
			int i = 0;
			foreach (string str in cards) {
				if (i == 13) {
					i = 0;
					Console.WriteLine ();
				}
				Console.Write (String.Format ("{0,4}", str));
				i++;
			}
            Console.WriteLine();
            Console.WriteLine("====================================================");
			Console.WriteLine ("player1 : ");
			foreach (string str in player1) {
				Console.Write (String.Format ("{0,4}", str));
			}
            Console.WriteLine();
			Console.WriteLine ("player2 : ");
			foreach (string str in player2) {
				Console.Write (String.Format ("{0,4}", str));
			}
			Console.WriteLine ();
		}
	}
}
