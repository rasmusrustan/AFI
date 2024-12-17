using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace BattleShits.Models
{
    public class DatabaseMethods
    {
        //Konstruktor
        public DatabaseMethods() { }

        SqlConnection sqlConnection = new SqlConnection
        {
            ConnectionString = "Data Source=battleshipsserver.database.windows.net;Initial Catalog=Battleships;User ID=sqladmin;Password=********;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
        };

        // Publika metoder

        /*
         * Hämtar första lediga skepp plats
         */
        public int getFirstNullShip(int gameId, string boardnr)
        {
            string sqlstring = "SELECT * FROM @boardnr WHERE Game_Id == @gameId";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand.Parameters.AddWithValue("@boardnr", boardnr);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            int boardPos = 0;
            DataSet dataSet = new DataSet();

            try
            {
                sqlConnection.Open();

                sqlDataAdapter.Fill(dataSet, "Game");

                boardPos = dataSet.Tables["Game"].Rows.Count;

                return boardPos + 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return boardPos;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        /*
         * Skapar spel och returnerar det skapade id:t
         * Stoppar även in spelare i spelet
         */
        public int CreateGame(string Player1, string Player2)
        {
            string sqlstring = "INSERT INTO [Game] ([Player1], [Player2]) VALUES (@Player1, @Player2); SELECT SCOPE_IDENTITY();";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Player1", Player1);
            sqlCommand.Parameters.AddWithValue("@Player2", Player2);

            try
            {
                sqlConnection.Open();
                var result = sqlCommand.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
            finally
            {
                sqlConnection.Close();
            }
        }




    }
}
