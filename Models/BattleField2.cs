using System;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace BattleShits.Models
{
    public class BattleField2
    {
        public int[,] P1Board { get; set; } // Spelbräde (0 = tomt, 1 = skepp, 2 = träff, 3 = miss)
        public int[,] P2Board { get; set; }
        public List<(int y, int x)> P1Ships { get; set; }
        public List<(int y, int x)> P2Ships { get; set; }
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
            P1Board = new int[10, 10];
            P2Board = new int[10, 10];
            P1Ships = new List<(string name, int y, int x)>();
            P2Ships = new List<(string name, int y, int x)>();
        }

        public void PlaceBireme(string user, string orientation, int x, int y)
        {
            // Kontrollera vilket bräde och lista som ska användas
            int[,] board = user == "P1" ? P1Board : P2Board;
            var shipList = user == "P1" ? P1Ships : P2Ships;

            // Validera orientering och placering
            if (orientation == "hor")
            {
                shipList.Add(( y, x));
                shipList.Add((y, x + 1));
                id++;
            }
            else if (orientation == "vert")
            {
                shipList.Add((y, x));
                shipList.Add((y + 1, x));
                id++;
            }
            else
            {
                Console.WriteLine("Ogiltig orientering.");
            }
        }

        public void PlaceTrireme(string user, string orientation, int x, int y)
        {
            // Kontrollera bräde och lista baserat på target
            int[,] board = user == "P1" ? P1Board : P2Board;
            var shipList = user == "P1" ? P1Ships : P2Ships;

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

        public void PlaceLongship(string user, string orientation, int x, int y)
        {
            if (!CanPlaceLongship)
            {
                Console.WriteLine("Du har redan placerat Longship!");
                return;
            }

            // Kontrollera bräde och lista baserat på target
            int[,] board = user == "P1" ? P1Board : P2Board;
            var shipList = user == "P1" ? P1Ships : P2Ships;

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

        public void PlaceTitanic(string user, string orientation, int x, int y)
        {
            if (!CanPlaceTitanic)
            {
                Console.WriteLine("Du har redan placerat Titanic!");
                return;
            }

            int[,] board = user == "P1" ? P1Board : P2Board;
            var shipList = user == "P1" ? P1Ships : P2Ships;

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
        public bool Shoot(string user, int x, int y)
        {
            // Kontrollera vilket bräde och lista som ska användas beroende på vem som skjuter
            int[,] targetBoard = user == "P1" ? P2Board : P1Board;
            var targetShips = user == "P1" ? P2Ships : P1Ships;

            // Logga skott 
            if (user == "P1")
            {
                P1shots++;
            }
            else
            {
                P2shots++;
            }

            // Kolla om skottet träffar ett skepp
            if (targetBoard[y, x] == 1)
            {
                if (user == "Player1") P1Hits++; // Öka träffarna för Spelare 1
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





        public bool AreAllP1ShipsSunk()
        {
            if (P2Hits == 30)
            {
                Winner = "Player 2";
                return true;
            }
            return false;
        }
        public bool AreAllP2ShipsSunk()
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
