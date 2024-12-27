using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Data;
using System.Data.SqlTypes;
using System.Linq.Expressions;

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

                boardNumber2 = Convert.ToInt32(sqlCommand2.ExecuteScalar());
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

            /*int BoardNr1 = 3;
            int BoardNr2 = 3;*/

            updateBoardNumber(getBoardNumberFromBoards(1,gameId), getBoardNumberFromBoards(2,gameId), gameId);
            return;
        }

        public int getBoardNumberFromBoards(int board, int gameId)
        {
            string boardNumber = "Board" + board;
            string sqlstring1 = "SELECT (Id) FROM " + boardNumber + " WHERE (Game_Id) = @gameId";
            string sqlstring2 = "INSERT INTO Board2 (Game_Id) VALUES (@gameId)";
            SqlCommand sqlCommand1 = new SqlCommand(sqlstring1, sqlConnection);
            sqlCommand1.Parameters.AddWithValue("@gameId", Convert.ToInt32(gameId));
            int boardId = -3;

            try
            {
                sqlConnection.Open();

                object result = sqlCommand1.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    boardId = Convert.ToInt32(result);
                }

                return boardId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return boardId;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        /*
         * Placerar ut skepp
         */
        public void AddShip(int x, int y, int gameId, int playernumber, string shipType)
        {
            string boardnumber = "Board1";
            string position = "X" + x + " " + "Y" + y;
            if (playernumber == 2)
            {
                boardnumber = "Board2";
            }

            int boardId = getBoardNumberFromBoards(playernumber, gameId);
            int numberOfShips = -1;
            int numberOfPositions = -1;

            if (shipType == "DoubleShip")
            {
                numberOfShips = 4;
                numberOfPositions = 2;
            }
            if (shipType == "TripleShip")
            {
                numberOfShips = 3;
                numberOfPositions = 3;
            }
            if (shipType == "LongShip")
            {
                numberOfShips = 2;
                numberOfPositions = 4;
            }
            if (shipType == "Titanic")
            {
                numberOfShips = 1;
                numberOfPositions = 5;
            }

            for (int k = 0; k < numberOfShips; k++)
            {
                string shipTypeAndNumber = shipType + (k + 1);
                if (shipType == "Titanic")
                {
                    shipTypeAndNumber = "Titanic";
                }

                if (doesShipExist(getBoardNumber(gameId, playernumber), playernumber, shipTypeAndNumber))
                {
                    int shipNumber = getShipIdentity(playernumber, boardId, shipTypeAndNumber);
                    int firstpos = -1;
                    for (int i = 0; i < numberOfPositions; i++)
                    {
                        string pos = "Pos" + (i+1);
                        if (!getOccupancyOfShipPos(shipType, shipNumber, pos))
                        {
                            firstpos = i + 1;
                            break;
                        }
                    }

                    // If firstpos still -1 all ship positions taken
                    if (firstpos != -1)
                    {
                        string shipTable = shipType + "s";
                        
                        string sqlstring = "UPDATE " + shipTable + " SET Pos" + firstpos + " = @position WHERE Id = @shipNumber";

                        SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
                        sqlCommand.Parameters.AddWithValue("@shipNumber", shipNumber);
                        sqlCommand.Parameters.AddWithValue("@position", position);

                        try
                        {
                            sqlConnection.Open();

                            int i = sqlCommand.ExecuteNonQuery();
                            if (i != 1)
                            {
                                Console.WriteLine("Insert command shipPos failed");
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
                else
                {
                    int scalarInt = -1;
                    Boolean catched = false;
                    string shipTable = shipType + "s";
                    string sqlstring = "INSERT INTO " + shipTable + " (Pos1) VALUES (@position); SELECT SCOPE_IDENTITY();";
                    SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@position", position);

                    try
                    {
                        sqlConnection.Open();

                        object scalar = sqlCommand.ExecuteScalar();
                        scalarInt = Convert.ToInt32(scalar);
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        catched = true;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }

                    if (!catched)
                    {
                        int shipId = scalarInt;
                        sqlstring = "UPDATE " + boardnumber + " SET " + shipTypeAndNumber + " = @shipId";
                        SqlCommand sqlCommand3 = new SqlCommand(sqlstring, sqlConnection);
                        sqlCommand3.Parameters.AddWithValue("@shipId", shipId);
                        

                        try
                        {
                            sqlConnection.Open();

                            int i = sqlCommand3.ExecuteNonQuery();
                            if (i != 1)
                            {
                                Console.WriteLine("Insert command shipPos failed");
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
        }

        public int getShipIdentity(int boardNumber, int boardId, string shipType)
        {
            int scalarInt = -1;
            string boardString = "Board" + boardNumber;
            string sqlstring = "SELECT " + shipType + " FROM " + boardString + " WHERE (Id) = @boardId";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@boardId", boardId);

            try
            {
                sqlConnection.Open();

                object scalar = sqlCommand.ExecuteScalar();
                scalarInt = Convert.ToInt32(scalar);
                return scalarInt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return scalarInt;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public Boolean getOccupancyOfShipPos(string shipType, int shipId, string position)
        {
            string shipString = shipType + "s";
            string sqlstring2 = "SELECT (" + position + ") FROM " + shipString + " WHERE (Id) = (@shipId)";
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@shipId", shipId);
            string playerName = "NoName";

            try
            {
                sqlConnection.Open();

                object result = sqlCommand2.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
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
            string sqlstring2 = "SELECT (Player1) FROM Game WHERE (Id) = (@gameId)";
            if (playerNumber == 2)
            {
                sqlstring2 = "SELECT (Player2) FROM Game WHERE (Id) = (@gameId)";
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
        public Boolean checkIfEmpty(int[,] board, string orientation, int length, int x, int y)
        {
            if (orientation == "hor")
            {
                for (int i = 0; i < length; i++)
                {
                    if (board[x+i,y] == 1)
                    {
                        return false;
                    }
                }
            }

            if (orientation == "vert")
            {
                for (int i = 0; i < length; i++)
                {
                    if (board[x + i, y] == 1)
                    {
                        return false;
                    }
                }
            }

            return true;
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
            string sqlstring2 = "UPDATE Game SET Board1 = @number WHERE Id = @gameId";
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@gameId", gameId);
            sqlCommand2.Parameters.AddWithValue("@number", number1);

            string sqlstring3 = "UPDATE Game SET Board2 = @number WHERE Id = @gameId";
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

        //Returnerar ett spelbräde
        public int[,] getBoard(int gameId, int boardNumber)
        {
            Boolean titanicExists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "Titanic");
            Boolean longShip1Exists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "LongShip1");
            Boolean longShip2Exists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "LongShip2");
            Boolean trippleShip1Exists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "TrippleShip1");
            Boolean trippleShip2Exists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "TrippleShip2");
            Boolean trippleShip3Exists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "TrippleShip3");
            Boolean doubleShip1Exists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "DoubleShip1");
            Boolean doubleShip2Exists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "DoubleShip2");
            Boolean doubleShip3Exists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "DoubleShip3");
            Boolean doubleShip4Exists = doesShipExist(getBoardNumber(gameId, boardNumber), boardNumber, "DoubleShip4");

            // Sätt alla värden till 0
            int[,] board1 = new int[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    board1[i,j] = 0;
                }
            }

            if (titanicExists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "Titanic");
                List<string> positions = getShipPositions(shipNumber, "Titanic");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }

            if (longShip1Exists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "LongShip1");
                List<string> positions = getShipPositions(shipNumber, "LongShip");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }

            if (longShip2Exists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "LongShip2");
                List<string> positions = getShipPositions(shipNumber, "LongShip");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }

            if (trippleShip1Exists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "TrippleShip1");
                List<string> positions = getShipPositions(shipNumber, "TrippleShip");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }

            if (trippleShip2Exists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "TrippleShip2");
                List<string> positions = getShipPositions(shipNumber, "TrippleShip");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }

            if (trippleShip3Exists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "TrippleShip3");
                List<string> positions = getShipPositions(shipNumber, "TrippleShip");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }
            
            if (doubleShip1Exists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "DoubleShip1");
                List<string> positions = getShipPositions(shipNumber, "DoubleShip");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }

            if (doubleShip2Exists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "DoubleShip2");
                List<string> positions = getShipPositions(shipNumber, "DoubleShip");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }

            if (doubleShip3Exists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "DoubleShip3");
                List<string> positions = getShipPositions(shipNumber, "DoubleShip");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }

            if (doubleShip4Exists)
            {
                int shipNumber = getShipNumber(getBoardNumber(gameId, boardNumber), boardNumber, "DoubleShip4");
                List<string> positions = getShipPositions(shipNumber, "DoubleShip");

                for (int i = 0; i < positions.Count; i++)
                {
                    string positionString = positions[i];
                    string[] split = positionString.Split(' ');

                    int x = int.Parse(split[0].Substring(1));
                    int y = int.Parse(split[1].Substring(1));

                    board1[x, y] = 1;
                }
            }

            return board1;
        }

        //Returnerar spelbrädes id för en bräda
        public int getBoardNumber(int gameId, int boardNumber)
        {
            string board = "Board" + boardNumber;
            string sqlstring2 = "SELECT (Id) FROM " +board+ " WHERE (Game_Id) = @gameId";
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@gameId", gameId);

            try
            {
                sqlConnection.Open();
                object number = sqlCommand2.ExecuteScalar();
                return Convert.ToInt32(number);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public Boolean doesShipExist(int board, int boardNumber, string shipType)
        {
            string boardType = "Board" + boardNumber;
            
            string sqlstring2 = "SELECT * FROM " +boardType+ " WHERE (Id) = @board AND ("+shipType+") IS NOT NULL";

            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@board", board);

            Boolean exists = false;
            int hits = 0;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand2);
            DataSet dataSet = new DataSet();

            try
            {
                sqlConnection.Open();

                sqlDataAdapter.Fill(dataSet, "Ships");

                hits = dataSet.Tables["Ships"].Rows.Count;

                if (hits > 0)
                {
                    exists = true;
                }

                return exists;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }

        }

        // Returnerar en lista med strings på alla positioner skeppet har
        public List<string> getShipPositions(int shipNumber, string shipType)
        {
            List<string> positions = new List<string>();
            string ships = shipType + "s";
            string sqlstring2 = "SELECT * FROM " + ships + " WHERE (Id) = @shipNumber";
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@shipNumber", shipNumber);

            try
            {
                sqlConnection.Open();
                using (SqlDataReader reader = sqlCommand2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            positions.Add(reader.GetValue(i).ToString());
                        }
                    }
                }
                if (positions.Count > 0)
                {
                    positions.RemoveAt(positions.Count - 1);
                    positions.RemoveAt(0);
                }
                return positions;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public int getShipNumber(int board, int boardNumber, string shipTypeNr)
        {
            string boardType = "Board" + boardNumber;
            string sqlstring2 = "SELECT (@shipTypeNr) FROM @boardType WHERE (Id) = @board";
            SqlCommand sqlCommand2 = new SqlCommand(sqlstring2, sqlConnection);
            sqlCommand2.Parameters.AddWithValue("@shipTypeNr", shipTypeNr);
            sqlCommand2.Parameters.AddWithValue("@boardType", boardType);
            sqlCommand2.Parameters.AddWithValue("@board", board);

            try
            {
                sqlConnection.Open();
                SqlDataReader reader = sqlCommand2.ExecuteReader();
                return reader.GetInt32(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
