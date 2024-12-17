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
        public IActionResult PlaceDoubleShip(string target, string orientation, int x, int y)
        {
            int length = 2;
            if (battleField.PlayerBoard == null)
                return View("Error"); // Kontrollera att brädet existerar

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + length - 1 < battleField.PlayerBoard.GetLength(1) && y >= 0 && y < battleField.PlayerBoard.GetLength(0) && PosIsEmpty(target, orientation, x, y, length))
                {
                    battleField.PlaceBireme(target, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < battleField.PlayerBoard.GetLength(0) && x >= 0 && x < battleField.PlayerBoard.GetLength(1) && PosIsEmpty(target, orientation, x, y, length))
                {
                   battleField.PlaceBireme(target, orientation, x, y);
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

            if (target == "Robot")
            {
                
                return View("~/Views/Battle/AddShipP2.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }
        }
        public IActionResult PlaceTrippleShip(string target, string orientation, int x, int y)
        {
            int length = 3;
            if (battleField.PlayerBoard == null)
                return View("Error"); // Kontrollera att brädet existerar

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + (length - 1) < battleField.PlayerBoard.GetLength(1) && y >= 0 && y < battleField.PlayerBoard.GetLength(0)&& PosIsEmpty(target, orientation, x, y, length))
                {
                        battleField.PlaceTrireme(target, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    return View("~/Views/Battle/AddShipP1.cshtml", battleField);
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length - 1 < battleField.PlayerBoard.GetLength(0) && x >= 0 && x < battleField.PlayerBoard.GetLength(1) && PosIsEmpty(target, orientation, x, y, length))
                {
                    battleField.PlaceTrireme(target, orientation, x, y);
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

            if (target == "Robot")
            {

                return View("~/Views/Battle/AddShipP2.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }
        }
        public IActionResult PlaceLongShip(string target, string orientation, int x, int y)
        {
            int length = 4;
            if (battleField.PlayerBoard == null)
                return View("Error"); // Kontrollera att brädet existerar 

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + length-1 < battleField.PlayerBoard.GetLength(1) && y >= 0 && y < battleField.PlayerBoard.GetLength(0) && PosIsEmpty(target, orientation, x,y, length))
                {
                    battleField.PlaceLongship(target, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                    
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + length-1 < battleField.PlayerBoard.GetLength(0) && x >= 0 && x < battleField.PlayerBoard.GetLength(1) && PosIsEmpty(target, orientation, x, y, length))
                {
                    battleField.PlaceLongship(target, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för Vertikalt skepp.";
                    
                }
            }
            else
            {
                ViewBag.Message = "Ogiltig orientering.";
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }

            if (target == "Robot")
            {

                return View("~/Views/Battle/AddShipP2.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }
        }
        public IActionResult PlaceTitanic(string target, string orientation, int x, int y)
        {
            int length = 5;
            if (battleField.PlayerBoard == null)
                return View("Error"); // Kontrollera att brädet existerar
        
            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "hor")
            {
                if (x >= 0 && x + 4 < battleField.PlayerBoard.GetLength(1) && y >= 0 && y < battleField.PlayerBoard.GetLength(0) && PosIsEmpty(target, orientation, x, y, length))
                {
                    battleField.PlaceTitanic(target, orientation, x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för horizontellt skepp.";
                   
                }
            }
            else if (orientation == "vert")
            {
                if (y >= 0 && y + 4 < battleField.PlayerBoard.GetLength(0) && x >= 0 && x < battleField.PlayerBoard.GetLength(1) && PosIsEmpty(target,orientation, x, y, length))
                {
                    battleField.PlaceTitanic(target, orientation, x, y);
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

            if (target == "Robot")
            {

                return View("~/Views/Battle/AddShipP2.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/AddShipP1.cshtml", battleField);
            }
        }
        public IActionResult Fire(string target, int x, int y)
{
            // Användarens skott
            bool hit = battleField.Shoot(target, x, y);
            ViewBag.Message = hit ? "Träff!" : "Miss!";

            // Kontrollera om spelet är över efter användarens skott
             if (battleField.AreAllRobotShipsSunk())
             {
                ViewBag.Message += " Alla Robotens skepp är sänkta! Spelaren vinner!";
                return View("~/Views/Battle/Result.cshtml", battleField);
             }

            
            if (battleField.AreAllPlayerShipsSunk())
            {
                ViewBag.Message += " Alla spelarens skepp är sänkta! Roboten vinner!";
                return View("~/Views/Battle/Result.cshtml", battleField);
            }

            if (target == "Robot")
            {

                return View("~/Views/Battle/SeaBattle.cshtml", battleField);
            }
            else
            {
                return View("~/Views/Battle/SeaBattle2.cshtml", battleField);
            }
        }
        public bool PosIsEmpty(string target, string orientation, int x, int y, int length)
        {
            int[,] board = target == "player" ? battleField.PlayerBoard : battleField.RobotBoard;

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
