using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace blackjack
{
	class MainClass
	{
		private const int borderline_point = 21;

		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			MainClass instance = new MainClass ();
			instance.solve ();
		}

		private void solve ()
		{
			string[] cards = new string[52];

			// create card data
			createCards (ref cards);
			// deal
			string[] player1_cards = deal (ref cards);
			string[] player2_cards = deal (ref cards);

			// debug
			//debugDealAfterState (cards, player1, player2);

			// calc
			// debug
			//calcScore(new string[] {"dA","hA", "h10"});
			List<int> player1_scores = calcScore (player1_cards);
			List<int> player2_scores = calcScore (player2_cards);

			// judge
			string winner = judge (player1_scores, player2_scores);
			Console.WriteLine ("win : {0}", winner);
			Console.WriteLine ("player1 score : {0}", player1_scores.Max ());
			Console.WriteLine ("player2 score : {0}", player2_scores.Max ());

			// save result
			saveResult (winner);
		}

		/// <summary>
		/// Creates the cards.
		/// cardkinds : c -> clober
		///             s -> spade
		///             d -> dia
		///             h -> heart
		/// </summary>
		/// <param name="cards">Cards Array</param>
		private void createCards (ref string[] cards)
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

		private string[] deal (ref string[] cards)
		{
			string[] ret = new string[2];
			int seed = Environment.TickCount;

			for (int i = 0; i < 2; i++) {
				// create num
				int rndNum;
				do {
					Random rnd = new Random (seed++);
					rndNum = rnd.Next (52);
				} while(cards [rndNum] == "");

				// exchange
				ret [i] = cards [rndNum];
				cards [rndNum] = "";

			}
			return ret;
		}

		private List<int> calcScore (string[] player)
		{
			List<int> scoreList = new List<int> ();
			int cntA = 0;
			foreach (string str in player) {
				// ex.
				// num -> 10  str -> h10
				// num -> A   str -> dA
				string num = str.Substring (1);
				int score = getScore (num);
				if (score == 1)
					cntA++;
				scoreList.Add (getScore (num));
			}
			scoreList.Sort ();

			List<int> ret = createCombi (cntA);
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
		/// 
		/// </summary>
		/// <returns>The score.</returns>
		/// <param name="number">Number.</param>
		private int getScore (string number)
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
		private List<int> createCombi (int cnt)
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

		private string judge (List<int> player1, List<int> player2)
		{
			return player1.Max () > player2.Max () ? "player1" : "player2";
		}

		private void saveResult (string playerName)
		{
			// http://mikeda.hatenablog.com/entry/20090419/1240123566
			// http://okwakatta.net/code/ado21.html
			// http://mysql.stu.edu.tw/doc/refman/5.1/ja/connector-net-examples.html

			string tableName = "player_info";
			//DB接続
			MySqlCommand cmd = new MySqlCommand ();
			MySqlConnection conn = connDB ();
			conn.Open ();
			MySqlTransaction tran = conn.BeginTransaction (); 

			try {
				// select for update

				string sql = String.Format ("select * from {0} where name = '{1}' for update;", tableName, playerName);
				int win_count = SelectWinCount (ref cmd, conn, tran, sql);

				if (win_count == 0) {
					// insert
					sql = String.Format ("insert into {0} values ('{1}',{2});", tableName, playerName, 1);
				} else {
					// update
					sql = String.Format ("update {0} set win_count = {1} where name = '{2}'", tableName, win_count + 1, playerName);
				}
				UpdateWinCount (ref cmd, conn, tran, sql);

				tran.Commit ();
			} catch (Exception e) {
				tran.Rollback ();
				Console.WriteLine (e.Data);
				throw e;
			} finally {
				conn.Close ();
			}

		}

		private MySqlConnection connDB ()
		{
			string connStr = String.Format ("server={0};user id={1};password={2};"
			                 + "database={3}; pooling=false", "localhost", "root", "superuser", "test");
			

			return new MySqlConnection (connStr);
		}

		private int SelectWinCount (ref MySqlCommand cmd, MySqlConnection conn, MySqlTransaction tran, string sql)
		{
			//SQL実行
			cmd.CommandText = sql;
			cmd.Connection = conn;
			cmd.Transaction = tran;
			MySqlDataReader reader = cmd.ExecuteReader ();
			int win_count = 0;
			//テーブル出力
			while (reader.Read ()) {
				win_count = reader.GetInt32 (1);
			}
			reader.Close ();
			return win_count;
		}

		private int UpdateWinCount (ref MySqlCommand cmd, MySqlConnection conn, MySqlTransaction tran, string sql)
		{
			//SQL実行
			cmd.CommandText = sql;
			cmd.Connection = conn;
			cmd.Transaction = tran;
			return cmd.ExecuteNonQuery ();
		}

		static void debugDealAfterState (string[] cards, string[] player1, string[] player2)
		{
			int i = 0;
			foreach (string str in cards) {
				if (i == 13) {
					i = 0;
					Console.WriteLine ();
				}
				Console.Write (String.Format ("{0,4}", str));
				i++;
			}
			Console.WriteLine ();
			Console.WriteLine ("player1 : ");
			foreach (string str in player1) {
				Console.Write (String.Format ("{0,4}", str));
			}
			Console.WriteLine ();
			Console.WriteLine ("player2 : ");
			foreach (string str in player2) {
				Console.Write (String.Format ("{0,4}", str));
			}
			Console.WriteLine ();
		}
	}
}
