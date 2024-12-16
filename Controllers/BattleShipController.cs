using BattleShits.Models;
using Microsoft.AspNetCore.Mvc;

namespace BattleShits.Controllers
{

    public class BattleShipController : Controller
    {
        private static BattleField battleField = new BattleField();

        public IActionResult AddShipP1()
        {
            battleField = new BattleField();
            return View("~/Views/Battle/AddShipP1.cshtml", battleField);
        }

        public IActionResult SeaBattle()
        {
            return View("~/Views/Battle/SeaBattle.cshtml", battleField);
        }
        public IActionResult Result()
        {
            return View("~/Views/Battle/Result.cshtml", battleField);
        }
        public IActionResult PlaceShip(int x, int y)
        {
            if (battleField.PlayerBoard[x, y] == 0) // Kontrollera att rutan är tom
            {
                battleField.PlaceSingleShip(x, y);
                ViewBag.Message = "Skepp placerat!";
            }
            else
            {
                ViewBag.Message = "Platsen är redan upptagen!";
            }
            return View("~/Views/Battle/AddShipP1.cshtml", battleField);
        }
        [HttpPost]
        public IActionResult PlaceDoubleShip(string orientation, int x, int y)
        {
            int length = 2;
            if (battleField.PlayerBoard == null)
                return View("Error"); // Kontrollera att brädet existerar

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + length - 1 < battleField.PlayerBoard.GetLength(1) && y >= 0 && y < battleField.PlayerBoard.GetLength(0) && PosIsEmpty(orientation, x, y, length))
                {
                    battleField.PlaceBireme(orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < battleField.PlayerBoard.GetLength(0) && x >= 0 && x < battleField.PlayerBoard.GetLength(1) && PosIsEmpty(orientation, x, y, length))
                {
                   battleField.PlaceBireme(orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för vertikalt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else
            {
                ViewBag.Message = "Ogiltig orientering.";
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }

            ViewBag.Message = "Bireme placerat!";
            return View("~/Views/Battle/AddShipP1.cshtml", battleField);
        }
        public IActionResult PlaceTrippleShip(string orientation, int x, int y)
        {
            int length = 3;
            if (battleField.PlayerBoard == null)
                return View("Error"); // Kontrollera att brädet existerar

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + (length - 1) < battleField.PlayerBoard.GetLength(1) && y >= 0 && y < battleField.PlayerBoard.GetLength(0)&& PosIsEmpty(orientation, x, y, length))
                {
                        battleField.PlaceTrireme(orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < battleField.PlayerBoard.GetLength(0) && x >= 0 && x < battleField.PlayerBoard.GetLength(1) && PosIsEmpty(orientation, x, y, length))
                {
                    battleField.PlaceTrireme(orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för Vertikalt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else
            {
                ViewBag.Message = "Ogiltig orientering.";
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }

            ViewBag.Message = "Trireme placerat!";
            return View("~/Views/Battle/AddShipP1.cshtml", battleField);
        }
        public IActionResult PlaceLongShip(string orientation, int x, int y)
        {
            int length = 4;
            if (battleField.PlayerBoard == null)
                return View("Error"); // Kontrollera att brädet existerar 

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + length-1 < battleField.PlayerBoard.GetLength(1) && y >= 0 && y < battleField.PlayerBoard.GetLength(0) && PosIsEmpty(orientation, x,y, length))
                {
                    battleField.PlaceLongShip(orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length-1 < battleField.PlayerBoard.GetLength(0) && x >= 0 && x < battleField.PlayerBoard.GetLength(1) && PosIsEmpty(orientation, x, y, length))
                {
                    battleField.PlaceLongShip(orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för Vertikalt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else
            {
                ViewBag.Message = "Ogiltig orientering.";
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }

            ViewBag.Message = "Longship placerat!";
            return View("~/Views/Battle/AddShipP1.cshtml", battleField);
        }
        public IActionResult PlaceTitanic(string orientation, int x, int y)
        {
            int length = 5;
            if (battleField.PlayerBoard == null)
                return View("Error"); // Kontrollera att brädet existerar
        
            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + 4 < battleField.PlayerBoard.GetLength(1) && y >= 0 && y < battleField.PlayerBoard.GetLength(0) && PosIsEmpty(orientation, x, y, length))
                {
                    battleField.PlaceTitanic(orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + 4 < battleField.PlayerBoard.GetLength(0) && x >= 0 && x < battleField.PlayerBoard.GetLength(1) && PosIsEmpty(orientation, x, y, length))
                {
                    battleField.PlaceTitanic(orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för Vertikalt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else
            {
                ViewBag.Message = "Ogiltig orientering.";
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }

            ViewBag.Message = "Titanic placerat!";
            return View("~/Views/Battle/AddShipP1.cshtml", battleField);
        }
        public IActionResult Fire(int x, int y)
{
            // Användarens skott
            bool hit = battleField.Shoot(x, y);
            ViewBag.Message = hit ? "Träff!" : "Miss!";

            // Kontrollera om spelet är över efter användarens skott
             if (battleField.AreAllRobotShipsSunk())
             {
                ViewBag.Message += " Alla Robotens skepp är sänkta! Spelaren vinner!";
                return View("~/Views/Battle/Result.cshtml", battleField);
             }

            // Roboten skjuter
            var robotResult = battleField.RobotShoot();
            ViewBag.Message += robotResult ? "Roboten träffade!" : "Roboten Missade!";
            if (battleField.AreAllPlayerShipsSunk())
            {
                ViewBag.Message += " Alla spelarens skepp är sänkta! Roboten vinner!";
                return View("~/Views/Battle/Result.cshtml", battleField);
            }

            return View("~/Views/Battle/SeaBattle.cshtml", battleField);
        }
        public bool PosIsEmpty(string orientation, int x, int y, int length)
        {
            if (orientation == "hor")
            {
                for (int i = 0; i < length; i++)
                {
                    if (battleField.PlayerBoard[y, x + i] !=0 )
                    {
                        ViewBag.Message("Platsen är upptagen!");
                        return false;
                    }
                }
            }
            else if (orientation == "vert")
            {
                for (int i = 0; i < length; i++)
                {
                    if (battleField.PlayerBoard[y + i, x] != 0)
                    {
                        ViewBag.Message("Platsen är upptagen!");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
