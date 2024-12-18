using System;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace BattleShits.Models
{
    public class BattleField2
    {
        public int[,] PlayerBoard { get; set; } // Spelbräde (0 = tomt, 1 = skepp, 2 = träff, 3 = miss)
        public int[,] RobotBoard { get; set; }
        public List<(string name, int y, int x)> PlayerShips { get; set; }
        public List<(string name, int y, int x)> RobotShips { get; set; }
        public int id = 0;

        public int TitanicCount { get; private set; } = 0;
        public int LongshipCount { get; private set; } = 0;
        public int TriremeCount { get; private set; } = 0;
        public int BiremeCount { get; private set; } = 0;

        // Max antal skepp av varje typ
        public const int MaxTitanic = 1;
        public const int MaxLongship = 2;
        public const int MaxTrireme = 3;
        public const int MaxBireme = 4;

        public bool CanPlaceTitanic => TitanicCount < MaxTitanic;
        public bool CanPlaceLongship => LongshipCount < MaxLongship;
        public bool CanPlaceTrireme => TriremeCount < MaxTrireme;
        public bool CanPlaceBireme => BiremeCount < MaxBireme;

        public bool AllShipsPlaced =>
        TitanicCount == MaxTitanic &&
        LongshipCount == MaxLongship &&
        TriremeCount == MaxTrireme &&
        BiremeCount == MaxBireme;

        public int P1shots = 0;
        public int P2shots = 0;
        public int P1Hits = 0;
        public int P2Hits = 0;

        public string Winner = null;
        public BattleField2()
        {
            PlayerBoard = new int[10, 10];
            RobotBoard = new int[10, 10];
            PlayerShips = new List<(string name, int y, int x)>();
            RobotShips = new List<(string name, int y, int x)>();
        }

        public void PlaceBireme(string target, string orientation, int x, int y)
        {
            // Kontrollera vilket bräde och lista som ska användas
            int[,] board = target == "Robot" ? RobotBoard : PlayerBoard;
            var shipList = target == "Robot" ? RobotShips : PlayerShips;

            // Validera orientering och placering
            if (orientation == "hor")
            {
                if (x + 1 >= board.GetLength(1))
                {
                    Console.WriteLine("Skeppet ryms inte horisontellt på brädet.");
                    return;
                }

                board[y, x] = 1;
                board[y, x + 1] = 1;

                shipList.Add(("Bireme" + id, y, x));
                shipList.Add(("Bireme" + id, y, x + 1));
                id++;
            }
            else if (orientation == "vert")
            {
                if (y + 1 >= board.GetLength(0))
                {
                    Console.WriteLine("Skeppet ryms inte vertikalt på brädet.");
                    return;
                }

                board[y, x] = 1;
                board[y + 1, x] = 1;

                shipList.Add(("Bireme" + id, y, x));
                shipList.Add(("Bireme" + id, y + 1, x));
                id++;
            }
            else
            {
                Console.WriteLine("Ogiltig orientering.");
            }
        }

        public void PlaceTrireme(string target, string orientation, int x, int y)
        {
            // Kontrollera bräde och lista baserat på target
            int[,] board = target == "Robot" ? RobotBoard : PlayerBoard;
            var shipList = target == "Robot" ? RobotShips : PlayerShips;

            // Validera orienteringen och positionen
            if (orientation == "hor" && x + 2 < board.GetLength(1))
            {
                for (int i = 0; i < 3; i++)
                {
                    board[y, x + i] = 1;
                    shipList.Add(("Trireme" + id, y, x + i));
                }

                TriremeCount++;
                id++;
            }
            else if (orientation == "vert" && y + 2 < board.GetLength(0))
            {
                for (int i = 0; i < 3; i++)
                {
                    board[y + i, x] = 1;
                    shipList.Add(("Trireme" + id, y + i, x));
                }

                TriremeCount++;
                id++;
            }
            else
            {
                Console.WriteLine("Ogiltig orientering eller startposition. Skeppet ryms inte på brädet.");
            }
        }

        public void PlaceLongship(string target, string orientation, int x, int y)
        {
            if (!CanPlaceLongship)
            {
                Console.WriteLine("Du har redan placerat Longship!");
                return;
            }

            // Kontrollera bräde och lista baserat på target
            int[,] board = target == "Robot" ? RobotBoard : PlayerBoard;
            var shipList = target == "Robot" ? RobotShips : PlayerShips;

            // Validera orienteringen och positionen
            if (orientation == "hor" && x + 3 < board.GetLength(1))
            {
                for (int i = 0; i < 4; i++)
                {
                    board[y, x + i] = 1;
                    shipList.Add(("Longship" + id, y, x + i));
                }

                LongshipCount++;
                id++;
            }
            else if (orientation == "vert" && y + 3 < board.GetLength(0))
            {
                for (int i = 0; i < 4; i++)
                {
                    board[y + i, x] = 1;
                    shipList.Add(("Longship" + id, y + i, x));
                }

                LongshipCount++;
                id++;
            }
            else
            {
                Console.WriteLine("Ogiltig orientering eller startposition. Skeppet ryms inte på brädet.");
            }
        }

        public void PlaceTitanic(string target, string orientation, int x, int y)
        {
            if (!CanPlaceTitanic)
            {
                Console.WriteLine("Du har redan placerat Titanic!");
                return;
            }

            int[,] board = target == "Robot" ? RobotBoard : PlayerBoard;
            var shipList = target == "Robot" ? RobotShips : PlayerShips;

            if (orientation == "hor" && x + 4 < board.GetLength(1))
            {
                for (int i = 0; i < 5; i++)
                    board[y, x + i] = 1;

                for (int i = 0; i < 5; i++)
                    shipList.Add(("Titanic" + id, y, x + i));

                TitanicCount++;
                id++;
            }
            else if (orientation == "vert" && y + 4 < board.GetLength(0))
            {
                for (int i = 0; i < 5; i++)
                    board[y + i, x] = 1;

                for (int i = 0; i < 5; i++)
                    shipList.Add(("Titanic" + id, y + i, x));

                TitanicCount++;
                id++;
            }
            else
            {
                Console.WriteLine("Ogiltig orientering eller startposition.");
            }
        }



        // Spelare skjuter skott på en koordinat
        public bool Shoot(string target, int x, int y)
        {
            // Kontrollera vilket bräde och lista som ska användas beroende på vem som skjuter
            int[,] targetBoard = target == "Player" ? PlayerBoard : RobotBoard;
            var targetShips = target == "Player" ? PlayerShips : RobotShips;

            // Spelare 1 skjuter på Robot eller Spelare 2
            if (target == "Player")
            {
                P1shots++;
            }
            else // Spelare 2 skjuter på Robot eller Spelare 1
            {
                P2shots++;
            }

            // Kolla om skottet träffar ett skepp
            if (targetBoard[y, x] == 1) // Träff på skepp
            {
                if (target == "Player1") P1Hits++; // Öka träffarna för Spelare 1
                else P2Hits++; // Öka träffarna för Spelare 2

                targetBoard[y, x] = 2; // Markera träff

                Console.WriteLine($"Träff på skepp vid koordinat ({y}, {x})!");

                // Kontrollera om hela skeppet är sänkt
                var shipCoordinates = targetShips.Where(s => s.y == y && s.x == x).ToList();
                foreach (var ship in shipCoordinates)
                {
                    bool isSunk = true;

                    // Kolla om alla delar av skeppet är träffade
                    foreach (var coord in targetShips.Where(s => s.name == ship.name))
                    {
                        if (targetBoard[coord.y, coord.x] != 2) // Om någon del av skeppet inte är träffad
                        {
                            isSunk = false;
                            break;
                        }
                    }

                    if (isSunk)
                    {
                        Console.WriteLine($"Du sänkte ett {ship.name}!");
                    }
                }

                return true;
            }
            else if (targetBoard[y, x] == 0) // Miss
            {
                targetBoard[y, x] = 3; // Markera miss
                Console.WriteLine($"Miss vid koordinat ({y}, {x})");
                return false;
            }

            return false;
        }




        public bool RobotShoot()
        {
            P2shots++;
            Random random = new Random();
            int x = random.Next(0, 3);
            int y = random.Next(0, 3);

            if (PlayerBoard[y, x] == 1) // Träff på skepp
            {
                P2Hits++;
                PlayerBoard[y, x] = 2; // Markera träff
                Console.WriteLine($"Robot träffade ditt skepp vid koordinat ({y}, {x})!");

                // Kontrollera om hela skeppet är sänkt
                var shipCoordinates = PlayerShips.Where(s => s.y == y && s.x == x).ToList();
                foreach (var ship in shipCoordinates)
                {
                    bool isSunk = true;

                    // Kolla om alla delar av skeppet är träffade (dvs. markeras som 2 på brädet)
                    foreach (var coord in PlayerShips.Where(s => s.name == "name"))
                    {

                        if (PlayerBoard[coord.y, coord.x] != 2) // Om någon del av skeppet inte är träffad
                        {
                            Console.WriteLine("Robot hit part of a bireme");
                            isSunk = false;
                            break;

                        }
                    }

                    if (isSunk)
                    {
                        Console.WriteLine("Robot sunk a bireme!");
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
            if (P2Hits == 30)
            {
                Winner = "Player 2";
                return true;
            }
            return false;
        }
        public bool AreAllRobotShipsSunk()
        {
            if (P1Hits == 2)
            {
                Winner = "Player 1";
                return true;
            }
            return false;
        }
    }

}
