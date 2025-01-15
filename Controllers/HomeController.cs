using System.Diagnostics;
using BattleShits.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BattleShits.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        // Rasmus string
        private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        

        //Micke string
        //private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BattleLocal;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        //private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Battleships;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public class Leaderboard
        {
            public string Player { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public decimal WinRate { get; set; }
            public int TotalShots { get; set; }
            public int Hits { get; set; }
            public decimal HitRate { get; set; }
        }

        private List<Leaderboard> GetLeaderboard()
        {
            var leaderboard = new List<Leaderboard>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
            SELECT 
                PlayerStats.Player,
                SUM(PlayerStats.Wins) AS Wins,
                SUM(PlayerStats.Losses) AS Losses,
                COUNT(s.Id) AS TotalShots,
                SUM(CASE WHEN s.Hit = 1 THEN 1 ELSE 0 END) AS Hits,
                CASE 
                    WHEN COUNT(s.Id) > 0 THEN CAST(SUM(CASE WHEN s.Hit = 1 THEN 1 ELSE 0 END) * 100.0 / COUNT(s.Id) AS DECIMAL(5, 2))
                    ELSE 0
                END AS HitRate,
                CASE 
                    WHEN SUM(PlayerStats.Wins) + SUM(PlayerStats.Losses) > 0 
                    THEN CAST(SUM(PlayerStats.Wins) * 100.0 / (SUM(PlayerStats.Wins) + SUM(PlayerStats.Losses)) AS DECIMAL(5, 2))
                    ELSE 0
                END AS WinRate
            FROM (
                SELECT 
                    g.Winner AS Player,
                    1 AS Wins,
                    0 AS Losses,
                    g.Id AS GameId
                FROM 
                    Game g
                WHERE 
                    g.GameFinished = 1 AND g.Winner IS NOT NULL

                UNION ALL

                SELECT 
                    CASE 
                        WHEN g.Winner = g.Player1 THEN g.Player2
                        ELSE g.Player1
                    END AS Player,
                    0 AS Wins,
                    1 AS Losses,
                    g.Id AS GameId
                FROM 
                    Game g
                WHERE 
                    g.GameFinished = 1 AND g.Winner IS NOT NULL
            ) AS PlayerStats
            LEFT JOIN 
                Shots s ON PlayerStats.GameId = s.Game_Id AND PlayerStats.Player = s.Player
            GROUP BY 
                PlayerStats.Player
            ORDER BY 
                Wins DESC, WinRate DESC;", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        leaderboard.Add(new Leaderboard
                        {
                            Player = reader["Player"].ToString(),
                            Wins = Convert.ToInt32(reader["Wins"]),
                            Losses = Convert.ToInt32(reader["Losses"]),
                            WinRate = Convert.ToDecimal(reader["WinRate"]),
                            TotalShots = Convert.ToInt32(reader["TotalShots"]),
                            Hits = Convert.ToInt32(reader["Hits"]),
                            HitRate = Convert.ToDecimal(reader["HitRate"])
                        });
                    }
                }
            }

            return leaderboard;
        }

        public IActionResult Index(string sortField = "Wins", string sortOrder = "desc")
        {
            var leaderboard = GetLeaderboard();

            // Sortera baserat på det valda fältet och ordningen
            leaderboard = sortOrder.ToLower() switch
            {
                "asc" => sortField.ToLower() switch
                {
                    "wins" => leaderboard.OrderBy(x => x.Wins).ToList(),
                    "hitrate" => leaderboard.OrderBy(x => x.HitRate).ToList(),
                    "winrate" => leaderboard.OrderBy(x => x.WinRate).ToList(),
                    _ => leaderboard.OrderBy(x => x.Wins).ToList()
                },
                "desc" => sortField.ToLower() switch
                {
                    "wins" => leaderboard.OrderByDescending(x => x.Wins).ToList(),
                    "hitrate" => leaderboard.OrderByDescending(x => x.HitRate).ToList(),
                    "winrate" => leaderboard.OrderByDescending(x => x.WinRate).ToList(),
                    _ => leaderboard.OrderByDescending(x => x.Wins).ToList()
                },
                _ => leaderboard.OrderByDescending(x => x.Wins).ToList()
            };

            ViewBag.SortField = sortField;
            ViewBag.SortOrder = sortOrder;

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
