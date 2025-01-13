using System.Diagnostics;
using BattleShits.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BattleShits.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString = "Data Source=battleshipsserver.database.windows.net;Initial Catalog=Battleships;User ID=sqladmin;Password=Skola123;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public class Leaderboard
        {
            public string Player { get; set; }
            public int Wins { get; set; }
        }

        private List<Leaderboard> GetLeaderboard()
        {
            var leaderboard = new List<Leaderboard>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT 
                        Winner AS Player, 
                        COUNT(*) AS Wins
                    FROM 
                        Game
                    WHERE 
                        GameFinished = 1 AND Winner IS NOT NULL
                    GROUP BY 
                        Winner
                    ORDER BY 
                        Wins DESC", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        leaderboard.Add(new Leaderboard
                        {
                            Player = reader["Player"].ToString(),
                            Wins = Convert.ToInt32(reader["Wins"])
                        });
                    }
                }
            }

            return leaderboard;
        }

        public async Task<IActionResult> Index()
        {
            var leaderboard = GetLeaderboard();
            return View(leaderboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
