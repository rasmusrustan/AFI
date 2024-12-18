using BattleShits.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BattleShits.Controllers
{
    [AllowAnonymous]
    public class BattleShipController : Controller
    {
        private static BattleField battleField = new BattleField();

        public IActionResult AddShipP1()
        {
          
            return View("~/Views/Battle/AddShipP1.cshtml", battleField);
        }
        public IActionResult AddShipP2()
        {
            
            return View("~/Views/Battle/AddShipP2.cshtml", battleField);
        }

        public IActionResult SeaBattle()
        {
            return View("~/Views/Battle/SeaBattle.cshtml", battleField);
        }
        public IActionResult SeaBattle2()
        {
            return View("~/Views/Battle/SeaBattle2.cshtml", battleField);
        }
        public IActionResult Result()
        {
            return View("~/Views/Battle/Result.cshtml", battleField);
        }
       
        [HttpPost]
        public IActionResult PlaceDoubleShip(string user, string orientation, int x, int y)
        {
            int length = 2;
            if (battleField.P1Board == null)
                return View("Error"); // Kontrollera att brädet existerar

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + length - 1 < battleField.P1Board.GetLength(1) && y >= 0 && y < battleField.P1Board.GetLength(0) && PosIsEmpty(user, orientation, x, y, length))
                {
                    battleField.PlaceBireme(user, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < battleField.P1Board.GetLength(0) && x >= 0 && x < battleField.P1Board.GetLength(1) && PosIsEmpty(user, orientation, x, y, length))
                {
                   battleField.PlaceBireme(user, orientation, x, y);
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

            if (user == "P1")
            {
                
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/AddShipP2.cshtml", battleField);
            }
        }
        public IActionResult PlaceTrippleShip(string user, string orientation, int x, int y)
        {
            int length = 3;
            if (battleField.P1Board == null)
                return View("Error"); // Kontrollera att brädet existerar

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + (length - 1) < battleField.P1Board.GetLength(1) && y >= 0 && y < battleField.P1Board.GetLength(0)&& PosIsEmpty(user, orientation, x, y, length))
                {
                        battleField.PlaceTrireme(user, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < battleField.P1Board.GetLength(0) && x >= 0 && x < battleField.P1Board.GetLength(1) && PosIsEmpty(user, orientation, x, y, length))
                {
                    battleField.PlaceTrireme(user, orientation, x, y);
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

            if (user == "P1")
            {

                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/AddShipP2.cshtml", battleField);
            }
        }
        public IActionResult PlaceLongShip(string user, string orientation, int x, int y)
        {
            int length = 4;
            if (battleField.P1Board == null)
                return View("Error"); // Kontrollera att brädet existerar 

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + length-1 < battleField.P1Board.GetLength(1) && y >= 0 && y < battleField.P1Board.GetLength(0) && PosIsEmpty(user, orientation, x,y, length))
                {
                    battleField.PlaceLongship(user, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length-1 < battleField.P1Board.GetLength(0) && x >= 0 && x < battleField.P1Board.GetLength(1) && PosIsEmpty(user, orientation, x, y, length))
                {
                    battleField.PlaceLongship(user, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för Vertikalt skepp.";
                    
                }
            }
            else
            {
                ViewBag.Message = "Ogiltig orientering.";
            }

            if (user == "P1")
            {

                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/AddShipP2.cshtml", battleField);
            }
        }
        public IActionResult PlaceTitanic(string user, string orientation, int x, int y)
        {
            int length = 5;
            if (battleField.P1Board == null)
                return View("Error"); // Kontrollera att brädet existerar
        
            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + 4 < battleField.P1Board.GetLength(1) && y >= 0 && y < battleField.P1Board.GetLength(0) && PosIsEmpty(user, orientation, x, y, length))
                {
                    battleField.PlaceTitanic(user, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                   
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + 4 < battleField.P1Board.GetLength(0) && x >= 0 && x < battleField.P1Board.GetLength(1) && PosIsEmpty(user,orientation, x, y, length))
                {
                    battleField.PlaceTitanic(user, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för Vertikalt skepp.";
                    
                }
            }
            else
            {
                ViewBag.Message = "Ogiltig orientering.";
                
            }

            if (user == "P1")
            {

                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/AddShipP2.cshtml", battleField);
            }
        }
        public IActionResult Fire(string user, int x, int y)
{
            // Användarens skott
            bool hit = battleField.Shoot(user, x, y);
            ViewBag.Message = hit ? "Träff!" : "Miss!";

            // Kontrollera om spelet är över efter användarens skott
             if (battleField.AreAllP2ShipsSunk())
             {
                ViewBag.Message += " Alla Spealre 2's skepp är sänkta! Spelare 1 vinner!";
                return View("~/Views/Battle/Result.cshtml", battleField);
             }

            
            if (battleField.AreAllP1ShipsSunk())
            {
                ViewBag.Message += " Alla spelare 1's skepp är sänkta! Spelare 2 vinner!";
                return View("~/Views/Battle/Result.cshtml", battleField);
            }

            if (user == "P1")
            {

                return View("~/Views/Battle/SeaBattle.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/SeaBattle2.cshtml", battleField);
            }
        }
        public bool PosIsEmpty(string user, string orientation, int x, int y, int length)
        {
            int[,] board = user == "P1" ? battleField.P1Board : battleField.P2Board;

            if (orientation == "hor")
            {
                for (int i = 0; i < length; i++)
                {
                    if (board[y, x + i] != 0)
                    {
                        ViewBag.Message = "Platsen är upptagen!";
                        return false;
                    }
                }
            }
            else if (orientation == "vert")
            {
                for (int i = 0; i < length; i++)
                {
                    if (board[y + i, x] != 0)
                    {
                        ViewBag.Message = "Platsen är gen!";
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
