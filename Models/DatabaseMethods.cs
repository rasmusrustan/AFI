using Microsoft.CodeAnalysis.Elfie.Diagnostics;
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
            string sqlstring = "SELECT * FROM @boardnr WHERE Game_Id = @gameId";
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
                if (boardPos < 1)
                {
                    boardPos = 0;
                }

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
         * Skapar även spelbräde med skepp
         */
        public int CreateGame(string Player1, string Player2)
        {
            string sqlstring = "INSERT INTO [Game] ([Player1], [Player2]) VALUES (@Player1, @Player2); SELECT SCOPE_IDENTITY();";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Player1", Player1);
            sqlCommand.Parameters.AddWithValue("@Player2", Player2);
            Boolean catched = false;
            int result = -1;

            try
            {
                sqlConnection.Open();
                result = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return Convert.ToInt32(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                catched = true;
                return -1;
            }
            finally
            {
                if (catched)
                {
                    sqlConnection.Close();
                }
                else
                {
                    CreateBoards(result);
                }
            }
        }

        /*
         * Skapar spelbräde för instoppande av skepp
         */
        public void CreateBoards(int gameId)
        {
            string sqlstring1 = "INSERT INTO [Board1] ([Game_Id]) VALUES @gameId";
            string sqlstring2 = "INSERT INTO [Board2] ([Game_Id]) VALUES @gameId";
            SqlCommand sqlCommand1 = new SqlCommand(sqlstring1, sqlConnection);
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand1.Parameters.AddWithValue("@gameId", gameId);

            try
            {
                sqlConnection.Open();

                int i = sqlCommand1.ExecuteNonQuery();
                if (i != 1)
                {
                    Console.WriteLine("Insert command1 failed");
                }
                i = sqlCommand2.ExecuteNonQuery();
                if (i != 1)
                {
                    Console.WriteLine("Insert command2 failed");
                }
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        /*
         * Placerar ut skepp
         */
        private void AddShip(int x, int y, int gameId, int playernumber)
        {
            string boardnumber = "Board1";
            if (playernumber == 2)
            {
                boardnumber = "Board2";
            }
            int firstAvailable = getFirstNullShip(gameId, boardnumber);

            string shipListPosition = "Ship" + firstAvailable;

            string position = "X" + x + " " + "Y" + y;

            string sqlstring = "UPDATE @board SET (@shipListPosition) =  @position WHERE Game_Id = @gameId";

            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@board", boardnumber);
            sqlCommand.Parameters.AddWithValue("@shipListPosition", shipListPosition);
            sqlCommand.Parameters.AddWithValue("@position", position);
            sqlCommand.Parameters.AddWithValue("@gameId", gameId);

            try
            {
                sqlConnection.Open();

                int i = sqlCommand.ExecuteNonQuery();
                if (i != 1)
                {
                    Console.WriteLine("Insert command1 failed");
                }
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private Boolean Shoot(int x, int y, int playerNumber, int gameId)
        {
            string position = "X" + x + " " + "Y" + y;

            string sqlstring1 = "SELECT * FROM @board WHERE Position = @position AND Game_Id = @gameId";

            SqlCommand sqlCommand1 = new SqlCommand(sqlstring1, sqlConnection);
            if (playerNumber == 1)
            {
                sqlCommand1.Parameters.AddWithValue("@board", "Board1");
            }
            else
            {
                sqlCommand1.Parameters.AddWithValue("@board", "Board2");
            }
            sqlCommand1.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand1.Parameters.AddWithValue("@position", position);
            Boolean hit = false;

            try
            {
                sqlConnection.Open();
                int count = (int)sqlCommand1.ExecuteScalar();

                if (count > 0)
                {
                    hit = true; // If the position exists, it's a hit
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., database connection issues)
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Ensure the connection is closed after the query
                sqlConnection.Close();
            }

            string playerName = getPlayerNamefromGame(gameId, playerNumber);
            int hit2 = 0;
            if (hit)
            {
                hit2 = 1;
            }
            string sqlstring2 = "INSERT INTO Shots (Game_Id, Player, Position, Hit) VALUES (@gameId, @playerName, @position; @hitt)";
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand2.Parameters.AddWithValue("@position", position);
            sqlCommand2.Parameters.AddWithValue("@playerName", playerName);
            sqlCommand2.Parameters.AddWithValue("@hitt", hit2);

            try
            {
                sqlConnection.Open();

                int i = sqlCommand2.ExecuteNonQuery();
                if (i != 1)
                {
                    Console.WriteLine("Insert command to Shots failed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            return hit;
        }

        public string getPlayerNamefromGame(int gameId, int playerNumber)
        {
            string sqlstring2 = "SELECT Player1 FROM Game WHERE Game_Id = @gameId";
            if (playerNumber == 2)
            {
                sqlstring2 = "SELECT Player2 FROM Game WHERE Game_Id = @gameId";
            }
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@gameId", gameId);
            string playerName = "NoName";

            try
            {
                sqlConnection.Open();

                object result = sqlCommand2.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    playerName = result.ToString();
                }

                return playerName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return playerName;
            }
            finally
            {
                sqlConnection.Close();
            }
        }


    }
}
