using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BattleShits.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Storage;

namespace BattleShits.Controllers
{
    [Authorize]
    public class LobbyController : Controller
    {
        private static DatabaseMethods database = new DatabaseMethods();
        // Visa lobbyn för värden
        public IActionResult HostLobby()
        {
            var lobbyId = Guid.NewGuid().ToString(); // Generera unikt lobby-ID
            var invitationUrl = Url.Action("JoinLobby", "Lobby", new { lobbyId }, Request.Scheme); // Skapa URL
            ViewBag.LobbyId = lobbyId;
            ViewBag.InvitationUrl = invitationUrl; // Skicka URL till vyn
            return View();
        }

        // Visa lobbyn för gäster
        public IActionResult JoinLobby(string lobbyId)
        {
            ViewBag.LobbyId = lobbyId;
            return View();
        }
    }
}