using BattleShits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BattleShits.Controllers
{
    [AllowAnonymous]
    public class BattleField2Controller : Controller
    {
        /*private static BattleField2 battleField = new BattleField2();*/
        private static DatabaseMethods database = new DatabaseMethods();

        public IActionResult AddShipP1(bool firstStart, int gameNumber, string message)
        {

            int gameId = gameNumber;
            if (firstStart == true)
            {
                gameId = database.CreateGame("P1", "P2");
            }
            
            
            int[,] p1Board = database.getBoard(gameId,1);
            string playerName = database.getPlayerNamefromGame(gameId, 1);

            // Check if ships can be blaced
            bool titanicExists = database.doesShipExist(database.getBoardNumber(gameId, 1), 1, "Titanic");
            bool longShip2Exists = database.doesShipExist(database.getBoardNumber(gameId, 1), 1, "LongShip2");
            bool trippleShip3Exists = database.doesShipExist(database.getBoardNumber(gameId, 1), 1, "TrippleShip3");
            bool doubleShip4Exists = database.doesShipExist(database.getBoardNumber(gameId, 1), 1, "DoubleShip4");
            
            ViewBag.TitanicExists = titanicExists;
            ViewBag.Longship2Exists = longShip2Exists;
            ViewBag.Trippleship3Exists = trippleShip3Exists;
            ViewBag.Doubleship4Exists = doubleShip4Exists;

            ViewBag.GameId = gameId;
            ViewBag.PlayerName = playerName;
            ViewBag.p1Board = p1Board;
            ViewBag.Message = message;
            

            return View("~/Views/Battle2/AddShipP1.cshtml", database);
        }
        public IActionResult AddShipP2()
        {
            int gameId = database.CreateGame("P1", "P2");
            ViewBag.GameId = gameId;

            return View("~/Views/Battle2/AddShipP2.cshtml", database);
        }

        public IActionResult SeaBattle(int gameId, int boardNumber)
        {
            ViewBag.boardNumber = boardNumber;
            ViewBag.gameId = gameId;
            ViewBag.boardNumber2 = 2;
            if (boardNumber == 2)
            {
                ViewBag.boardNumber2 = 1;
            }
            return View("~/Views/Battle2/SeaBattle.cshtml", database);
        }
        public IActionResult SeaBattle2(int gameId, int boardNumber)
        {
            ViewBag.boardNumber = boardNumber;
            ViewBag.gameId = gameId;
            ViewBag.boardNumber2 = 2;
            if (boardNumber == 2)
            {
                ViewBag.boardNumber2 = 1;
            }
            return View("~/Views/Battle2/SeaBattle2.cshtml", database);
        }
        public IActionResult Result()
        {
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
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
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
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                }
            }
            else
            {
                string message = "Ogiltig orientering.";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }

            if (playerNumber == 1)
            {
                string message = "";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }
            else
            {
                return View("~/Views/Battle2/AddShipP2.cshtml", database);
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
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
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
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                }
            }
            else
            {
                string message = "Ogiltig orientering.";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }

            if (playerNumber == 1)
            {
                string message = "";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }
            else
            {
                return View("~/Views/Battle2/AddShipP2.cshtml", database);
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
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
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
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                }
            }
            else
            {
                string message = "Ogiltig orientering.";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }

            if (playerNumber == 1)
            {
                string message = "";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }
            else
            {
                return View("~/Views/Battle2/AddShipP2.cshtml", database);
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
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
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
                    return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
                }
            }
            else
            {
                string message = "Ogiltig orientering.";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }

            if (playerNumber == 1)
            {
                string message = "";
                return RedirectToAction("AddShipP1", new { firstStart = false, gameNumber = gameId, message = message });
            }
            else
            {
                return View("~/Views/Battle2/AddShipP2.cshtml", database);
            }
        }
        /*
        public IActionResult Fire(string user, int x, int y)
        {
            // Användarens skott
            bool hit = battleField.Shoot(user, x, y);
            ViewBag.Message = hit ? "Träff!" : "Miss!";

            // Kontrollera om spelet är över efter användarens skott
            if (battleField.AreAllP2ShipsSunk())
            {
                ViewBag.Message += " Alla Spealre 2's skepp är sänkta! Spelare 1 vinner!";
                return View("~/Views/Battle2/Result.cshtml", database);
            }


            if (battleField.AreAllP1ShipsSunk())
            {
                ViewBag.Message += " Alla spelare 1's skepp är sänkta! Spelare 2 vinner!";
                return View("~/Views/Battle2/Result.cshtml", battleField);
            }

            if (user == "P1")
            {

                return View("~/Views/Battle2/SeaBattle.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle2/SeaBattle2.cshtml", battleField);
            }
        }*/



    }
}
