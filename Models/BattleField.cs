using System;
using System.Security.Cryptography;

namespace BattleShits.Models
{
    public class BattleField
    {
        public int[,] PlayerBoard { get; set; } // Spelbräde (0 = tomt, 1 = skepp, 2 = träff, 3 = miss)
        public int[,] RobotBoard { get; set; }
        public List<(string name, int y, int x)> PlayerShips {get; set; }
        public List<(string name, int y, int x)> RobotShips { get; set; }

        public BattleField()
        {
            PlayerBoard = new int[3, 3];
            RobotBoard = new int[3, 3];
            PlayerShips = new List<(string name,int y, int x)>();
            RobotShips = new List<(string name, int y, int x)>();
            RandomShips();
        }

        // Lägg till skepp på specifika koordinater
        public void PlaceSingleShip(int x, int y)
        {
            PlayerShips.Add(("Single", x, y));
            PlayerBoard[y, x] = 1; 
        }
        public void PlaceDoubleShip(int x, int y)
        {
            // Använd unika namn för varje variabel
            Console.WriteLine("Ange orientering (lodrätt eller vågrätt): ");
            string orientation = Console.ReadLine()?.ToLower(); // Läs orienteringen och omvandla till små bokstäver

            Console.WriteLine("Ange startposition (x y): ");
            string[] input = Console.ReadLine()?.Split(' '); // Läs in koordinaterna

            if (input == null || input.Length != 2 ||
                !int.TryParse(input[0], out int startX) || // Ändra namn på x till startX
                !int.TryParse(input[1], out int startY))   // Ändra namn på y till startY
            {
                Console.WriteLine("Ogiltig input. Försök igen.");
                return;
            }

            // Validera orienteringen och positionen
            if (orientation == "lodrätt" && startX >= 0 && startX < 3 && startY >= 0 && startY < 2)
            {
                RobotBoard[startX, startY] = 1;
                RobotBoard[startX, startY + 1] = 1; // Lodrätt lägger till i nästa rad

                RobotShips.Add(("Double", startX, startY));
                RobotShips.Add(("Double", startX, startY + 1));
            }
            else if (orientation == "vågrätt" && startX >= 0 && startX < 2 && startY >= 0 && startY < 3)
            {
                RobotBoard[startX, startY] = 1;
                RobotBoard[startX + 1, startY] = 1; // Vågrätt lägger till i nästa kolumn
                RobotShips.Add(("Double", startX, startY));
                RobotShips.Add(("Double", startX + 1, startY));
            }
            else
            {
                Console.WriteLine("Ogiltig orientering eller startposition. Skeppet ryms inte på brädet.");
            }
        }


        public void RandomShips()
        {
            Random random = new Random();
            while(RobotShips.Count<3)
            {
                int x = random.Next(0, 3);
                int y = random.Next(0, 3);
                var newCoord = ("Double", y, x);

                if (!RobotShips.Contains(newCoord)) 
                {
                    RobotShips.Add(newCoord);
                    RobotBoard[y, x] = 1; 
                }
            }
        }

        // Spelare skjuter skott på en koordinat
        public bool Shoot(int x, int y)
        {
            if (RobotBoard[y, x] == 1) // Träff på skepp
            {
                RobotBoard[y, x] = 2; // Markera träff
                Console.WriteLine($"Träff på skepp vid koordinat ({y}, {x})!");

                // Kontrollera om hela skeppet är sänkt
                var shipCoordinates = RobotShips.Where(s => s.x == x && s.y == y).ToList();
                foreach (var ship in shipCoordinates)
                {
                    bool isSunk = true;

                    // Kolla om alla delar av skeppet är träffade (dvs. markeras som 2 på brädet)
                    foreach (var coord in RobotShips.Where(s => s.name == "Double"))
                    {
                        
                        if (RobotBoard[coord.y, coord.x] != 2) // Om någon del av skeppet inte är träffad
                        {
                            Console.WriteLine("You hit part of a bireme");
                            isSunk = false;
                            break;
                            
                        }
                    }

                    if (isSunk)
                    {
                        Console.WriteLine("You sunk a bireme!");
                    }
                }

                return true;
            }
            else if (RobotBoard[y, x] == 0) // Miss
            {
                RobotBoard[y, x] = 3; // Markera miss
                Console.WriteLine($"Miss vid koordinat ({y}, {x})");
                return false;
            }
            return false;
        }



        public bool RobotShoot()
        {
            Random random = new Random();
            int x = random.Next(0, 3);
            int y = random.Next(0, 3);

            if (PlayerBoard[y, x] == 1) // Träff på skepp
            {
                PlayerBoard[y, x] = 2; // Markera träff
                Console.WriteLine($"Robot träffade ditt skepp vid koordinat ({y}, {x})!");

                // Kontrollera om hela skeppet är sänkt
                var shipCoordinates = PlayerShips.Where(s => s.x == y && s.x == y).ToList();
                foreach (var ship in shipCoordinates)
                {
                    bool isSunk = true;

                    // Kolla om alla delar av skeppet är träffade (dvs. markeras som 2 på brädet)
                    foreach (var coord in PlayerShips.Where(s => s.name == ship.name))
                    {
                   
                        if (PlayerBoard[coord.y, coord.x] != 2) // Om någon del av skeppet inte är träffad
                        {
                            Console.WriteLine("Robot hit a part of a bireme");
                            isSunk = false;
                            break;
                        }
                    }

                    if (isSunk)
                    {
                        Console.WriteLine($"Robot sunk the entire bireme!");
                    }
                }

                return true;
            }
            else if (PlayerBoard[y, x] == 0) // Miss
            {
                PlayerBoard[y, x] = 3; // Markera miss
                Console.WriteLine($"Robot missade vid koordinat ({y}, {x})");
                return false;
            }
            return false;
        }

        public bool AreAllPlayerShipsSunk()
        {
            foreach (var (name, y, x) in PlayerShips)
            {
                if (PlayerBoard[y, x] != 2) return false;
            }
            return true;
        }
        public bool AreAllRobotShipsSunk()
        {
            foreach (var (name, y, x) in RobotShips)
            {
                if (RobotBoard[y, x] != 2) return false;
            }
            return true;
        }
    }

}
