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
            ConnectionString = "Data Source=battleshipsserver.database.windows.net;Initial Catalog=Battleships;User ID=sqladmin;Password=Skola123;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
        };

        // Publika metoder

        /*
         * Hämtar första lediga skepp plats
         */
        public int getFirstNullShip(int gameId, string boardnr)
        {
            string sqlstring = "SELECT * FROM (@boardnr) WHERE (Game_Id) = (@gameId)";
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
            string sqlstring = "INSERT INTO [Game] ([Player1], [Player2], [NextPlayer]) VALUES (@Player1, @Player2, 1); SELECT SCOPE_IDENTITY();";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Player1", Player1);
            sqlCommand.Parameters.AddWithValue("@Player2", Player2);
            Boolean catched = false;
            int result = -1;

            try
            {
                sqlConnection.Open();
                result = Convert.ToInt32(sqlCommand.ExecuteScalar());
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                catched = true;
                return -1;
            }
            finally
            {
                sqlConnection.Close();
            }
            if (!catched)
            {
                CreateBoards(result);
            }
            
            return Convert.ToInt32(result);
        }

        /*
         * Skapar spelbräde för instoppande av skepp
         */
        public void CreateBoards(int gameId)
        {
            string sqlstring1 = "INSERT INTO Board1 (Game_Id) VALUES (@gameId)";
            string sqlstring2 = "INSERT INTO Board2 (Game_Id) VALUES (@gameId)";
            SqlCommand sqlCommand1 = new SqlCommand(sqlstring1, sqlConnection);
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand1.Parameters.AddWithValue("@gameId", Convert.ToInt32(gameId));
            sqlCommand2.Parameters.AddWithValue("@gameId", Convert.ToInt32(gameId));

            int boardNumber1 = -1;
            int boardNumber2 = -1;

            try
            {
                sqlConnection.Open();

                boardNumber1 = Convert.ToInt32(sqlCommand1.ExecuteScalar());

                if (boardNumber1 == -1)
                {
                    Console.WriteLine("Insert command1 failed");
                }

                boardNumber2 = Convert.ToInt32(sqlCommand1.ExecuteScalar());
                if (boardNumber2 == -1)
                {
                    Console.WriteLine("Insert command2 failed");
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

            updateBoardNumber(boardNumber1, boardNumber2, gameId);
            return;
        }

        /*
         * Placerar ut skepp
         */
        public void AddShip(int x, int y, int gameId, int playernumber)
        {
            string boardnumber = "Board1";
            if (playernumber == 2)
            {
                boardnumber = "Board2";
            }
            int firstAvailable = getFirstNullShip(gameId, boardnumber);

            string shipListPosition = "Ship" + firstAvailable;

            string position = "X" + x + " " + "Y" + y;

            string sqlstring = "UPDATE @board SET (@shipListPosition) =  (@position) WHERE (Game_Id) = (@gameId)";

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

        /*
         * Skjutförfarande, returnerar true/false på träff
         */
        public Boolean Shoot(int x, int y, int playerNumber, int gameId)
        {
            string position = "X" + x + " " + "Y" + y;

            string sqlstring1 = "SELECT * FROM @board WHERE (Position) = @position AND (Game_Id) = @gameId";

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
            string sqlstring2 = "INSERT INTO Shots (Game_Id, Player, Position, Hit) VALUES (@gameId, @playerName, @position, @hitt)";
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

            if (!hit) // Uppdatera nextplayer om miss
            {
                updateNextPlayer(gameId);
            }
            return hit;
        }

        /*
         * Returnerar spelarnamn
         */
        public string getPlayerNamefromGame(int gameId, int playerNumber)
        {
            string sqlstring2 = "SELECT (Player1) FROM Game WHERE (Game_Id) = @gameId";
            if (playerNumber == 2)
            {
                sqlstring2 = "SELECT (Player2) FROM Game WHERE (Game_Id) = @gameId";
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

        public int getNumberOfHits(int gameId, int playerNumber)
        {
            string playerName = getPlayerNamefromGame(gameId,playerNumber);
            string sqlstring = "SELECT (Id) FROM Shots WHERE (Game_Id) = @gameId AND (Hits) = 1 AND (Player) = @playerName";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand.Parameters.AddWithValue("@playerName", playerName);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();
            int hits = -1;

            try
            {
                sqlConnection.Open();

                sqlDataAdapter.Fill(dataSet, "Shots");

                hits = dataSet.Tables["Shots"].Rows.Count;

                if (hits > 29)
                {
                    declareWinner(gameId, playerName);
                }

                return hits;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return hits;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        /*
         * Updaterar vinnare och sätter game finished till 1
         */
        public void declareWinner(int gameId, string playerName)
        {
            string sqlstring1 = "UPDATE Game SET (Winner) = @playerName WHERE (Game_Id) = @gameId";
            SqlCommand sqlCommand1 = new SqlCommand(sqlstring1, sqlConnection);
            sqlCommand1.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand1.Parameters.AddWithValue("@playerName", playerName);

            string sqlstring2 = "UPDATE Game SET (GameFinished) = 1 WHERE (Game_Id) = @gameId";
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@gameId", gameId);

            try
            {
                sqlConnection.Open();

                int i = sqlCommand1.ExecuteNonQuery();
                if (i != 1)
                {
                    Console.WriteLine("Insert command playername failed");
                }

                int j = sqlCommand2.ExecuteNonQuery();
                if (j != 1)
                {
                    Console.WriteLine("Insert command gameFinished failed");
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
         * Uppdaterar Nextplayer i Game, om 1 sätts till 2 ochvise versa
         */
        public void updateNextPlayer(int gameId)
        {
            int nextPlayer = getNextPlayer(gameId);
            if (nextPlayer == 1)
            {
                nextPlayer = 2;
            }
            else
            {
                nextPlayer = 1;
            }

            string sqlstring2 = "UPDATE Game SET (NextPlayer) = @nextPlayer WHERE (Game_Id) = @gameId";
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand2.Parameters.AddWithValue("@nextPlayer", nextPlayer);

            try
            {
                sqlConnection.Open();

                int j = sqlCommand2.ExecuteNonQuery();
                if (j != 1)
                {
                    Console.WriteLine("Update command nextPlayer failed");
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
         * Hämtar NextPlayer från Game
         */
        public int getNextPlayer(int gameId)
        {
            string sqlstring = "SELECT (NextPlayer) FROM Game WHERE (Game_Id) = @gameId";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@gameId", gameId);

            try
            {
                sqlConnection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                return reader.GetInt32(0);
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

        /*
         * Kontrollerar om skepp finns
         */
        public int checkIfEmpty(int boardnumber, int x, int y, string orientation, int length, int gameId)
        {
            Boolean occupied = false;

            // Loop through the length of the ship being placed
            for (int loop = 0; loop < length; loop++)
            {
                string position;
                string sqlstring1;

                // Construct position based on orientation
                if (orientation == "hor")
                {
                    position = "X" + (x + loop) + " Y" + y;
                    // Dynamically set the table based on boardnumber
                    sqlstring1 = "SELECT COUNT(*) FROM " + (boardnumber == 1 ? "Board1" : "Board2") +
                                 " WHERE Position = @position AND Game_Id = @gameId";
                }
                else
                {
                    position = "X" + x + " Y" + (y + loop);
                    // Dynamically set the table based on boardnumber
                    sqlstring1 = "SELECT COUNT(*) FROM " + (boardnumber == 1 ? "Board1" : "Board2") +
                                 " WHERE Position = @position AND Game_Id = @gameId";
                }

                SqlCommand sqlCommand1 = new SqlCommand(sqlstring1, sqlConnection);
                sqlCommand1.Parameters.AddWithValue("@position", position);
                sqlCommand1.Parameters.AddWithValue("@gameId", gameId);

                try
                {
                    sqlConnection.Open();
                    int count = (int)sqlCommand1.ExecuteScalar();

                    // If the position exists, it's occupied
                    if (count > 0)
                    {
                        occupied = true;
                    }

                    // Break out of the loop if any position is occupied
                    if (occupied)
                    {
                        break;
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
            }

            // If occupied, return 0, else return 1
            if (occupied)
            {
                return 0;
            }
            else
            {
                return 1;
            }
            
        }

            /*
             * Kontrollerar om ett skott finns och om det är en träff eller ej
             */
        public int checkShots(int playerNumber, int gameId, int x, int y)
        {
            string playerName = getPlayerNamefromGame(gameId, playerNumber);
            string position = "X" + x + " " + "Y" + y;

            string sqlstring1 = "SELECT (Id) FROM Shots WHERE (Position) = @position AND (Game_Id) = @gameId AND (Player) = @playerName";

            SqlCommand sqlCommand1 = new SqlCommand(sqlstring1, sqlConnection);
            sqlCommand1.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand1.Parameters.AddWithValue("@position", position);
            sqlCommand1.Parameters.AddWithValue("@playerName", playerName);
            Boolean thereIsAShot = false;

            try
            {
                sqlConnection.Open();
                int count = (int)sqlCommand1.ExecuteScalar();

                if (count > 0)
                {
                    thereIsAShot = true;
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

            string sqlstring2 = "SELECT (Id) FROM Shots WHERE (Position) = @position AND (Game_Id) = @gameId AND (Player) = @playerName AND (Hit) = 1";

            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand2.Parameters.AddWithValue("@position", position);
            sqlCommand2.Parameters.AddWithValue("@playerName", playerName);
            Boolean hit = false;

            try
            {
                sqlConnection.Open();
                int count = (int)sqlCommand2.ExecuteScalar();

                if (count > 0)
                {
                    hit = true;
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

            if (thereIsAShot && hit)
            {
                return 2;
            }
            else if (thereIsAShot && (hit == false))
            {
                return 3;
            }
            else
            {
                return 1;
            }
        }

        public void updateBoardNumber(int number1, int number2, int gameId)
        {
            string sqlstring2 = "UPDATE Game SET (Board1) = @number WHERE (Id) = @gameId";
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand2.Parameters.AddWithValue("@number", number1);

            string sqlstring3 = "UPDATE Game SET (Board2) = @number WHERE (Id) = @gameId";
            SqlCommand sqlCommand3 = new SqlCommand(sqlstring3, sqlConnection);
            sqlCommand3.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand3.Parameters.AddWithValue("@number", number2);

            try
            {
                sqlConnection.Open();

                int j = sqlCommand2.ExecuteNonQuery();
                if (j != 1)
                {
                    Console.WriteLine("Update command board1 failed");
                }

                int k = sqlCommand3.ExecuteNonQuery();
                if (k != 1)
                {
                    Console.WriteLine("Update command board2 failed");
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
    }
}
