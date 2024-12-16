using BattleShits.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BattleShits.Controllers
{
    [AllowAnonymous]
    public class UsersController : Controller
    {
        private readonly UsersMethods userMethods;

        public UsersController(UsersMethods userMethods)
        {
            this.userMethods = userMethods;
        }

        // GET: /User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            if (userMethods.GetUserByUsername(username) != null)
            {
                ViewBag.ErrorMessage = "Username already exists.";
                return View();
            }

            var newUser = new Users
            {
                Username = username,
                Password = password
            };

            userMethods.AddUser(newUser);

            TempData["SuccessMessage"] = "Registration successful. You can now log in.";
            return RedirectToAction("Login");
        }

        // GET: /User/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var isValidUser = userMethods.ValidateUser(username, password);

            if (isValidUser)
            {
                var user = userMethods.GetUserByUsername(username);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal);

                TempData["SuccessMessage"] = $"Welcome, {username}!";
                return RedirectToAction("test", "Users");
            }

            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }

        [HttpGet]
        public IActionResult Test() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Users");
        }
    }
}
