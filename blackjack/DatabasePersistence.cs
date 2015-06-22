using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace blackjack
{
    class DatabasePersistence :ResultPersistence
    {
        private string tableName = "player_info";

        public void Save(string playerName)
        {
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
        private OracleConnection CreateConnection()
        {
            string connStr = String.Format(@"User Id={0}; Password={0}; Data Source="
                                          + "(DESCRIPTION="
                                          + " (LOAD_BALANCE=ON)(FAILOVER=ON)"
                                          + "  (ADDRESS=(PROTOCOL=TCP)(HOST={1})(PORT={3}))"
                                          + "  (ADDRESS=(PROTOCOL=TCP)(HOST={2})(PORT={3}))"
                                          + "  (CONNECT_DATA=(SERVICE_NAME={4}))"
                                          + " )", "test", "192.168.56.111", "192.168.56.112", "1521", "ORCL");
            return new OracleConnection(connStr);
        }

        /// <summary>
        /// 勝利したプレイヤーの勝利数を取得します.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int SelectWinCount(OracleCommand cmd, string sql)
        {
            //SQL実行
            cmd.CommandText = sql;
            OracleDataReader reader = cmd.ExecuteReader();
            int win_count = 0;

            while (reader.Read())
            {
                //win_count = reader.GetInt32(1);
                win_count = int.Parse(reader["win_count"].ToString());
            }
            reader.Close();
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
            return cmd.ExecuteNonQuery();
        }
    }
}
