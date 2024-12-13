using BattleShits.Models;
using Microsoft.AspNetCore.Mvc;

namespace BattleShits.Controllers
{

    public class BattleShipController : Controller
    {
        private static BattleField battleField = new BattleField();
        public IActionResult SeaBattle()
        {
            battleField = new BattleField();
            return View("~/Views/Battle/SeaBattle.cshtml", battleField);
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
            return View("~/Views/Battle/SeaBattle.cshtml", battleField);
        }
        [HttpPost]
        public IActionResult PlaceDoubleShip(string orientation, int x, int y)
        {
            if (battleField.PlayerBoard == null)
                return View("Error"); // Kontrollera att brädet existerar

            // Validera att skeppet kan placeras utan att gå utanför brädet
            if (orientation == "horizontal")
            {
                if (x >= 0 && x < battleField.PlayerBoard.GetLength(0) - 1 && y >= 0 && y < battleField.PlayerBoard.GetLength(1))
                {
                    battleField.PlayerBoard[x, y] = 1;
                    battleField.PlayerBoard[x + 1, y] = 1;
                    battleField.PlaceDoubleShip(x, y);
                    
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för vågrätt skepp.";
                    return View("~/Views/Battle/SeaBattle.cshtml", battleField);
                }
            }
            else if (orientation == "vertical")
            {
                if (x >= 0 && x < battleField.PlayerBoard.GetLength(0) && y >= 0 && y < battleField.PlayerBoard.GetLength(1) - 1)
                {
                    battleField.PlayerBoard[x, y] = 1;
                    battleField.PlayerBoard[x, y + 1] = 1;
                    battleField.PlaceDoubleShip(x, y);
                }
                else
                {
                    ViewBag.Message = "Ogiltig placering för lodrätt skepp.";
                    return View("~/Views/Battle/SeaBattle.cshtml", battleField);
                }
            }
            else
            {
                ViewBag.Message = "Ogiltig orientering.";
                return View("~/Views/Battle/SeaBattle.cshtml", battleField);
            }

            ViewBag.Message = "Dubbelskepp placerat!";
            return View("~/Views/Battle/SeaBattle.cshtml", battleField);
        }


        // Skjut på ett mål
        public IActionResult Fire(int x, int y)
{
            // Användarens skott
            bool hit = battleField.Shoot(x, y);
            ViewBag.Message = hit ? "Träff!" : "Miss!";

            // Kontrollera om spelet är över efter användarens skott
             if (battleField.AreAllRobotShipsSunk())
             {
                ViewBag.Message += " Alla Robotens skepp är sänkta! Spelaren vinner!";
                return View("~/Views/Battle/SeaBattle.cshtml", battleField);
             }

            // Roboten skjuter
            var robotResult = battleField.RobotShoot();
            ViewBag.Message += robotResult ? "Roboten träffade!" : "Roboten Missade!";
            if (battleField.AreAllPlayerShipsSunk())
            {
                ViewBag.Message += " Alla spelarens skepp är sänkta! Roboten vinner!";
                return View("~/Views/Battle/SeaBattle.cshtml", battleField);
            }

            return View("~/Views/Battle/SeaBattle.cshtml", battleField);
        }
    }
}
