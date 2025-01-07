using BattleShits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace BattleShits.Controllers
{
    [AllowAnonymous]
    public class BattleField2Controller : Controller
    {
        private static DatabaseMethods database = new DatabaseMethods();

        [HttpGet]
        public IActionResult DeclareWinner(int gameId, string playerName)
        {
            try
            {
                // Anropa den redan existerande declareWinner-metoden
                database.declareWinner(gameId, playerName);

                return Json(new { success = true, message = $"Spelare {playerName} har vunnit!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult AddShipP1(bool firstStart, int gameNumber, string message)
        {

            int gameId = gameNumber;
            if (firstStart == true)
            {
                gameId = database.CreateGame("P1", "P2");
            }
            
            
            int[,] p1Board = database.getBoard(gameId,1);
            string playerName = database.getPlayerNamefromGame(gameId, 1);
            bool readyToBattle = false;

            // Check if ships can be blaced
            bool titanicExists = database.doesShipExist(database.getBoardNumber(gameId, 1), 1, "Titanic");
            bool longShip2Exists = database.doesShipExist(database.getBoardNumber(gameId, 1), 1, "LongShip2");
            bool trippleShip3Exists = database.doesShipExist(database.getBoardNumber(gameId, 1), 1, "TrippleShip3");
            bool doubleShip4Exists = database.doesShipExist(database.getBoardNumber(gameId, 1), 1, "DoubleShip4");

            

            if (titanicExists && longShip2Exists && trippleShip3Exists && doubleShip4Exists)
            {
                database.updateBoardReady(gameId, 1);
            }

            readyToBattle = database.isGameSetup(gameId);

            ViewBag.TitanicExists = titanicExists;
            ViewBag.Longship2Exists = longShip2Exists;
            ViewBag.Trippleship3Exists = trippleShip3Exists;
            ViewBag.Doubleship4Exists = doubleShip4Exists;

            ViewBag.GameId = gameId;
            ViewBag.PlayerName = playerName;
            ViewBag.p1Board = p1Board;
            ViewBag.Message = message;
            ViewBag.readyToBattle = readyToBattle;
            

            return View("~/Views/Battle2/AddShipP1.cshtml", database);
        }
        public IActionResult AddShipP2(bool firstStart, int gameNumber, string message)
        {

            int gameId = database.getHighestGameNumber();

            int[,] p2Board = database.getBoard(gameId, 2);
            string playerName = database.getPlayerNamefromGame(gameId, 2);
            bool readyToBattle = false;

            // Check if ships can be blaced
            bool titanicExists = database.doesShipExist(database.getBoardNumber(gameId, 2), 2, "Titanic");
            bool longShip2Exists = database.doesShipExist(database.getBoardNumber(gameId, 2), 2, "LongShip2");
            bool trippleShip3Exists = database.doesShipExist(database.getBoardNumber(gameId, 2), 2, "TrippleShip3");
            bool doubleShip4Exists = database.doesShipExist(database.getBoardNumber(gameId, 2), 2, "DoubleShip4");

            if (titanicExists && longShip2Exists && trippleShip3Exists && doubleShip4Exists)
            {
                database.updateBoardReady(gameId, 2);
            }

            readyToBattle = database.isGameSetup(gameId);

            ViewBag.TitanicExists = titanicExists;
            ViewBag.Longship2Exists = longShip2Exists;
            ViewBag.Trippleship3Exists = trippleShip3Exists;
            ViewBag.Doubleship4Exists = doubleShip4Exists;

            ViewBag.GameId = gameId;
            ViewBag.PlayerName = playerName;
            ViewBag.p2Board = p2Board;
            ViewBag.Message = message;
            ViewBag.readyToBattle = readyToBattle;

            return View("~/Views/Battle2/AddShipP2.cshtml", database);
        }

        public IActionResult SeaBattle(int gameId, string message, bool gameOver)
        {
            int[,] p1Board = database.getBoard(gameId, 1);
            int[,] p2Board = database.getBoard(gameId, 2);
            int nextPlayer = database.getNextPlayer(gameId);
            

            // Kontrollera om spelet är slut
            if (!gameOver)
            {
                if (database.checkIfGameOver(p1Board, p2Board))
                {
                    gameOver = true;
                }
            }

            ViewBag.p1Board = p1Board;
            ViewBag.p2Board = p2Board;
            ViewBag.message = message;
            ViewBag.gameId = gameId;
            ViewBag.p2BoardJson = JsonConvert.SerializeObject(p2Board);
            ViewBag.nextPlayer = nextPlayer;
            ViewBag.gameOver = gameOver;

            if (!gameOver)
            {
                return View("~/Views/Battle2/SeaBattle.cshtml", database);
            }
            else
            {
                return RedirectToAction("Result", new { gameId = gameId });
            }
        }
        public IActionResult SeaBattle2(int gameId, string message, bool gameOver)
        {
            int[,] p1Board = database.getBoard(gameId, 1);
            int[,] p2Board = database.getBoard(gameId, 2);
            int nextPlayer = database.getNextPlayer(gameId);

            // Kontrollera om spelet är slut
            if (!gameOver)
            {
                if (database.checkIfGameOver(p1Board, p2Board))
                {
                    gameOver = true;
                }
            }

            ViewBag.p1Board = p1Board;
            ViewBag.p2Board = p2Board;
            ViewBag.message = message;
            ViewBag.gameId = gameId;
            ViewBag.p1BoardJson = JsonConvert.SerializeObject(p1Board);
            ViewBag.nextPlayer = nextPlayer;
            ViewBag.gameOver = gameOver;

            if (!gameOver)
            {
                return View("~/Views/Battle2/SeaBattle2.cshtml", database);
            }
            else
            {
                return RedirectToAction("Result", new { gameId = gameId });
            }
        }
        public IActionResult Result(int gameId)
        {
            List<Shot> p1Shots = database.getShotsFromGame(gameId, 1);
            List<Shot> p2Shots = database.getShotsFromGame(gameId, 2);
            string winner = database.getWinner(gameId);
            int p1ShotCount = p1Shots.Count;
            int p2ShotCount = p2Shots.Count;
            int p1HitCount = 0;
            int p2HitCount = 0;

            // Räkna träffar
            for (int i = 0; i < p1Shots.Count; i++)
            {
                if (p1Shots[i].hit)
                {
                    p1HitCount++;
                }
            }
            for (int i = 0; i < p2Shots.Count; i++)
            {
                if (p2Shots[i].hit)
                {
                    p2HitCount++;
                }
            }

            ViewBag.winner = winner;
            ViewBag.gameId = gameId;
            ViewBag.p1ShotCount = p1ShotCount;
            ViewBag.p2ShotCount = p2ShotCount;

            return View("~/Views/Battle2/Result.cshtml", database);
        }

        [HttpPost]
        public IActionResult PlaceDoubleShip(string user, string orientation, int x, int y, int gameId)
        {
            int length = 2;
            
            int playerNumber = -1;
            if (user == database.getPlayerNamefromGame(gameId,1))
            {
                playerNumber = 1;
            }
            if (user == database.getPlayerNamefromGame(gameId,2))
            {
                playerNumber = 2;
            }

            // Validera att skeppet kan placeras utan att gå utanför brädet
            int boardSize = 10;
            if (orientation == "hor")
            {
                if (x >= 0 && x + length - 1 < boardSize && y >= 0 && y < boardSize && (database.checkIfEmpty(database.getBoard(gameId, playerNumber), orientation, length, x, y)))
                {
                    for (int i = 0; i < length; i++)
                    {
                        database.AddShip((x + i), y, gameId, playerNumber, "DoubleShip");
                    }
                }
                else
                {
                    string message = "Ogiltig placering för horizontellt skepp.";
                    if (playerNumber == 1)
                    {
                        return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                    else
                    {
                        return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < boardSize && x >= 0 && x < boardSize && (database.checkIfEmpty(database.getBoard(gameId, playerNumber), orientation, length, x, y)))
                {
                    for (int i = 0; i < length; i++)
                    {
                        database.AddShip(x, (y + i), gameId, playerNumber, "DoubleShip");
                    }
                }
                else
                {
                    string message = "Ogiltig placering för vertikalt skepp.";
                    if (playerNumber == 1)
                    {
                        return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                    else
                    {
                        return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                }
            }
            else
            {
                string message = "Ogiltig orientering.";
                if (playerNumber == 1)
                {
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                }
                else
                {
                    return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                }
            }

            if (playerNumber == 1)
            {
                string message = "";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }
            else
            {
                string message = "";
                return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
            }
        }
        public IActionResult PlaceTrippleShip(string user, string orientation, int x, int y, int gameId)
        {
            int length = 3;

            int playerNumber = -1;
            if (user == database.getPlayerNamefromGame(gameId, 1))
            {
                playerNumber = 1;
            }
            if (user == database.getPlayerNamefromGame(gameId, 2))
            {
                playerNumber = 2;
            }

            // Validera att skeppet kan placeras utan att gå utanför brädet
            int boardSize = 10;
            if (orientation == "hor")
            {
                if (x >= 0 && x + length - 1 < boardSize && y >= 0 && y < boardSize && (database.checkIfEmpty(database.getBoard(gameId, playerNumber), orientation, length, x, y)))
                {
                    for (int i = 0; i < length; i++)
                    {
                        database.AddShip((x + i), y, gameId, playerNumber, "TrippleShip");
                    }
                }
                else
                {
                    string message = "Ogiltig placering för horizontellt skepp.";
                    if (playerNumber == 1)
                    {
                        return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                    else
                    {
                        return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < boardSize && x >= 0 && x < boardSize && (database.checkIfEmpty(database.getBoard(gameId, playerNumber), orientation, length, x, y)))
                {
                    for (int i = 0; i < length; i++)
                    {
                        database.AddShip(x, (y + i), gameId, playerNumber, "TrippleShip");
                    }
                }
                else
                {
                    string message = "Ogiltig placering för vertikalt skepp.";
                    if (playerNumber == 1)
                    {
                        return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                    else
                    {
                        return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                }
            }
            else
            {
                string message = "Ogiltig orientering.";
                if (playerNumber == 1)
                {
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                }
                else
                {
                    return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                }
            }

            if (playerNumber == 1)
            {
                string message = "";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }
            else
            {
                string message = "";
                return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
            }
        }
        public IActionResult PlaceLongShip(string user, string orientation, int x, int y, int gameId)
        {
            int length = 4;

            int playerNumber = -1;
            if (user == database.getPlayerNamefromGame(gameId, 1))
            {
                playerNumber = 1;
                
            }
            if (user == database.getPlayerNamefromGame(gameId, 2))
            {
                playerNumber = 2;
            }

            // Validera att skeppet kan placeras utan att gå utanför brädet
            int boardSize = 10;
            if (orientation == "hor")
            {
                if (x >= 0 && x + length - 1 < boardSize && y >= 0 && y < boardSize && (database.checkIfEmpty(database.getBoard(gameId, playerNumber), orientation, length, x, y)))
                {
                    for (int i = 0; i < length; i++)
                    {
                        database.AddShip((x + i), y, gameId, playerNumber, "LongShip");
                    }
                }
                else
                {
                    string message = "Ogiltig placering för horizontellt skepp.";
                    if (playerNumber == 1)
                    {
                        return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                    else
                    {
                        return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < boardSize && x >= 0 && x < boardSize && (database.checkIfEmpty(database.getBoard(gameId, playerNumber), orientation, length, x, y)))
                {
                    for (int i = 0; i < length; i++)
                    {
                        database.AddShip(x, (y + i), gameId, playerNumber, "LongShip");
                    }
                }
                else
                {
                    string message = "Ogiltig placering för vertikalt skepp.";
                    if (playerNumber == 1)
                    {
                        return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                    else
                    {
                        return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                }
            }
            else
            {
                string message = "Ogiltig orientering.";
                if (playerNumber == 1)
                {
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                }
                else
                {
                    return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                }
            }

            if (playerNumber == 1)
            {
                string message = "";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }
            else
            {
                string message = "";
                return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
            }
        }
        public IActionResult PlaceTitanic(string user, string orientation, int x, int y, int gameId)
        {
            int length = 5;

            int playerNumber = -1;
            if (user == database.getPlayerNamefromGame(gameId, 1))
            {
                playerNumber = 1;
            }
            if (user == database.getPlayerNamefromGame(gameId, 2))
            {
                playerNumber = 2;
            }

            // Validera att skeppet kan placeras utan att gå utanför brädet
            int boardSize = 10;
            if (orientation == "hor")
            {
                if (x >= 0 && x + length - 1 < boardSize && y >= 0 && y < boardSize && (database.checkIfEmpty(database.getBoard(gameId, playerNumber), orientation, length, x, y)))
                {
                    for (int i = 0; i < length; i++)
                    {
                        database.AddShip((x + i), y, gameId, playerNumber, "Titanic");
                    }
                }
                else
                {
                    string message = "Ogiltig placering för horizontellt skepp.";
                    if (playerNumber == 1)
                    {
                        return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                    else
                    {
                        return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < boardSize && x >= 0 && x < boardSize && (database.checkIfEmpty(database.getBoard(gameId, playerNumber), orientation, length, x, y)))
                {
                    for (int i = 0; i < length; i++)
                    {
                        database.AddShip(x, (y + i), gameId, playerNumber, "Titanic");
                    }
                }
                else
                {
                    string message = "Ogiltig placering för vertikalt skepp.";
                    if (playerNumber == 1)
                    {
                        return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                    else
                    {
                        return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                    }
                }
            }
            else
            {
                string message = "Ogiltig orientering.";
                if (playerNumber == 1)
                {
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                }
                else
                {
                    return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
                }
            }

            if (playerNumber == 1)
            {
                string message = "";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }
            else
            {
                string message = "";
                return RedirectToAction("AddShipP2", new { firstStart = false, gameNumber = gameId, message = message });
            }
        }


        
        public IActionResult Fire(int playerNumber, int x, int y, int gameNumber, string boardString)
        {
            string message = "";
            bool newShot = false;
            bool hit = false;
            int[,] board = JsonConvert.DeserializeObject<int[,]>(boardString);

            // Kontrollera om nytt skott och om träff
            if (board[x,y] == 1)
            {
                // träff
                message = "TRÄFF!!";
                newShot = true;
                hit = true;
            }
            else if ((board[x, y] == 2) || (board[x, y] == 3) || (board[x, y] == 4))
            {
                // Finns redan skott där
                message = "Det finns redan ett skott där";
            }
            else
            {
                // Miss
                message = "miss";
                newShot = true;
            }

            // Lägger till skott i databasen och uppdaterar eventuellt nextPlayer
            if (newShot)
            {
                database.Shoot(x, y, playerNumber, gameNumber, hit);
                if (!hit)
                {
                    database.updateNextPlayer(gameNumber);
                }
            }

            // Kontrollera om alla motståndarskepp är sänkta
            bool gameOver = false;
            if (hit)
            {
                board[x, y] = 2;
                gameOver = database.areAllShipsSunk(board);
            }

            // Deklarera vinnare om alla skepp är sänkta
            if (gameOver)
            {
                string playerName = database.getPlayerNamefromGame(gameNumber, playerNumber);
                database.declareWinner(gameNumber, playerName);
            }
            

            if (playerNumber == 1)
            {
                return RedirectToAction("SeaBattle", new { gameId = gameNumber, message = message, gameOver = gameOver });
            }
            else
            {
                return RedirectToAction("SeaBattle2", new { gameId = gameNumber, message = message, gameOver = gameOver });
            }
        }

    }
}
